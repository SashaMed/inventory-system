using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScreen : UIScreen<EmptyModel>
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
