using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemModel
{
    public ItemDefinition Definition;
}

public class ShopItemView : UIView<ShopItemModel>
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button buyButton;

    public System.Action OnPurchased;

    public override void Refresh()
    {
        var def = Model.Definition;
        if (icon)        icon.sprite  = def.icon;
        if (nameText)    nameText.text = def.itemName;
        if (descriptionText) descriptionText.text = def.description;
        if (effectText)  effectText.text = def.GetEffectDescription();
        if (priceText)   priceText.text  = $"{def.basePrice} coins";

        bool canAfford = GameData.GameSaves.Data.currentCoins >= def.basePrice;
        if (buyButton) buyButton.interactable = canAfford;
    }

    public void OnBuyClick()
    {
        EconomyManager.Instance.BuyItem(Model.Definition);
        OnPurchased?.Invoke();
    }
}
