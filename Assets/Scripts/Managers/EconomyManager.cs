using GameData;
using GameData.SavesData;
using System;
using System.Collections.Generic;
using UnityEngine;

// Central manager for all game economy: coins, drain, item effects, combo, and death.
public class EconomyManager : MonoBehaviourSingleton<EconomyManager>
{
    public event Action OnDeath;
    public event Action OnEconomyChanged;

    public GameConfig Config { get; private set; }

    // Cached item lookup built from GameConfig.shopItems
    private Dictionary<string, ItemDefinition> _itemLookup = new();

    // Combo tracking
    private float _lastClickTime = -999f;
    private int _comboLevel = 1;

    // Death guard — ensures OnDeath fires only once per run
    private bool _isDead;

    // Auto-save timer
    private float _saveTimer;
    private const float SaveInterval = 5f;

    // ── Computed economy stats ────────────────────────────────────────────────

    public float CoinsPerClick
    {
        get
        {
            float bonus = 0f;
            foreach (var effect in ActiveEffects())
                if (GetDef(effect.itemId) is { itemType: ItemType.ClickUpgrade } def)
                    bonus += def.effectValue;
            return 1f + bonus;
        }
    }

    public float CoinsPerSec
    {
        get
        {
            float total = 0f;
            foreach (var effect in ActiveEffects())
                if (GetDef(effect.itemId) is { itemType: ItemType.Generator } def)
                    total += def.effectValue;
            return total;
        }
    }

    public float Multiplier
    {
        get
        {
            float product = 1f;
            foreach (var effect in ActiveEffects())
                if (GetDef(effect.itemId) is { itemType: ItemType.Multiplier } def)
                    product *= def.effectValue;
            return product;
        }
    }

    public bool ShieldActive
    {
        get
        {
            foreach (var effect in ActiveEffects())
                if (GetDef(effect.itemId) is { itemType: ItemType.Shield })
                    return true;
            return false;
        }
    }

    public float CurrentDrain
    {
        get
        {
            float survivalTime = GameSaves.Data.currentRunSurvivalTime;
            float levels = Mathf.Floor(survivalTime / Config.drainIncreaseInterval);
            return Config.baseDrainRate + levels * Config.drainIncreaseAmount;
        }
    }

    public int CurrentComboLevel => _comboLevel;

    // ── Initialisation ────────────────────────────────────────────────────────

    public void Init(GameConfig config)
    {
        Config = config;

        _itemLookup.Clear();
        foreach (var item in config.shopItems)
            _itemLookup[item.itemId] = item;

        // Guarantee save data exists — LoadAndCheck must be called before Init,
        // but we defensively ensure it here in case execution order shifts.
        if (GameSaves.Data == null)
            GameSaves.LoadAndCheck();

        if (GameSaves.Data.currentCoins <= 0 && !_isDead)
            GameSaves.ChangeAndSave(d => d.currentCoins = config.startingCoins);

        _isDead = false;
    }

    // ── Unity Update ─────────────────────────────────────────────────────────

    protected void Update()
    {
        if (_isDead || Config == null || GameSaves.Data == null) return;

        float dt = Time.deltaTime;
        var data = GameSaves.Data;

        data.currentRunSurvivalTime += dt;

        TickActiveEffects(dt, data);

        // Passive income
        float income = CoinsPerSec * Multiplier * dt;
        data.currentCoins += income;
        data.totalCoinsEarned += income;

        // Drain (skipped when shield is active)
        if (!ShieldActive)
            data.currentCoins -= CurrentDrain * dt;

        data.currentCoins = Mathf.Max(data.currentCoins, 0f);
        data.peakCoins = Mathf.Max(data.peakCoins, data.currentCoins);

        if (data.currentCoins <= 0f)
        {
            TriggerDeath(data);
            return;
        }

        // Periodic save to avoid hammering disk every frame
        _saveTimer += dt;
        if (_saveTimer >= SaveInterval)
        {
            _saveTimer = 0f;
            GameSaves.ChangeAndSave(_ => { });
        }

        OnEconomyChanged?.Invoke();
    }

