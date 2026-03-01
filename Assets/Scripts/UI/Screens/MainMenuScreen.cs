using GameData;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class MainMenuScreen : UIScreen<EmptyModel>
{

    [SerializeField] private CoinsView coinsView;


    public override void Refresh()
    {
        if (coinsView != null)
        {
            coinsView.Refresh();
        }
    }


    public void OnInventoryClick()
    {
        SimpleNavigation.Instance?.Push<InventoryScreen, EmptyModel>(new EmptyModel());

    }

    public void OnShopClick()
    {
        SimpleNavigation.Instance?.Push<ShopScreen, EmptyModel>(new EmptyModel());

    }



    public void OnIdleClick()
    {
        GameSaves.ChangeAndSave(d =>
        {
            d.currentCoins += 1;
        });
        Refresh();
    }
}
