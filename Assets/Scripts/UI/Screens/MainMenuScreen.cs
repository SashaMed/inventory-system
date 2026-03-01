using GameData;
using TMPro;
using UnityEngine;

public class MainMenuScreen : UIScreen<EmptyModel>
{
    [SerializeField] private CoinsView coinsView;
    [SerializeField] private StatsView statsView;

    private EconomyManager eco => EconomyManager.Instance;

    public override void Refresh()
    {
        coinsView?.Refresh();
        statsView?.Refresh();
    }

    // Called when this screen becomes the top screen again (e.g. back from Shop)
    public override void OnResume()
    {
        if (eco == null)
        {
            return;
        }
        eco.OnEconomyChanged += Refresh;
    }

    public override void OnSuspend()
    {
        if (eco == null)
        {
            return;
        }
        eco.OnEconomyChanged -= Refresh;
    }

    // Also subscribe when first shown
    public override void OnPageLoaded(bool successful)
    {
        if (eco == null)
        {
            return;
        }
        eco.OnEconomyChanged += Refresh;
    }

    // ── Button handlers ───────────────────────────────────────────────────────

    public void OnIdleClick()
    {
        if (eco == null)
        {
            return;
        }
        int combo = eco.Click();
        Refresh();
    }

    public void OnInventoryClick()
    {
        SimpleNavigation.Instance?.Push<InventoryScreen, EmptyModel>(new EmptyModel());
    }

    public void OnShopClick()
    {
        SimpleNavigation.Instance?.Push<ShopScreen, EmptyModel>(new EmptyModel());
    }
}
