using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    private CharController charController;

    private void Start()
    {
        charController = GameObject.FindGameObjectWithTag("Character").GetComponent<CharController>();
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.CompareTag("Character") && !gameObject.GetComponent<Rigidbody>())
        {
            if (charController.GetCharCap() < charController.GetCharMaxCap())
            {
                StartCoroutine(MoveBlocksToInventory(hit.transform.GetChild(1).GetChild(2).GetChild(0).gameObject, (int)charController.GetCharCap()));
            }
        }
    }

    private IEnumerator MoveBlocksToInventory(GameObject player, int capacity)
    {
        charController.CollectBlock(gameObject);

        float timeElapsed = 0f;
        float duration = 0.5f;

        transform.parent = player.transform;

        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;
        Vector3 targetPos;
        Quaternion targetRot = Quaternion.Euler(Vector3.zero);

        int floor = capacity / 2;

        if (capacity % 2 == 0)
        {
            targetPos = new Vector3(-0.125f, 0.125f * floor, -0.0625f);
        }
        else
        {
            targetPos = new Vector3(0.125f, 0.125f * floor, -0.0625f);
        }

        while (timeElapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(startPos, targetPos, timeElapsed / duration);
            transform.localRotation = Quaternion.Lerp(startRot, targetRot, timeElapsed / duration);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        if (capacity % 2 == 0)
        {
            transform.localPosition = new Vector3(0.125f, 0.125f * floor, -0.0625f);
        }
        else
        {
            transform.localPosition = new Vector3(-0.125f, 0.125f * floor, -0.0625f);
        }

        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
}
