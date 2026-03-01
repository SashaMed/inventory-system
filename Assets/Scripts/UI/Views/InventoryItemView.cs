using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemModel
{
    public ItemDefinition Definition;
    public int OwnedQuantity;
}

public class InventoryItemView : UIView<InventoryItemModel>
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Image selectionHighlight;

    public System.Action<InventoryItemModel> OnSelected;

    private bool _isSelected;

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        if (selectionHighlight) selectionHighlight.enabled = selected;
    }

    public override void Refresh()
    {
        var def = Model.Definition;
        if (icon)         icon.sprite    = def.icon;
        if (nameText)     nameText.text  = def.itemName;
        if (quantityText) quantityText.text = $"x{Model.OwnedQuantity}";
    }

    public void OnItemClick()
    {
        OnSelected?.Invoke(Model);
    }
}
