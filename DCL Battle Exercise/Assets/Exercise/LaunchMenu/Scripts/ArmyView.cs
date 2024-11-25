using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface IArmyView
{
    void UpdateWithModel(IArmyModel model);
}

public class ArmyView : MonoBehaviour, IArmyView
{
    [SerializeField]
    private UnitCountView unitCountPrefab;

    [SerializeField]
    private Transform unitCountsParent;

    [SerializeField]
    private TMP_Dropdown strategyDropdown;

    private readonly List<UnitCountView> unitCountUIs = new List<UnitCountView>();
    private IArmyPresenter presenter;

    private EnumDropdownWrapper<ArmyStrategy> wrappedStrategyDropdown;

    private void Awake()
    {
        foreach(UnitType unitType in Enum.GetValues(typeof(UnitType)))
        {
            UnitCountView unitCountView = Instantiate(unitCountPrefab, unitCountsParent);
            unitCountView.UnitType = unitType;
            unitCountUIs.Add(unitCountView);
        }

        wrappedStrategyDropdown = new EnumDropdownWrapper<ArmyStrategy>(strategyDropdown);
        wrappedStrategyDropdown.OnValueChanged += OnStrategyChanged;
    }

    private void OnDestroy()
    {
        wrappedStrategyDropdown.OnValueChanged -= OnStrategyChanged;
        wrappedStrategyDropdown?.Dispose();
    }

    public void UpdateWithModel(IArmyModel model)
    {
        foreach(UnitCountView unitCountUI in unitCountUIs)
        {
            unitCountUI.SetWithoutNotify(model.GetUnitCount(unitCountUI.UnitType));
            unitCountUI.Bind(value => { presenter?.UpdateUnitCount(unitCountUI.UnitType, (int)value); });
        }

        wrappedStrategyDropdown.SetValueWithoutNotify(model.Strategy);
    }

    public void BindPresenter(IArmyPresenter presenter)
    {
        this.presenter = presenter;
    }

    private void OnStrategyChanged(ArmyStrategy strategy)
    {
        presenter?.UpdateStrategy(strategy);
    }
}