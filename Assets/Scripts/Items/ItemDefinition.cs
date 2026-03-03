using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Shop/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [Tooltip("Unique key used in save data — never change after release")]
    public string itemId;

    public string itemName;
    public Sprite icon;
    [TextArea] public string description;

    public ItemType itemType;
    public int basePrice;

    [Tooltip("Meaning depends on type: coins/click, coins/sec, or income multiplier")]
    public float effectValue;

    [Tooltip("How long the effect lasts in seconds after being used")]
    public float effectDuration = 30f;

    public string GetEffectDescription()
    {
        return itemType switch
        {
            ItemType.ClickUpgrade => $"+{effectValue} coins/click for {effectDuration}s",
            ItemType.Generator    => $"+{effectValue} coins/sec for {effectDuration}s",
            ItemType.Multiplier   => $"x{effectValue} all income for {effectDuration}s",
            ItemType.Shield       => $"Drain paused for {effectDuration}s",
            _                     => string.Empty
        };
    }
}

public enum ItemType
{
    ClickUpgrade,
    Generator,
    Multiplier,
    Shield
}