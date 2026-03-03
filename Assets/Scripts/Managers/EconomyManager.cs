using GameData;
using GameData.SavesData;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EconomyManager : MonoBehaviourSingleton<EconomyManager>
{
    public event Action OnDeath;
    public event Action OnEconomyChanged;

    public GameConfig Config { get; private set; }

    private Dictionary<string, ItemDefinition> _itemLookup = new();

    private float _lastClickTime = -999f;
    private int _comboLevel = 1;

    private bool _isDead;


    private float _saveTimer;
    private const float SaveInterval = 5f;



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
            float multiplieLevels = survivalTime / Config.drainMultiplieInterval;
            float multiplieAmount = multiplieLevels > 1 ? multiplieLevels * Config.drainMultiplieAmount : 1;
            return (Config.baseDrainRate + levels * Config.drainIncreaseAmount) * multiplieAmount;
        }
    }

    public int CurrentComboLevel => _comboLevel;


    public void Init(GameConfig config)
    {
        Config = config;

        _itemLookup.Clear();
        foreach (var item in config.shopItems)
            _itemLookup[item.itemId] = item;

        if (GameSaves.Data == null)
            GameSaves.LoadAndCheck();

        if (GameSaves.Data.currentCoins <= 0 && !_isDead)
            GameSaves.ChangeAndSave(d => d.currentCoins = config.startingCoins);

        _isDead = false;
    }


    protected void Update()
    {
        if (_isDead || Config == null || GameSaves.Data == null) return;

        float dt = Time.deltaTime;
        var data = GameSaves.Data;

        data.currentRunSurvivalTime += dt;

        TickActiveEffects(dt, data);


        float income = CoinsPerSec * Multiplier * dt;
        data.currentCoins += income;
        data.totalCoinsEarned += income;


        if (!ShieldActive)
            data.currentCoins -= CurrentDrain * dt;

        data.currentCoins = Mathf.Max(data.currentCoins, 0f);
        data.peakCoins = Mathf.Max(data.peakCoins, data.currentCoins);

        if (data.currentCoins <= 0f)
        {
            TriggerDeath(data);
            return;
        }

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
        _ = anyExpired;
    }

    // ── Public API ────────────────────────────────────────────────────────────

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

    public IEnumerable<ActiveEffect> ActiveEffects()
    {
        foreach (var e in GameSaves.Data.activeEffects)
            if (e.remainingTime > 0f)
                yield return e;
    }

    public IEnumerable<ActiveEffect> ActiveEffectsFor(string itemId)
    {
        foreach (var e in ActiveEffects())
            if (e.itemId == itemId)
                yield return e;
    }


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
