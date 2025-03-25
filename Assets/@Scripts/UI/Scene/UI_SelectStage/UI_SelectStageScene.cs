using Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SelectStageScene : UI_Scene
{
    enum GameObjects
    {
        Player,
        Stand,
        SelectedChapter1,
        SelectedChapter2,
        SelectedChapter3,
        SelectedChapter4,
        SelectedChapter5,
        SelectedChapter6,
    }

    enum Buttons
    {
        ChapterButton_1,
        ChapterButton_2,
        ChapterButton_3,
        ChapterButton_4,
        ChapterButton_5,
        ChapterButton_6,
        OptionButton,
    }

    enum Images
    {
        MapTop,
        MapBottom,
        MapCenter1,
        MapCenter2,
    }

    UI_StageBlock[] _stageBlockUI = new UI_StageBlock[20];
    UI_StartStagePopup _startStagePopupUI;
    ScrollRect scroll;

    public ChapterResourceData ChapterData;

    public UI_InventoryPopup InventoryPopupUI;

    public int SelectedChapter { get; private set; } = 1;

    public int SelectedStage { get; private set; } = 1;

    public UI_SelectStageSceneTop TopUI { get; private set; }
    public UI_SelectStageSceneBottom BottomUI { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        BindButtons(typeof(Buttons));
        BindImages(typeof(Images));
        BindObjects(typeof(GameObjects));

        GetButton((int)Buttons.ChapterButton_1).gameObject.BindEvent((PointerEventData evt) => { OnClickChapterButton(1, evt); });
        GetButton((int)Buttons.ChapterButton_2).gameObject.BindEvent((PointerEventData evt) => { OnClickChapterButton(2, evt); });
        GetButton((int)Buttons.ChapterButton_3).gameObject.BindEvent((PointerEventData evt) => { OnClickChapterButton(3, evt); });
        GetButton((int)Buttons.ChapterButton_4).gameObject.BindEvent((PointerEventData evt) => { OnClickChapterButton(4, evt); });
        GetButton((int)Buttons.ChapterButton_5).gameObject.BindEvent((PointerEventData evt) => { OnClickChapterButton(5, evt); });
        GetButton((int)Buttons.ChapterButton_6).gameObject.BindEvent((PointerEventData evt) => { OnClickChapterButton(6, evt); });

        GetButton((int)Buttons.OptionButton).gameObject.BindEvent(OnClickOptionButton);

        TopUI = Utils.FindChild<UI_SelectStageSceneTop>(gameObject, "UI_SelectStageSceneTop", true);
        TopUI.SetInfo(this);

        BottomUI = Utils.FindChild<UI_SelectStageSceneBottom>(gameObject, "UI_SelectStageSceneBottom", true);
        BottomUI.SetInfo(this);

        scroll = Utils.FindChild<ScrollRect>(gameObject, recursive: true);
        SelectedStage = 0;

        for (int i = 0; i < _stageBlockUI.Length; i++)
        {
            _stageBlockUI[i] = Utils.FindChild<UI_StageBlock>(gameObject, "UI_StageBlock" + (i + 1), recursive: true);
        }

        //RefreshUI();

    }

    public void SetInfo(int chapter)
    {
        // 챕터 이미지 데이터 세팅해줘야함
        SelectedChapter = chapter;

        RefreshUI();
    }

    private void RefreshUI()
    {
        //if (_init != true)
        //    return;

        if (Managers.Data.ChapterResources.TryGetValue(SelectedChapter, out ChapterResourceData chapterData) == false)
            return;

        ChapterData = chapterData;

        var sp = Managers.Resource.Load<Sprite>(chapterData.MapTop);

        if(sp == null)
        {
            Debug.Log("Null!");
        }

        GetImage((int)Images.MapTop).sprite = Managers.Resource.Load<Sprite>(chapterData.MapTop);

        GetImage((int)Images.MapBottom).sprite = Managers.Resource.Load<Sprite>(chapterData.MapBottom);

        GetImage((int)Images.MapCenter1).sprite = Managers.Resource.Load<Sprite>(chapterData.MapCenter);
        GetImage((int)Images.MapCenter2).sprite = Managers.Resource.Load<Sprite>(chapterData.MapCenter);

        for (int i = 0; i < _stageBlockUI.Length; i++)
        {
            _stageBlockUI[i].SetInfo(i + 1, this);
        }

        DisableButtonSelectedImage();
        EnableButtonSelectedImage();
    }

    private void DisableButtonSelectedImage()
    {
        GetObject((int)GameObjects.SelectedChapter1).SetActive(false);
        GetObject((int)GameObjects.SelectedChapter2).SetActive(false);
        GetObject((int)GameObjects.SelectedChapter3).SetActive(false);
        GetObject((int)GameObjects.SelectedChapter4).SetActive(false);
        GetObject((int)GameObjects.SelectedChapter5).SetActive(false);
        GetObject((int)GameObjects.SelectedChapter6).SetActive(false);
    }

    private void EnableButtonSelectedImage()
    {
        switch (SelectedChapter)
        {
            case 1:
                GetObject((int)GameObjects.SelectedChapter1).SetActive(true);
                break;
            case 2:
                GetObject((int)GameObjects.SelectedChapter2).SetActive(true);
                break;
            case 3:
                GetObject((int)GameObjects.SelectedChapter3).SetActive(true);
                break;
            case 4:
                GetObject((int)GameObjects.SelectedChapter4).SetActive(true);
                break;
            case 5:
                GetObject((int)GameObjects.SelectedChapter5).SetActive(true);
                break;
            case 6:
                GetObject((int)GameObjects.SelectedChapter6).SetActive(true);
                break;
        }
    }

    public void SetScrollPosition(int stageNum)
    {
        float bottomPositionY = _stageBlockUI[0].transform.position.y;
        float topPositionY = _stageBlockUI[_stageBlockUI.Length - 1].transform.position.y;
        float focusPositionY = _stageBlockUI[stageNum - 1].transform.position.y;
        //scroll.verticalNormalizedPosition = focusPositionY / (topPositionY - bottomPositionY);
        scroll.verticalNormalizedPosition = (stageNum - 1f) / ((float)_stageBlockUI.Length - 1f);
    }

    void MovePlayer(int stage)
    {
        GetObject((int)GameObjects.Player).SetActive(false);
        GetObject((int)GameObjects.Stand).SetActive(false);

        int index = stage - 1;
        GetObject((int)GameObjects.Stand).transform.position = _stageBlockUI[index].transform.position;
        GetObject((int)GameObjects.Player).transform.position = _stageBlockUI[index].transform.position;

        GetObject((int)GameObjects.Player).SetActive(true);
        GetObject((int)GameObjects.Stand).SetActive(true);

        GetObject((int)GameObjects.Player).transform.DOKill();
        GetObject((int)GameObjects.Player).transform.rotation = Quaternion.Euler(Vector3.zero);
        GetObject((int)GameObjects.Player).transform.DOPunchRotation(new Vector3(-90f, 0f, 0f), 1f, 4);
        Managers.Sound.Play(Define.ESound.Effect, "Sound_ButtonMain");
    }

    bool CheckChapter(int chapter)
    {
        if (chapter > Managers.Game.HighestChapter)
            return false;

        if (chapter == SelectedChapter)
            return false;

        SelectedStage = 0;
        return true;
    }

    public void ShowStartStagePopup()
    {
        var popup = Managers.UI.ShowPopupUI<UI_StartStagePopup>();
        int templateID = (SelectedChapter - 1) * 20 + SelectedStage;
        popup.SetInfo(Managers.Data.Stages[templateID]);

        Managers.Sound.Play(Define.ESound.Effect, "Sound_ButtonMain");
    }

    #region EventHandler

    public void OnSelectStage(int stage)
    {
        //Debug.Log($"SelectedStage : {SelectedStage} \t stage : {stage}");
        if (SelectedStage == stage)
        {
            ShowStartStagePopup();
            return;
        }

        for (int i = 0; i < _stageBlockUI.Length; i++)
        {
            if (i + 1 != stage)
                _stageBlockUI[i].SelectStage(false);
        }

        SelectedStage = stage;
        MovePlayer(SelectedStage);
    }

    private void OnClickChapterButton(int chapter, PointerEventData evt)
    {
        if (CheckChapter(chapter) == false)
            return;

        SelectedChapter = chapter;
        SelectedStage = 0;
        RefreshUI();
    }

    private void OnClickOptionButton(PointerEventData evt)
    {
        Managers.UI.ShowPopupUI<UI_Option>();
    }

    #endregion
}
