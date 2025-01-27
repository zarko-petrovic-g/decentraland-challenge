using System;
using UnityEngine;

public abstract class RangedUnit : UnitBase
{
    public override ArmyStrategy ArmyStrategy
    {
        set
        {
            UnitStrategy = value switch
            {
                ArmyStrategy.Basic => new StrategyBasic(this),
                ArmyStrategy.Defensive => new RangedStrategyDefensive(this),
                ArmyStrategy.Bloodthirsty => new StrategyBloodthirsty(this),
                ArmyStrategy.Cowardly => new RangedStrategyCowardly(this),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    protected override void PerformAttack(UnitBase enemy)
    {
        LaunchProjectile(CachedTransform.position, enemy.CachedTransform.position);
    }

    protected abstract void LaunchProjectile(Vector3 position, Vector3 targetPosition);
}