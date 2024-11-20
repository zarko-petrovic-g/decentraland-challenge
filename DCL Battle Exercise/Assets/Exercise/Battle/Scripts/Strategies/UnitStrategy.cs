public interface IUnitStrategy
{
    void Update();
}

public abstract class UnitStrategy : IUnitStrategy
{
    protected readonly UnitBase unit;

    protected UnitStrategy(UnitBase unit)
    {
        this.unit = unit;
    }

    public abstract void Update();
}