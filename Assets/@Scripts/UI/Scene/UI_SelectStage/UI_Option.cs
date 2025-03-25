using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Option : UI_Popup
{
    enum GameObjects
    {
        BGMOnImage,
        BGMOffImage,
        EffectSoundOnImage,
        EffectSoundOffImage,
        Block,
    }

    enum Buttons
    {
        BGMButton,
        EffectSoundButton,
        CloseButton,
    }

    protected override void Awake()
    {
        base.Awake();

        Managers.Sound.Play(Define.ESound.Effect, "Sound_ButtonMain");

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));

        GetButton((int)Buttons.BGMButton).gameObject.BindEvent(OnClickBGMButton);
        GetButton((int)Buttons.EffectSoundButton).gameObject.BindEvent(OnClickEffectSoundButton);

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        GetObject((int)GameObjects.Block).BindEvent(OnClickCloseButton);

        BGMOnOffImage(Managers.Game.BGMOn);
        EffectSoundOnOffImage(Managers.Game.EffectSoundOn);
    }

    public void SetInfo()
    {

    }

    public void RefreshUI()
    {
        BGMOnOffImage(Managers.Game.BGMOn);
        EffectSoundOnOffImage(Managers.Game.EffectSoundOn);
    }

    void BGMOnOffImage(bool bgmOn)
    {
        GetObject((int)GameObjects.BGMOnImage).SetActive(bgmOn);
        GetObject((int)GameObjects.BGMOffImage).SetActive(!bgmOn);
    }

    void EffectSoundOnOffImage(bool effectSoundOn)
    {
        GetObject((int)GameObjects.EffectSoundOnImage).SetActive(effectSoundOn);
        GetObject((int)GameObjects.EffectSoundOffImage).SetActive(!effectSoundOn);
    }

    #region EventHandler
    void OnClickBGMButton(PointerEventData evt)
    {
        bool bgmOn = Managers.Game.BGMOn;
        bgmOn = !bgmOn;
        Managers.Game.BGMOn = bgmOn;

        BGMOnOffImage(bgmOn);

        if (!bgmOn)
            Managers.Sound.Stop(Define.ESound.Bgm);
        else
            Managers.Sound.Play(Define.ESound.Bgm);
    }

    void OnClickEffectSoundButton(PointerEventData evt) 
    {
        bool effectSoundOn = Managers.Game.EffectSoundOn;
        effectSoundOn = !effectSoundOn;
        Managers.Game.EffectSoundOn = effectSoundOn;

        EffectSoundOnOffImage(effectSoundOn);
        if (!effectSoundOn)
            Managers.Sound.Stop(Define.ESound.SubBgm);
        else
            Managers.Sound.Play(Define.ESound.SubBgm);
    }

    void OnClickCloseButton(PointerEventData evt)
    {
        Managers.Sound.Play(Define.ESound.Effect, "Sound_HomeButton");
        Managers.UI.ClosePopupUI(this);
        Managers.Game.SaveGame();
    }
    #endregion
}
