using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Model for the right-hand detail panel in InventoryScreen
public class InventoryDetailModel
{
    public ItemDefinition Definition;  // null = nothing selected
    public int OwnedQuantity;
    public InventoryScreen ParentScreen;
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
    [SerializeField] private TextMeshProUGUI timerEntryPrefab; // simple label prefab

    public UIStatesBool emptyState;

    // Injected by InventoryScreen so USE triggers a full refresh
    public System.Action OnUsed;

    private readonly List<TextMeshProUGUI> _timerLabels = new();

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

        RefreshTimers(def);
    }

    private void RefreshTimers(ItemDefinition def)
    {
        if (timersContainer == null) return;

        // Collect active timers for this item
        var activeEffects = new List<ActiveEffect>();
        foreach (var e in EconomyManager.Instance.ActiveEffectsFor(def.itemId))
            activeEffects.Add(e);

        // Grow/shrink labels pool
        while (_timerLabels.Count < activeEffects.Count)
        {
            var label = Instantiate(timerEntryPrefab, timersContainer);
            _timerLabels.Add(label);
        }
        for (int i = 0; i < _timerLabels.Count; i++)
        {
            bool visible = i < activeEffects.Count;
            _timerLabels[i].gameObject.SetActive(visible);
            if (visible)
                _timerLabels[i].text = $"Active: {activeEffects[i].remainingTime:0.0}s remaining";
        }
    }

    // Wired via Button OnClick in the Inspector
    public void OnUseClick()
    {
        if (Model?.Definition == null) return;
        EconomyManager.Instance.UseItem(Model.Definition);
        Model?.ParentScreen.Refresh();
    }
}
