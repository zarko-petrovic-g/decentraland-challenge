using System;

public class MeleeUnit : UnitBase
{
    public override ArmyStrategy ArmyStrategy
    {
        set
        {
            UnitStrategy = value switch
            {
                ArmyStrategy.Basic => new StrategyBasic(this),
                ArmyStrategy.Defensive => new MeleeStrategyDefensive(this),
                ArmyStrategy.Bloodthirsty => new StrategyBloodthirsty(this),
                ArmyStrategy.Cowardly => new MeleeStrategyCowardly(this),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    protected override void PerformAttack(UnitBase enemy)
    {
        enemy.Hit(AttackDamage, CachedTransform.position);
    }
}