    private void TickActiveEffects(float dt, GameSavesData data)
    {
        bool anyExpired = false;
        for (int i = data.activeEffects.Count - 1; i >= 0; i--)
        {
            data.activeEffects[i].remainingTime -= dt;
            if (data.activeEffects[i].remainingTime <= 0f)
            {
                data.activeEffects.RemoveAt(i);
                anyExpired = true;
            }
        }
        // No extra action needed on expiry; computed properties re-evaluate each frame.
        _ = anyExpired;
    }

    // ── Public API ────────────────────────────────────────────────────────────

    // Returns the combo level used for this click (1-4), useful for UI feedback.
    public int Click()
    {
        float now = Time.time;
        if (now - _lastClickTime <= Config.comboWindowSeconds)
            _comboLevel = Mathf.Min(_comboLevel + 1, Config.maxCombo);
        else
            _comboLevel = 1;

        _lastClickTime = now;

        float comboMult = Config.comboMultipliers[_comboLevel - 1];
        float earned = CoinsPerClick * Multiplier * comboMult;

        var data = GameSaves.Data;
        data.currentCoins += earned;
        data.totalCoinsEarned += earned;
        data.peakCoins = Mathf.Max(data.peakCoins, data.currentCoins);

        OnEconomyChanged?.Invoke();
        return _comboLevel;
    }

    public void BuyItem(ItemDefinition item)
    {
        if (GameSaves.Data.currentCoins < item.basePrice) return;

        GameSaves.ChangeAndSave(d =>
        {
            d.currentCoins -= item.basePrice;
            d.ownedItems.TryGetValue(item.itemId, out int current);
            d.ownedItems[item.itemId] = current + 1;
        });

        OnEconomyChanged?.Invoke();
    }

    public void UseItem(ItemDefinition item)
    {
        if (!GameSaves.Data.ownedItems.TryGetValue(item.itemId, out int qty) || qty <= 0) return;

        GameSaves.ChangeAndSave(d =>
        {
            d.ownedItems[item.itemId] = qty - 1;
            d.activeEffects.Add(new ActiveEffect(item.itemId, item.effectDuration));
        });

        OnEconomyChanged?.Invoke();
    }

    public void ResetGame()
    {
        GameSaves.ChangeAndSave(d =>
        {
            if (d.currentRunSurvivalTime > d.bestSurvivalTime)
                d.bestSurvivalTime = d.currentRunSurvivalTime;

            d.currentCoins = Config.startingCoins;
            d.totalCoinsEarned = 0f;
            d.peakCoins = Config.startingCoins;
            d.currentRunSurvivalTime = 0f;
            d.ownedItems.Clear();
            d.activeEffects.Clear();
        });

        _comboLevel = 1;
        _lastClickTime = -999f;
        _isDead = false;
        _saveTimer = 0f;

        OnEconomyChanged?.Invoke();
    }

    public int GetOwnedCount(string itemId)
    {
        GameSaves.Data.ownedItems.TryGetValue(itemId, out int qty);
        return qty;
    }

    // Returns all active effects (remainingTime > 0) — safe read-only enumeration
    public IEnumerable<ActiveEffect> ActiveEffects()
    {
        foreach (var e in GameSaves.Data.activeEffects)
            if (e.remainingTime > 0f)
                yield return e;
    }

    // Returns active effects for a specific item
    public IEnumerable<ActiveEffect> ActiveEffectsFor(string itemId)
    {
        foreach (var e in ActiveEffects())
            if (e.itemId == itemId)
                yield return e;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private ItemDefinition GetDef(string itemId) =>
        _itemLookup.TryGetValue(itemId, out var def) ? def : null;

    private void TriggerDeath(GameSavesData data)
    {
        _isDead = true;

        GameSaves.ChangeAndSave(d =>
        {
            if (d.currentRunSurvivalTime > d.bestSurvivalTime)
                d.bestSurvivalTime = d.currentRunSurvivalTime;
        });

        OnDeath?.Invoke();
    }
}
