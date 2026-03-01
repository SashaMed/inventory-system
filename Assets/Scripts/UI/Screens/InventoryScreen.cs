using System.Collections.Generic;
using GameData;
using UnityEngine;

public class InventoryScreen : UIScreen<EmptyModel>
{
    [SerializeField] private CoinsView coinsView;
    [SerializeField] private StatsView statsView;

    [Header("Two-panel Layout")]
    [SerializeField] private UIList<InventoryItemView, InventoryItemModel> itemList;
    [SerializeField] private InventoryDetailView detailView;

    private InventoryItemModel _selectedModel;

    public override void OnPageLoaded(bool successful)
    {
        if (detailView != null)
            detailView.OnUsed = RefreshAll;

        RefreshAll();
    }

    public override void OnResume()
    {
        RefreshAll();
    }

    public override void Refresh()
    {
        RefreshAll();
    }

    public void RefreshAll()
    {
        coinsView?.Refresh();
        statsView?.Refresh();
        RebuildList();
        RefreshDetail();
    }

    public void SelectItem(InventoryItemModel model)
    {
        _selectedModel = model;

        // Update highlight on all views
        foreach (var view in itemList.Views)
            view.SetSelected(view.Model == model);

        RefreshDetail();
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void RebuildList()
    {
        var models = new List<InventoryItemModel>();
        var ownedItems = GameSaves.Data.ownedItems;

        foreach (var item in EconomyManager.Instance.Config.shopItems)
        {
            ownedItems.TryGetValue(item.itemId, out int qty);
            if (qty > 0)
                models.Add(new InventoryItemModel { Definition = item, OwnedQuantity = qty });
        }

        itemList.SetModeles(models);

        // Inject selection callback and restore highlight for current selection
        foreach (var view in itemList.Views)
        {
            view.OnSelected = SelectItem;
            view.SetSelected(view.Model == _selectedModel);
        }

        // If selected item no longer in list (qty dropped to 0), clear selection
        if (_selectedModel != null && !models.Contains(_selectedModel))
            _selectedModel = null;
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
            OwnedQuantity = qty
        });
    }

    public void OnBackClick()
    {
        Close();
    }
}
