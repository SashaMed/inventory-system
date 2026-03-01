using System;
using System.Collections.Generic;
using GameData.SavesData;
using UnityEngine;

namespace GameData
{
    public class GameSaves : DataSaves<GameSavesData>
    {
        public static void LoadAndCheck()
        {
            Load();

            // BinaryFormatter doesn't call constructors on deserialization, so
            // reference-type fields from old saves arrive as null. Reset() is the
            // fallback for a completely missing/corrupt save file.
            if (Data == null) Reset();

            Data.ownedItems    ??= new();
            Data.activeEffects ??= new();
        }

        public static void ChangeAndSave(Action<GameSavesData> action)
        {
            action?.Invoke(Data);
            Save();
        }
    }

    namespace SavesData
    {
        [Serializable]
        public class GameSavesData
        {
            public GameSavesData()
            {
                currentRunSurvivalTime = 1;
            }

            public float currentCoins;
            public float totalCoinsEarned;

            // Items the player has bought but not yet used
            public Dictionary<string, int> ownedItems = new();

            // Currently running temporary effects
            public List<ActiveEffect> activeEffects = new();

            // Run statistics
            public float currentRunSurvivalTime;
            public float bestSurvivalTime;
            public float peakCoins;
        }
    }
}
