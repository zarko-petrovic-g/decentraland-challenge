using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UnitCountView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI titleText;

    [SerializeField]
    private Slider countSlider;

    [SerializeField]
    private TextMeshProUGUI countText;

    private IArmyModel armyModel;

    [NonSerialized]
    private UnitType unitType;

    public UnitType UnitType
    {
        get => unitType;
        set
        {
            unitType = value;
            titleText.text = UnitType + "s";
        }
    }

    private void Awake()
    {
        countSlider.onValueChanged.AddListener(OnUnitCountUpdated);
    }

    public void SetWithoutNotify(int count)
    {
        countSlider.SetValueWithoutNotify(count);
        countText.text = count.ToString();
    }

    public void Bind(UnityAction<float> onValueChanged)
    {
        countSlider.onValueChanged.AddListener(onValueChanged);
    }

    private void OnUnitCountUpdated(float value)
    {
        int count = (int)value;
        countText.text = count.ToString();
    }
}