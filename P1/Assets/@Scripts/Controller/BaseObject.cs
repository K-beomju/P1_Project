using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : InitBase
{
    public Collider2D _collider { get; protected set; }
    public SpriteRenderer _sprite { get; protected set; }

    public Vector3 CenterPosition => _collider.bounds.center;


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        _collider = GetComponent<Collider2D>();
        _sprite = GetComponent<SpriteRenderer>();
        return true;
    }

}
