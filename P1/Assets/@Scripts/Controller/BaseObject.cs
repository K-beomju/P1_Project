using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseObject : InitBase
{
    public EObjectType ObjectType { get; protected set; } = EObjectType.None;
    public Collider2D _collider { get; protected set; }
    public SpriteRenderer _sprite { get; protected set; }

    public Vector3 CenterPosition => _collider.bounds.center;

    public int DataTemplateID { get; set; }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        _collider = GetComponent<Collider2D>();
        _sprite = GetComponent<SpriteRenderer>();
        return true;
    }

    public virtual void SetInfo(int dataTemplateID)
    { 
        DataTemplateID = dataTemplateID;
    }

}
