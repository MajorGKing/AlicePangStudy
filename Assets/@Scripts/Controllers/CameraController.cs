using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : BaseController
{
    DOTweenAnimation _tweenAnimation;

    private void Awake()
    {
        SetCameraSize();

        _tweenAnimation = transform.GetComponent<DOTweenAnimation>();
    }

    void SetCameraSize()
    {
        float width = Screen.width;
        float height = Screen.height;

        float baseRatio = 1080f / 2280f;
        float currentRatio = width / height;

        if (currentRatio >= baseRatio)
            Camera.main.orthographicSize = 11.4f;
        else
            Camera.main.orthographicSize = 11.4f * baseRatio / currentRatio;
    }

    public void CameraAnimation(string key)
    {
        _tweenAnimation.DORestartAllById(key);
    }

}
