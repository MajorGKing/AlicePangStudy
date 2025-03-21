using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectStageScene : BaseScene
{
    protected override void Awake()
    {
        base.Awake();

        SceneType = Define.EScene.SelectStageScene;
        
    }

    public override void Clear()
    {
        throw new System.NotImplementedException();
    }
}
