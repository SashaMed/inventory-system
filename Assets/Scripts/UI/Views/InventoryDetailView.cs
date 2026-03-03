using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDetailModel
{
    public ItemDefinition Definition; 
    public InventoryScreen ParentScreen;
    public int OwnedQuantity;
}

public class InventoryDetailView : UIView<InventoryDetailModel>
{

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Button useButton;

    [Header("Active Timers")]
    [SerializeField] private Transform timersContainer;
    [SerializeField] private TextMeshProUGUI timerEntryPrefab; 

    public UIStatesBool emptyState;

    public override void Refresh()
    {
        bool hasSelection = Model?.Definition != null;
        emptyState.SetState(!hasSelection);

        if (!hasSelection) return;

        var def = Model.Definition;
        if (icon)            icon.sprite          = def.icon;
        if (nameText)        nameText.text        = def.itemName;
        if (descriptionText) descriptionText.text = def.description;
        if (effectText)      effectText.text      = def.GetEffectDescription();
        if (quantityText)    quantityText.text    = $"In bag: {Model.OwnedQuantity}";

        if (useButton) useButton.interactable = Model.OwnedQuantity > 0;

    }

    public void OnUseClick()
    {
        if (Model?.Definition == null) return;
        EconomyManager.Instance.UseItem(Model.Definition);
        Model?.ParentScreen.Refresh();
    }
}
