using System;
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
    private float minUnitDistance = 2f;

    [SerializeField]
    private float battleRadius = 80f;

    [SerializeField]
    private float partitionSize = 10f;

    private bool gameOver;

    public float BattlefieldSize =>
        battleRadius * 2.5f; // *2 should be just enough but add some extra space just in case

    public float PartitionSize => partitionSize;

    public Vector3 Center { get; private set; }

    public Army Army1 { get; private set; }
    public Army Army2 { get; private set; }

    public Pool Pool { get; private set; }

    private void Awake()
    {
        Army1 = new Army();
        Army2 = new Army();
        // TODO it's not really scalable if there are more types of units
        Army1.InstantiateUnits(army1Model, leftArmySpawnBounds.bounds, warriorPrefab, archerPrefab, army1Color, Army2,
            this);

        Army2.InstantiateUnits(army2Model, rightArmySpawnBounds.bounds, warriorPrefab, archerPrefab, army2Color, Army1,
            this);

        Center = CalculateCenter();

        int arrowCount = army1Model.Archers + army2Model.Archers;
        var arrows = new ArcherArrow[arrowCount];

        for(int i = 0; i < arrowCount; i++)
        {
            arrows[i] = Instantiate(archerPrefab.ArrowPrefab);
        }

        Pool = new Pool(new[]
        {
            new Pool.CategoryData
            {
                Category = PoolableCategory.ArcherArrow,
                Objects = arrows
            }
        });
    }

    private void Update()
    {
        Army1.Update();
        Army2.Update();
        Center = CalculateCenter();
    }

    public event Action<Army> OnGameOver;

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

    public void UnitDied(UnitBase unit)
    {
        if((Army1.UnitCount == 0 || Army2.UnitCount == 0) && !gameOver)
        {
            OnGameOver?.Invoke(Army1.UnitCount == 0 ? Army2 : Army1);
            gameOver = true;
        }
    }

    private Vector3 CalculateCenter()
    {
        return (Army1.Center * Army1.UnitCount + Army2.Center * Army2.UnitCount) / (Army1.UnitCount + Army2.UnitCount);
    }
}