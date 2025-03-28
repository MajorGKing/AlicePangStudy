using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerController : BaseController
{
    SpriteRenderer _spriteRenderer;

    Sequence _seq;
    float _speed = 30.0f;
    int _currentIndex;

    public int ComboCount
    {
        get
        {
            if (Managers.UI.SceneUI is UI_GameScene)
            {
                (Managers.UI.SceneUI as UI_GameScene)?.Combo(_currentIndex + 1);
            }

            return _currentIndex + 1;
        }
    }

    List<MonsterController> _targets;
    MonsterController _target;
    Vector3 _dest;
    Vector2 _attackDir;
    Vector3 _playerPos = new Vector3(0f, -2.2f, -2.2f);

    public Define.ECreatureState State { get; set; } = Define.ECreatureState.Idle;
    public bool IsBusy { get; set; } = true;

    public int Damage
    {
        get
        {
            int weaponID = Managers.Game.CurrentWeaponID();
            int level = Managers.Game.WeaponLevel[weaponID - 1];
            return Managers.Data.Weapons[weaponID].WeaponLevelData[level - 1].Damage;
        }
    }

    public Define.EKnockbackDirection Knockback
    {
        get
        {
            int weaponID = Managers.Game.CurrentWeaponID();
            return Managers.Data.Weapons[weaponID].KnockbackDirection;
        }
    }

    int _hp = 0;
    public int Hp
    {
        get { return _hp; }
        set
        {
            _hp = value;
            if (_statusBar != null && MaxHp > 0)
                _statusBar.SetHp(_hp, MaxHp);
        }
    }

    StatusBar _statusBar;

    [SerializeField]
    SpriteRenderer _weapon;

    [SerializeField]
    ParticleSystem _damagedEffect;

    [SerializeField]
    ParticleSystem _landingEffect;

    public int MaxHp { get; private set; }

    public void OnDamaged(MonsterController mc)
    {

    }

    public void SetInfo(Vector2 pos)
    {
        MaxHp = 100;
        Hp = 100;

        transform.position = pos;
    }

    public void StartAttack(List<MonsterController> targets)
    {
        _currentIndex = 0;
        _targets = targets;
        State = Define.ECreatureState.Idle;
        IsBusy = true;
    }

    public void RevivePlayer()
    {
        Hp = MaxHp;
        _currentIndex = 0;
        _targets.Clear();
    }

    public void ChangeWeaponImage()
    {
        _weapon.sprite = Managers.Resource.Load<Sprite>(Managers.Data.Weapons[Managers.Game.CurrentWeaponID()].Sprite);        
    }
}
