using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : InitBase
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }
}
