using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PopupScreen : PopupScreenBase<PopupContent>
{

}

public abstract class PopupScreenSimple<T> : BasicScreen<T>
{
    private const float TIME_TO_ATTENTION = 1.0f;
    private const string TRIGGER_SUSPEND = "Suspend";
    protected float allowedOverlapCloseTime;

    [Header("Buttons")]
    [SerializeField] private Button[] closeButtons;
    [SerializeField] private Button overlapButton;

    public bool DisableSuspendAnim { get; set; } = false;

    protected override float AppearAnimationLen() => 35f / 60f;

    public override void InitialPrepare()
    {
        closeButtons.ForEach(b => b.onClick.AddListener(OnCloseButton));
        overlapButton?.onClick.AddListener(OnOverlapCloseButton);
    }

    public override void Prepare()
    {
        allowedOverlapCloseTime = Time.time + TIME_TO_ATTENTION;
    }

    protected void OnOverlapCloseButton()
    {
        // Ignore all clicks on some time on start popup
        // for exclude missclicks on background
        if (Time.time < allowedOverlapCloseTime)
        {
            return;
        }

        OnCloseButton();
    }

    public virtual void OnCloseButton()
    {
        //CoreBasicLayer.PlayClick();
        Close();
    }

    public override void OnAndroidBack()
    {
        if (Time.time < allowedOverlapCloseTime || Lock)
        {
            return;
        }

        OnCloseButton();
    }

    #region UISCREEN

    public override void OnSuspend()
    {
        base.OnSuspend();
        if (DisableSuspendAnim == false)
        {
            animator?.SetBool(TRIGGER_SUSPEND, true);
        }
    }

    public override void OnResume()
    {
        base.OnResume();
        animator?.SetBool(TRIGGER_SUSPEND, false);
        DisableSuspendAnim = false;
    }

    #endregion
}

public class PopupScreenBase<T> : PopupScreenSimple<T> where T : PopupContent
{

    [Header("Texts")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;

    [Header(" ")]
    public TextMeshProUGUI yesText;
    public TextMeshProUGUI noText;
    public TextMeshProUGUI okText;


    [Header("Answers")]
    public Button okButton;
    public Button cancelButton;
    [FormerlySerializedAs("confignButton")]
    public Button confirmButton;

    public UIStatesBool negativeStates;

    //[Header("Audio")]
    //public AudioHitInfo audioOpen;

    public override void InitialPrepare()
    {
        base.InitialPrepare();

        okButton?.onClick.AddListener(OnCloseButton);
        cancelButton?.onClick.AddListener(OnCloseButton);
        confirmButton?.onClick.AddListener(OnConfirmButton);
    }

    public override void Prepare()
    {
        // Text configuration

        if (titleText != false)
        {
            titleText.text = Model.title ?? "Popup/Message";
        }

        if (messageText != false)
        {
            messageText.text = Model.message;
        }

        // (Button texts)

        if (yesText != false)
        {
            yesText.text = Model.yesTitle ?? "Confirm";
        }

        if (noText != false)
        {
            noText.text = Model.noTitle ?? "No";
        }

        if (okText != false)
        {
            okText.text = Model.okTitle ?? "Ok";
        }

        // enabling

        okButton?.gameObject.SetActive(!Model.IsConfigmedState);
        cancelButton?.gameObject.SetActive(Model.IsConfigmedState);
        confirmButton?.gameObject.SetActive(Model.IsConfigmedState);

        negativeStates.SetState(Model.isNegativeVariant);
        base.Prepare();
    }

    public override void Refresh() { }


    public override void OnCloseButton()
    {
        if (Model.IsConfigmedState)
        {
            Model.onCancel?.Invoke();
        }

        base.OnCloseButton();
    }

    public virtual void OnConfirmButton()
    {
        Model.onConfirm?.Invoke();

        Close();
    }

    public override void OnClose()
    {
        base.OnClose();
        Model.onFinish?.Invoke();
    }

    public override void OnPageLoaded(bool successful)
    {
        base.OnPageLoaded(successful);
        //audioOpen.Play();
    }

    public virtual void OnRemoveToPool()
    {

    }
}

public class PopupContent
{
    public string title;
    public string message;

    public virtual bool IsConfigmedState => onConfirm != null || onCancel != null;

    public Action onFinish;
    public Action onConfirm;
    public Action onCancel;

    public string okTitle;
    public string yesTitle;
    public string noTitle;

    public bool isNegativeVariant;

    public PopupContent(string message, string title = null)
    {
        this.message = message;
        this.title = title;
    }

    public PopupContent SetOnConfirm(Action onConfirm, Action onCancel = null)
    {
        this.onConfirm = onConfirm;
        this.onCancel = onCancel;

        return this;
    }

    public PopupContent SetOnFinish(Action onFinish)
    {
        this.onFinish = onFinish;

        return this;
    }
}