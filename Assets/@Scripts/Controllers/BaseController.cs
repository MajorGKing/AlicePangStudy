using System;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering;
using Event = Spine.Event;

public class BaseController : MonoBehaviour
{
    protected bool _init = false;
    protected virtual void Awake()
    {
        _init = true;
    }
}

