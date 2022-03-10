using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdate : MonoBehaviour
{
    [SerializeField] private Image imgCharCap;
    [SerializeField] TextMeshProUGUI textCharCap;
    [SerializeField] TextMeshProUGUI textMoney;

    private CharController charController;
    private GameEconomic gameEconomic;

    void Start()
    {
        charController = GameObject.FindGameObjectWithTag("Character").GetComponent<CharController>();
        gameEconomic = GameObject.FindGameObjectWithTag("EconomicManager").GetComponent<GameEconomic>();
    }

    public void UpdateUI()
    {
        StartCoroutine(UpdateMoneyText());
        imgCharCap.fillAmount = charController.GetCharCap() / charController.GetCharMaxCap();
        textCharCap.text = charController.GetCharCap() + "\n/\n" + charController.GetCharMaxCap();
    }

    private IEnumerator UpdateMoneyText()
    {
        float timeElapsed = 0f;
        float duration = 1f;
        float money = float.Parse(textMoney.text);

        while (timeElapsed < duration)
        {
            textMoney.text = Mathf.Round(Mathf.Lerp(money, gameEconomic.GetMoney(), timeElapsed / duration)).ToString();
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        textMoney.text = gameEconomic.GetMoney().ToString();
    }
}
