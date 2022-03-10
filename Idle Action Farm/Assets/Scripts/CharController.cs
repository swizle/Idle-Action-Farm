using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharController : MonoBehaviour
{
    [SerializeField] private GameObject objCharCap;
    [SerializeField] private MeshRenderer objScythe;
    [SerializeField] private Image objCoin;
    [SerializeField] private Animator objCoinAnim;
    [SerializeField] private GameObject objPan;

    private bool isSelling = false;

    private float charCap = 0;
    private float charMaxCap = 40;
    private List<GameObject> charCapList = new List<GameObject>();

    private float speedMove = 2f;
    private float gravityForce = -1f;
    private Vector3 moveVector;

    private CharacterController charController;
    private JoystickController joystickController;
    private Animator charAnim;
    private GameEconomic gameEconomic;
    private UIUpdate uiUpdate;

    private void Start()
    {
        charController = gameObject.GetComponent<CharacterController>();
        joystickController = GameObject.FindGameObjectWithTag("Joystick").GetComponent<JoystickController>();
        charAnim = gameObject.GetComponent<Animator>();
        gameEconomic = GameObject.FindGameObjectWithTag("EconomicManager").GetComponent<GameEconomic>();
        uiUpdate = GameObject.FindGameObjectWithTag("UIUpdater").GetComponent<UIUpdate>();
    }

    private void Update()
    {
        CharacterMove();
        GamingGravity();
    }

    private void CharacterMove()
    {
        if (charController.isGrounded)
        {
            moveVector = Vector3.zero;
            moveVector.x = joystickController.Horizontal() * speedMove;
            moveVector.z = joystickController.Vertical() * speedMove;

            if (moveVector.x != 0 || moveVector.z != 0)
            {
                objScythe.enabled = false;
                charAnim.ResetTrigger("isHarvest");
                charAnim.SetBool("isRun", true);
            }
            else
            {
                charAnim.SetBool("isRun", false);
            }

            if (Vector3.Angle(Vector3.forward, moveVector) > 1 || Vector3.Angle(Vector3.forward, moveVector) == 0)
            {
                Vector3 direct = Vector3.RotateTowards(transform.forward, moveVector, speedMove, 0);
                transform.rotation = Quaternion.LookRotation(direct);
            }
        }

        moveVector.y = gravityForce;
        charController.Move(moveVector * Time.deltaTime);
    }

    private void GamingGravity()
    {
        if (!charController.isGrounded) gravityForce -= 20 * Time.deltaTime;
        else gravityForce = -1;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Wheat"))
        {
            objScythe.enabled = true;
            charAnim.SetTrigger("isHarvest");
        }
        else if (hit.gameObject.CompareTag("SellZone"))
        {
            if (charCap != 0 && !isSelling)
            {
                isSelling = true;
                StartCoroutine(MoveBlocksToSell(hit.gameObject, (int)charCap));
            }
        }
    }

    public void CollectBlock(GameObject block)
    {
        charCapList.Add(block);
        ++charCap;
        uiUpdate.UpdateUI();
    }

    public float GetCharCap()
    {
        return charCap;
    }

    public float GetCharMaxCap()
    {
        return charMaxCap;
    }

    private IEnumerator MoveBlocksToSell(GameObject ambar, int capacity)
    {
        float timeElapsed = 0;
        float duration = 1f;

        charCapList[capacity - 1].transform.parent = ambar.transform.parent.transform;

        Vector3 startPos = charCapList[capacity - 1].transform.localPosition;
        Quaternion startRot = charCapList[capacity - 1].transform.localRotation;
        Vector3 targetPos = Vector3.zero;
        Quaternion targetRot = Quaternion.Euler(Vector3.zero);

        while (timeElapsed < duration)
        {
            charCapList[capacity - 1].transform.localPosition = Vector3.Lerp(startPos, targetPos, timeElapsed / duration);
            charCapList[capacity - 1].transform.localRotation = Quaternion.Lerp(startRot, targetRot, timeElapsed / duration);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        charCapList[capacity - 1].transform.localPosition = targetPos;
        charCapList[capacity - 1].transform.localRotation = targetRot;

        Destroy(charCapList[capacity - 1].gameObject);
        charCapList.RemoveAt(capacity - 1);

        timeElapsed = 0;

        Image coin = Instantiate(objCoin, ambar.transform.parent.position, Quaternion.identity, objPan.transform);

        startPos = coin.rectTransform.localPosition;
        startRot = coin.rectTransform.localRotation;
        targetPos = new Vector3(-75, 0, 0);
        targetRot = Quaternion.Euler(Vector3.zero);

        while (timeElapsed < duration)
        {
            coin.rectTransform.localPosition = Vector3.Lerp(startPos, targetPos, timeElapsed / duration);
            coin.rectTransform.localRotation = Quaternion.Lerp(startRot, targetRot, timeElapsed / duration);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        coin.rectTransform.localPosition = targetPos;
        coin.rectTransform.localRotation = targetRot;

        Destroy(coin.gameObject);

        objCoinAnim.SetTrigger("isMove");
        --charCap;
        gameEconomic.SellBlock(0);

        yield return new WaitForSeconds(0.25f);
        isSelling = false;
    }
}
