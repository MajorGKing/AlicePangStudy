using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : BaseController
{
    SpriteRenderer _spriteRenderer;
    public Color _damagedColor = new Vector4(1f, 0.874f, 0.874f, 1f);

    Sequence _seq;
    float _speed = 30.0f;
    int _currentIndex;

    //[SerializeField]
    Sprite _normalSprite;
    //[SerializeField]
    Sprite _damagedSprite;
    //[SerializeField]
    Sprite _attackSprite;

    [SerializeField]
    GameObject _shadow;

    [SerializeField]
    GameObject _redShadow;

    [SerializeField]
    GameObject _selectedMark;

    bool _selected = false;
    public bool Selected
    {
        get { return _selected; }
        set
        {
            _selected = value;
            if (_selected)
            {
                _shadow.SetActive(false);
                _redShadow.SetActive(true);
                _selectedMark.SetActive(true);
            }
            else
            {
                _shadow.SetActive(true);
                _redShadow.SetActive(false);
                _selectedMark.SetActive(false);
            }
        }
    }


    StatusBar _statusBar;
}
