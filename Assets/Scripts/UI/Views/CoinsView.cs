using GameData;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinsView : UIView<EmptyModel>
{
    [SerializeField] private TextMeshProUGUI coinsText;


    public override void Refresh()
    {
        if (coinsText != null)
        {
            coinsText.text = GameSaves.Data.currentCoins.ToString();
        }
    }


}
