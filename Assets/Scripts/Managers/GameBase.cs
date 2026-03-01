using GameData;
using System;
using UnityEngine;

public class GameBase : MonoBehaviourSingleton<GameBase>
{

    protected override void Awake()
    {
        base.Awake();
        GameSaves.LoadAndCheck();
    }


    private void Start()
    {
        SimpleNavigation.Instance?.Push<MainMenuScreen, EmptyModel>(new EmptyModel());
    }

}