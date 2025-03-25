using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

public class UI_StageBlock : UI_SubItem
{
    enum GameObjects
    {
        StageTileSpine,
        ClearFlag,
    }

    enum Texts
    {
        StageText,
    }

    enum Buttons
    {
        SelectStageButton,
    }
    enum Images
    {
        Shadow
    }
    int _stage;
    UI_SelectStageScene _selectStageSceneUI;

    protected override void Awake()
    {
        base.Awake();

        BindTexts(typeof(Texts));
        BindButtons(typeof(Buttons));
        BindObjects(typeof(GameObjects));
        BindImages(typeof(Images));

        GetButton((int)Buttons.SelectStageButton).gameObject.BindEvent(OnClickStageButton);
    }

    public void SetInfo(int stage, UI_SelectStageScene scene)
    {
        _stage = stage;
        _selectStageSceneUI = scene;

        RefreshUI();

        int selectStage = Managers.Game.HighestChapter > scene.SelectedChapter ? 20 : Managers.Game.HighestStage;
        //Debug.Log($"stage : {stage} \t selectStage : {selectStage}");
        if (stage == selectStage)
        {
            OnClickStageButton();
            _selectStageSceneUI.SetScrollPosition(selectStage);
        }
    }

    private void RefreshUI()
    {
        if (_init != true)
            return;

        GetText((int)Texts.StageText).text = _stage.ToString();

        if (Managers.Game.HighestStage == _stage)
        {
            GetObject((int)GameObjects.ClearFlag).SetActive(false);
        }

        Utils.GetOrAddComponent<Image>(GetButton((int)Buttons.SelectStageButton).gameObject).sprite = Managers.Resource.Load<Sprite>(_selectStageSceneUI.ChapterData.StageBlock);

        {
            SkeletonGraphic skeleton = Utils.GetOrAddComponent<SkeletonGraphic>(GetObject((int)GameObjects.StageTileSpine).gameObject);
            skeleton.skeletonDataAsset = Managers.Resource.Load<SkeletonDataAsset>(_selectStageSceneUI.ChapterData.StageTileSpine);
            skeleton.Initialize(true);
            skeleton.startingAnimation = "animation";
        }

        GetComponent<Image>().sprite = Managers.Resource.Load<Sprite>(_selectStageSceneUI.ChapterData.StageBlock);

        GetImage((int)Images.Shadow).sprite = Managers.Resource.Load<Sprite>(_selectStageSceneUI.ChapterData.Shadow);
        GetImage((int)Images.Shadow).gameObject.SetActive(IsReachableStage());
    }

    bool IsReachableStage()
    {
        if (_selectStageSceneUI.SelectedChapter < Managers.Game.HighestChapter)
            return true;

        //갈 수 없는 지역
        if (Managers.Game.HighestStage < _stage)
            return false;

        return true;
    }

    private void OnClickStageButton()
    {
        //Debug.Log("Stage Button Clicked!");
        //(Managers.Scene.CurrentScene as SelectStageScene)?.OnSelectStage(_stage);
        if (!IsReachableStage())
            return;

        _selectStageSceneUI?.OnSelectStage(_stage);
        SelectStage(true);
    }

    private void OnClickStageButton(PointerEventData evt)
    {
        OnClickStageButton();
    }

    public void SelectStage(bool selected)
    {
        if (_init != true)
            return;

        GetObject((int)Images.Shadow).gameObject.SetActive(!selected);
    }
}
