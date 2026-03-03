using System.Collections.Generic;
using System.Linq;
using GameData;
using UnityEngine;

public class InventoryScreen : UIScreen<EmptyModel>
{
    [SerializeField] private CoinsView coinsView;
    [SerializeField] private StatsView statsView;

    [SerializeField] private UIList<InventoryItemView, InventoryItemModel> itemList;
    [SerializeField] private InventoryDetailView detailView;

    private InventoryItemModel _selectedModel;

    public override void Refresh()
    {
        coinsView?.Refresh();
        statsView?.Refresh();
        RebuildList();
        RefreshDetail();
    }

    public void SelectItem(InventoryItemModel model)
    {
        _selectedModel = model;

        foreach (var view in itemList.Views)
            view.SetSelected(view.Model == model);

        RefreshDetail();
    }



    private void RebuildList()
    {
        var models = new List<InventoryItemModel>();
        var ownedItems = GameSaves.Data.ownedItems;

        foreach (var item in EconomyManager.Instance.Config.shopItems)
        {
            ownedItems.TryGetValue(item.itemId, out int qty);
            if (qty > 0)
            {
                models.Add(new InventoryItemModel
                {
                    Definition = item,
                    OwnedQuantity = qty,
                    ParentScreen = this
                });
            }
        }

        itemList.SetModeles(models);

        _selectedModel = itemList.Models.FirstOrDefault();

    }

    private void RefreshDetail()
    {
        if (detailView == null) return;

        if (_selectedModel == null)
        {
            detailView.SetModel(new InventoryDetailModel { Definition = null });
            return;
        }

        // Rebuild model with fresh quantity from saves
        GameSaves.Data.ownedItems.TryGetValue(_selectedModel.Definition.itemId, out int qty);
        detailView.SetModel(new InventoryDetailModel
        {
            Definition    = _selectedModel.Definition,
            OwnedQuantity = qty,
            ParentScreen = this
        });
    }
}
