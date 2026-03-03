using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Config/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Starting State")]
    public float startingCoins = 50f;

    [Header("Drain")]
    [Tooltip("Coins drained per second at the start of a run")]
    public float baseDrainRate = 1f;

    [Tooltip("Additional coins/sec added to drain every interval")]
    public float drainIncreaseAmount = 0.5f;

    [Tooltip("Seconds between each drain increase")]
    public float drainIncreaseInterval = 30f;


    public float drainMultiplieAmount = 1.2f;

    [Tooltip("Seconds between each drain increase")]
    public float drainMultiplieInterval = 30f;


    [Header("Click Combo")]
    [Tooltip("Max seconds between clicks to maintain a combo")]
    public float comboWindowSeconds = 0.8f;

    public int maxCombo = 4;

    [Tooltip("Coin multiplier applied at each combo level (index 0 = combo 1)")]
    public float[] comboMultipliers = { 1f, 1.5f, 2f, 3f };

    [Header("Shop")]
    public ItemDefinition[] shopItems;
}
