using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemModel
{
    public InventoryScreen ParentScreen;
    public ItemDefinition Definition;
    public int OwnedQuantity;
}

public class InventoryItemView : UIView<InventoryItemModel>
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Image selectionHighlight;

    public UIStatesBool selected;

    public void SetSelected(bool selected)
    {
        this.selected.SetState(selected);
    }

    public override void Refresh()
    {
        var def = Model.Definition;
        if (icon)         icon.sprite    = def.icon;
        if (nameText)     nameText.text  = def.itemName;
        if (quantityText) quantityText.text = $"x{Model.OwnedQuantity}";

        this.selected.SetState(false);

    }

    public void OnItemClick()
    {
        Model?.ParentScreen?.SelectItem(Model);
    }
}
