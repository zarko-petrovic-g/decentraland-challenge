using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitStrategy : IUnitStrategy
{
    protected readonly UnitBase unit;

    protected UnitStrategy(UnitBase unit)
    {
        this.unit = unit;
    }

    public abstract void Update();
}
