using System;
using System.Collections.Generic;
using System.Linq;
using GameData.SavesData;
using UnityEngine;

namespace GameData
{
    public class GameSaves : DataSaves<GameSavesData>
    {
        public static void LoadAndCheck()
        {
            Load();
          
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
           
            public int currentCoins;
            public int totalEverEarnedCoins;

            public GameSavesData()
            {
            }

        }

    }
}
