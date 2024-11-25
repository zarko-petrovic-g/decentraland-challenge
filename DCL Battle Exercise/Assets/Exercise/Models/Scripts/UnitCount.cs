using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitCount 
{
    public UnitType type;
    public int count;

    public UnitCount(UnitType type, int count)
    {
        this.type = type;
        this.count = count;
    }
}
