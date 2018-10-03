using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    public Text playerName;
    public Text playerHp;

    public void SetHp(int value, int max = 100)
    {
        playerHp.text = $"HP: {value}/{max}";
        if (value <= 0)
        {
            playerHp.color = Color.black;
        }
    }
}
