public interface IArmyPresenter
{
    void UpdateStrategy(ArmyStrategy strategy);
    void UpdateUnitCount(UnitType unitType, int value);
}

public class ArmyPresenter : IArmyPresenter
{
    private readonly IArmyModel model;
    private readonly IArmyView view;

    public ArmyPresenter(IArmyModel model, IArmyView view)
    {
        this.model = model;
        this.view = view;
        this.view.UpdateWithModel(model);
    }

    public void UpdateUnitCount(UnitType unitType, int value)
    {
        model.SetUnitCount(unitType, value);
    }

    public void UpdateStrategy(ArmyStrategy strategy)
    {
        model.Strategy = strategy;
    }
}