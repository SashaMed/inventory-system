using GameData;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;



public class BasicLayer : MonoBehaviourSingleton<BasicLayer>
{

#if UNITY_EDITOR

    private async void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            Time.timeScale = Time.timeScale > 1f ? 0.1f : Time.timeScale < 1f ? 1f : 20f;
            Debug.Log($"<b><color=white>Time scale: {Time.timeScale} </color></b>");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            var paused = EditorApplication.isPaused = !EditorApplication.isPaused;
            Debug.Log($"<b><color=white>PAUSED</color></b> (To Unpause:  <b>⇧ ⌘ P</b>)");
        }


        if (Input.GetKeyDown(KeyCode.O))
        {
            GameSaves.ChangeAndSave(data =>
            {
                data.currentCoins += 20000;
            });
            Debug.Log($"<b><color=white>data.currentCoins = 20000</color></b> cool </b>)");
        }


    }


#endif

}
