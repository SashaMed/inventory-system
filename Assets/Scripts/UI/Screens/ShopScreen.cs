using GameData;
using TMPro;
using UnityEngine;


public class ShopScreen : UIScreen<EmptyModel>
{
    [SerializeField] private CoinsView coinsView;


    public override void Refresh()
    {
        if (coinsView != null)
        {
            coinsView.Refresh();
        }
    }

}
