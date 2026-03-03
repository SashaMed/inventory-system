using GameData;
using TMPro;
using UnityEngine;

public class ShopScreen : UIScreen<EmptyModel>
{
    [SerializeField] private CoinsView coinsView;
    [SerializeField] private UIList<ShopItemView, ShopItemModel> shopList;

    public override void OnPageLoaded(bool successful)
    {
        PopulateList();
    }

    public override void OnResume()
    {
        coinsView?.Refresh();
        shopList.Refresh(); 
    }

    public override void Refresh()
    {
        coinsView?.Refresh();
        PopulateList();
    }

    private void PopulateList()
    {
        var items = EconomyManager.Instance.Config.shopItems;
        var models = new ShopItemModel[items.Length];

        for (int i = 0; i < items.Length; i++)
            models[i] = new ShopItemModel { Definition = items[i] };

        shopList.SetModeles(models);

        foreach (var view in shopList.Views)
            view.OnPurchased = Refresh;
    }

    public void OnBackClick()
    {
        Close();
    }
}
