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

    // Refresh when returning from a deeper screen (coin balance may have changed)
    public override void OnResume()
    {
        coinsView?.Refresh();
        shopList.Refresh(); // re-check afford state for each item
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

        // Inject purchase callback into each view so BUY triggers a refresh
        foreach (var view in shopList.Views)
            view.OnPurchased = Refresh;
    }

    public void OnBackClick()
    {
        Close();
    }
}
