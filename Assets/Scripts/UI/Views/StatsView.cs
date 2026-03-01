using TMPro;
using UnityEngine;


public class StatsView : UIView<EmptyModel>
{
    [SerializeField] private TextMeshProUGUI coinsPerClickText;
    [SerializeField] private TextMeshProUGUI coinsPerSecText;
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private TextMeshProUGUI drainText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI activeEffectsText;

    public override void Refresh()
    {
        var eco = EconomyManager.Instance;
        if (eco == null) return;

        if (coinsPerClickText)
            coinsPerClickText.text = $"+{eco.CoinsPerClick * eco.Multiplier:0.#} / click";

        if (coinsPerSecText)
            coinsPerSecText.text = $"+{eco.CoinsPerSec * eco.Multiplier:0.##} / sec";

        if (multiplierText)
            multiplierText.text = $"x{eco.Multiplier:0.##}";

        if (drainText)
        {
            string shield = eco.ShieldActive ? " [SHIELDED]" : string.Empty;
            drainText.text = $"-{eco.CurrentDrain:0.##} / sec{shield}";
        }

        if (comboText)
        {
            int combo = eco.CurrentComboLevel;
            float mult = eco.Config.comboMultipliers[combo - 1];
            comboText.text = combo > 1 ? $"COMBO x{mult}" : string.Empty;
        }

        if (activeEffectsText)
        {
            int count = 0;
            foreach (var _ in eco.ActiveEffects()) count++;
            activeEffectsText.text = count > 0 ? $"{count} effect(s) active" : string.Empty;
        }
    }
}
