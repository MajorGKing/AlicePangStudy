using Data;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : BaseController
{
    public int TemplateID { get; set; }
    BossData _bossData;

    public bool IsBusy { get; set; }

    StatusBar _statusBar;

    Animator _animator;
    SkeletonAnimation _spineAni;

    public int MaxHp { get; set; }
    int _hp = 0;
    public int Hp
    {
        get { return _hp; }
        set
        {
            _hp = value;
            if (_statusBar != null && MaxHp > 0)
            {
                _statusBar.SetHp(_hp, MaxHp);
            }
        }
    }

    public void StartBossTurn()
    {

    }
}
