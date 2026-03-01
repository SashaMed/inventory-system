using GameData;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;



public class BasicLayer : CoreBasicLayer
{
    const float _Dur = 0.3f;

    public new static BasicLayer Instance => CoreBasicLayer.Instance as BasicLayer;

    [SerializeField] private CanvasGroup _fader;



    //public void GoToMainMenuScreen()
    //{
    //    var nav = SimpleNavigation.Instance;

    //    if (nav.navigationStack
    //        .Any(s => s.GetType() == typeof(GameScreen)))
    //    {
    //        _ = nav.PopScreensTo<GameScreen>();
    //    }

    //}

    public void FadeWithAction(Action doThis, float duration = -1, bool waitScreenLoaded = false)
    {
        //duration = duration < 0 ? _Dur : duration;
        //if (_fader && doThis != null)
        //{

        //    _fader.DOKill(false);
        //    _fader.gameObject.SetActive(true);
        //    var seq = DOTween.Sequence(_fader).SetUpdate(true)
        //        .Append(_fader.DOFade(1, duration).ChangeStartValue(0))
        //        .AppendCallback(doThis.Invoke);

        //    if (waitScreenLoaded)
        //    {
        //        IBasicScr.OnPageAppeared += UnfadeWhenLoaded;
        //    }
        //    else
        //    {
        //        seq.Append(_fader.DOFade(0, duration))
        //            .OnComplete(() => _fader.gameObject.SetActive(false));
        //    }
        //}
        //else
        //{
        //    doThis?.Invoke();
        //}
    }

    private void UnfadeWhenLoaded(IBasicScr scr)
    {
        //if (scr.IsAppearing == false)
        //{
        //    IBasicScr.OnPageAppeared -= UnfadeWhenLoaded;

        //    _fader.DOKill(false);
        //    _fader.gameObject.SetActive(true);
        //    DOTween.Sequence(_fader).SetUpdate(true)
        //        .Append(_fader.DOFade(0, _Dur))
        //        .OnComplete(() => _fader.gameObject.SetActive(false));
        //}
    }





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
