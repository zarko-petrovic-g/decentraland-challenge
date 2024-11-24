public interface IArmyPresenter
{
    void UpdateWarriors(int warriors);
    void UpdateArchers(int archers);
    void UpdateCannons(int cannons);
    void UpdateStrategy(ArmyStrategy strategy);
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

    public void UpdateWarriors(int warriors)
    {
        model.Warriors = warriors;
    }

    public void UpdateArchers(int archers)
    {
        model.Archers = archers;
    }

    public void UpdateCannons(int cannons)
    {
        model.Cannons = cannons;
    }

    public void UpdateStrategy(ArmyStrategy strategy)
    {
        model.Strategy = strategy;
    }
}