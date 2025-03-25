using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectStageScene : BaseScene
{
    UI_SelectStageScene _selectStageSceneUI;

    protected override void Awake()
    {
        base.Awake();

        SceneType = Define.EScene.SelectStageScene;

        if(Managers.Game.LastStoryID == -1)
        {
            UI_StoryPopup storyPopup = Managers.UI.ShowPopupUI<UI_StoryPopup>($"UI_StoryPopup0");
            Managers.Game.LastStoryID = 0;
            Managers.Game.SaveGame();

            storyPopup.SetInfo(LoadUISelectStageScene);
        }
        else
        {
            LoadUISelectStageScene();
        }

        Managers.Sound.Clear();
        Managers.Sound.Play(Define.ESound.Bgm, "Sound_OnStage");
    }

    public void LoadUISelectStageScene()
    {
        _selectStageSceneUI = Managers.UI.ShowSceneUI<UI_SelectStageScene>();
        _selectStageSceneUI.SetInfo(Managers.Game.HighestChapter);
    }

    public override void Clear()
    {

    }
}
