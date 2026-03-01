using TMPro;
using UnityEngine;

public class DeathModel
{
    public float survivalTime;
    public float peakCoins;
    public float totalCoinsEarned;
}

public class DeathScreen : UIScreen<DeathModel>
{
    [SerializeField] private TextMeshProUGUI survivalTimeText;
    [SerializeField] private TextMeshProUGUI peakCoinsText;
    [SerializeField] private TextMeshProUGUI totalEarnedText;

    public override void Refresh()
    {
        if (Model == null) return;

        if (survivalTimeText)
            survivalTimeText.text = $"Survived: {FormatTime(Model.survivalTime)}";

        if (peakCoinsText)
            peakCoinsText.text = $"Peak coins: {CoinsView.FormatCoins(Model.peakCoins)}";

        if (totalEarnedText)
            totalEarnedText.text = $"Total earned: {CoinsView.FormatCoins(Model.totalCoinsEarned)}";
    }

    // Wired via Button OnClick in the Inspector
    public void OnTryAgainClick()
    {
        EconomyManager.Instance.ResetGame();
        SimpleNavigation.Instance?.PopScreensTo<MainMenuScreen>();
    }

    private static string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        return m > 0 ? $"{m}m {s:00}s" : $"{s}s";
    }
}
