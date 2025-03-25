using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StoryPopup : UI_Popup
{
    [Serializable]
    class CutSceneSequence
    {
        public GameObject[] CutScenes;
    }

    [SerializeField]
    CutSceneSequence[] _sequence;
    Coroutine _playCutScene;

    enum GameObjects
    {
        SkipPanel,
    }

    int _currentSequence = 0;
    int _scenesIndex = 0;

    bool _isComplete = false;
    Action? _callBack;

    float time = 0f;

    protected override void Awake()
    {
        base.Awake();

        BindObjects(typeof(GameObjects));

        GetObject((int)GameObjects.SkipPanel).BindEvent(OnClickSkipPanel);

        _playCutScene = StartCoroutine(CoPlayCutScene());
    }

    public void SetInfo(Action callBack)
    {
        _callBack = callBack;
    }

    IEnumerator CoPlayCutScene()
    {
        time = 0f;
        while (true)
        {
            time += Time.deltaTime;
            Debug.Log(time);

            if (time > 2f)
            {
                NextScene();
            }
            if (_scenesIndex >= _sequence[_currentSequence].CutScenes.Length)
                break;

            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(1f);

        _isComplete = true;
        _playCutScene = null;
    }

    void NextScene()
    {
        if (_scenesIndex < _sequence[_currentSequence].CutScenes.Length)
        {
            _sequence[_currentSequence].CutScenes[_scenesIndex].SetActive(true);
            _scenesIndex++;
            time = 0f;
        }
        else
        {
            _isComplete = true;
        }
    }

    void NextSequence()
    {
        if (_currentSequence < _sequence.Length - 1)
        {
            for (int i = 0; i < _sequence[_currentSequence].CutScenes.Length; i++)
            {
                _sequence[_currentSequence].CutScenes[i].SetActive(false);
            }
            _currentSequence++;
            _scenesIndex = 0;
            if (_playCutScene != null)
                StopCoroutine(_playCutScene);

            _isComplete = false;
            _playCutScene = StartCoroutine(CoPlayCutScene());
        }
        else
        {
            _callBack?.Invoke();
            Managers.UI.ClosePopupUI(this);
        }
    }


    #region EventHandler
    void OnClickSkipPanel(PointerEventData evt)
    {
        Debug.Log("Click");
        if (_isComplete)
            NextSequence();
        else
            NextScene();
    }
    #endregion


}
