using GameData;
using TMPro;
using UnityEngine;

public class CoinsView : UIView<EmptyModel>
{
    [SerializeField] private TextMeshProUGUI coinsText;

    public override void Refresh()
    {
        if (coinsText != null)
            coinsText.text = FormatCoins(GameSaves.Data.currentCoins);
    }

    public static string FormatCoins(float value)
    {
        if (value >= 1_000_000f) return $"{value / 1_000_000f:0.#}M";
        if (value >= 1_000f)     return $"{value / 1_000f:0.#}K";
        return $"{Mathf.FloorToInt(value)}";
    }
}
