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

    [SerializeField]
    private float partitionSize = 10f;

    private readonly List<UnitBase> allUnits = new List<UnitBase>();

    public float BattlefieldSize =>
        battleRadius * 2.5f; // *2 should be just enough but add some extra space just in case

    public float PartitionSize => partitionSize;

    public IEnumerable<UnitBase> AllUnits => allUnits;

    public Vector3 Center { get; private set; }

    public Army Army1 { get; private set; }
    public Army Army2 { get; private set; }

    private void Awake()
    {
        Army1 = new Army();
        Army2 = new Army();
        // TODO it's not really scalable if there are more types of units
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
        Army1.EvadeOtherUnits(unit, minUnitDistance);
        Army2.EvadeOtherUnits(unit, minUnitDistance);
    }

    public bool ClampPosition(UnitBase unit)
    {
        Vector3 position = unit.CachedTransform.position;
        float centerDist = Vector3.Distance(position, Center);

        if(centerDist > battleRadius)
        {
            Vector3 toNearest = Center - position;
            Vector3 newPosition = position - toNearest.normalized * (battleRadius - centerDist);
            unit.SetPosition(newPosition);
            return true;
        }

        return false;
    }

    private void UnitDied(UnitBase unit)
    {
        unit.OnDeath -= UnitDied;

        allUnits.Remove(unit);

        if(Army1.UnitCount == 0 || Army2.UnitCount == 0 && !gameOverMenu.gameObject.activeInHierarchy)
        {
            gameOverMenu.gameObject.SetActive(true);
            gameOverMenu.Populate();
        }
    }
}