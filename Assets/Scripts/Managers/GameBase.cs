using GameData;
using UnityEngine;

public class GameBase : MonoBehaviourSingleton<GameBase>
{
    [SerializeField] private GameConfig config;

    protected override void Awake()
    {
        base.Awake();
        GameSaves.LoadAndCheck();
    }


    private void Start()
    {
        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.Init(config);
            EconomyManager.Instance.OnDeath += HandleDeath;
        }
        SimpleNavigation.Instance?.Push<MainMenuScreen, EmptyModel>(new EmptyModel());
    }

    private void HandleDeath()
    {
        var data = GameSaves.Data;
        var model = new DeathModel
        {
            survivalTime    = data.currentRunSurvivalTime,
            peakCoins       = data.peakCoins,
            totalCoinsEarned = data.totalCoinsEarned
        };
        SimpleNavigation.Instance?.Push<DeathScreen, DeathModel>(model);
    }

    private void OnDestroy()
    {
        if (EconomyManager.Instance != null)
            EconomyManager.Instance.OnDeath -= HandleDeath;
    }
}
