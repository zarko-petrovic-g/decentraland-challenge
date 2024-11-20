using System;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour
{
    [SerializeField]
    private ArmyModelSO army1Model;

    [SerializeField]
    private ArmyModelSO army2Model;

    [SerializeField]
    private Warrior warriorPrefab;

    [SerializeField]
    private Archer archerPrefab;

    [SerializeField]
    private BoxCollider leftArmySpawnBounds;

    [SerializeField]
    private BoxCollider rightArmySpawnBounds;

    [SerializeField]
    private Color army1Color;

    [SerializeField]
    private Color army2Color;

    [SerializeField]
    private GameOverMenu gameOverMenu;

    [SerializeField]
    private CameraController cameraController;

    [SerializeField]
    private float minUnitDistance = 2f;
    
    [SerializeField]
    private float battleRadius = 80f;
    
    private readonly List<UnitBase> allUnits = new List<UnitBase>();

    public IEnumerable<UnitBase> AllUnits => allUnits;

    public Vector3 Center { get; private set; }

    public Army Army1 { get; private set; }
    public Army Army2 { get; private set; }

    private void Awake()
    {
        Army1 = new Army();
        Army2 = new Army();
        Army1.InstantiateUnits(army1Model, leftArmySpawnBounds.bounds, warriorPrefab, archerPrefab, army1Color, Army2,
            this);

        Army2.InstantiateUnits(army2Model, rightArmySpawnBounds.bounds, warriorPrefab, archerPrefab, army2Color, Army1,
            this);

        allUnits.AddRange(Army1.Units);
        allUnits.AddRange(Army2.Units);

        foreach(UnitBase unit in allUnits)
        {
            unit.OnDeath += UnitDied;
        }

        Center = Utils.GetCenter(allUnits);

        cameraController.SetArmies(Army1, Army2);
    }

    private void Update()
    {
        Army1.Update();
        Army2.Update();
        Center = Utils.GetCenter(allUnits);
    }

    public void EvadeOtherUnits(UnitBase unit)
    {
        Vector3 position = unit.CachedTransform.position;
        int count = allUnits.Count;

        for(int i = 0; i < count; i++)
        {
            UnitBase otherUnit = allUnits[i];
            Vector3 toEvadePosition = otherUnit.CachedTransform.position;
            float dist = Vector3.Distance(position, toEvadePosition);

            if(dist < minUnitDistance)
            {
                Vector3 toNearest = (toEvadePosition - position).normalized;
                position -= toNearest * (minUnitDistance - dist);
                unit.CachedTransform.position = position;
            }
        }
    }
    
    public bool ClampPosition(UnitBase unit)
    {
        Vector3 position = unit.CachedTransform.position;
        float centerDist = Vector3.Distance(position, Center);

        if(centerDist > battleRadius)
        {
            Vector3 toNearest = Center - position;
            unit.CachedTransform.position -= toNearest.normalized * (battleRadius - centerDist);
            return true;
        }

        return false;
    }

    private void UnitDied(UnitBase unit)
    {
        allUnits.Remove(unit);

        if(Army1.UnitCount == 0 || Army2.UnitCount == 0 && !gameOverMenu.gameObject.activeInHierarchy)
        {
            gameOverMenu.gameObject.SetActive(true);
            gameOverMenu.Populate();
        }
    }
}