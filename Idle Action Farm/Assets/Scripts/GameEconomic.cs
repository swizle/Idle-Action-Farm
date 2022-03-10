using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameEconomic : MonoBehaviour
{
    private float playerMoney = 0;
    private int[] blockTypes = new int[] {15};

    private UIUpdate uiUpdate;

    private void Start()
    {
        uiUpdate = GameObject.FindGameObjectWithTag("UIUpdater").GetComponent<UIUpdate>();
    }

    public void SellBlock(int blockType)
    {
        playerMoney += blockTypes[blockType];
        uiUpdate.UpdateUI();
    }

    public float GetMoney()
    {
        return playerMoney;
    }
}
