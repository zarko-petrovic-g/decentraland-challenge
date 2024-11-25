using System;
using System.Linq;
using UnityEngine;

public class Battle : MonoBehaviour
{
    [SerializeField]
    private ArmyModelSO army1Model;

    [SerializeField]
    private ArmyModelSO army2Model;

    [SerializeField]
    private UnitPrefab[] unitPrefabs;

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

    [SerializeField]
    private float gravity = 9.81f;

    private bool gameOverAnnounced;

    public float BattlefieldSize =>
        battleRadius * 3f; // *2 should be just enough but add some extra space just in case

    public float PartitionSize => partitionSize;

    public Vector3 Center { get; private set; }

    public Army Army1 { get; private set; }
    public Army Army2 { get; private set; }

    public Pool Pool { get; private set; }

    public bool IsGameOver => Army1.UnitCount == 0 || Army2.UnitCount == 0;
    public Army Winner
    {
        get
        {
            if(!IsGameOver)
            {
                return null;
            }

            return Army1.UnitCount == 0 ? Army2 : Army1;
        }
    }

    private void Awake()
    {
        Army1 = new Army();
        Army2 = new Army();

        Army1.InstantiateUnits(army1Model, leftArmySpawnBounds.bounds, army1Color, Army2, unitPrefabs, this);
        Army2.InstantiateUnits(army2Model, rightArmySpawnBounds.bounds, army2Color, Army1, unitPrefabs, this);

        Center = CalculateCenter();

        UnitPrefab archerPrefabData = unitPrefabs.First(prefab => prefab.type == UnitType.Archer);
        var archerPrefab = archerPrefabData.prefab as Archer;
        int arrowCount = army1Model.GetUnitCount(UnitType.Archer) + army2Model.GetUnitCount(UnitType.Archer);
        var arrows = new ArcherArrow[arrowCount];

        for(int i = 0; i < arrowCount; i++)
        {
            arrows[i] = Instantiate(archerPrefab.ArrowPrefab);
        }

        UnitPrefab cannonPrefabData = unitPrefabs.First(prefab => prefab.type == UnitType.Cannon);
        var cannonPrefab = cannonPrefabData.prefab as Cannon;
        int unitsInDamageDiameter = Mathf.CeilToInt(cannonPrefab.CannonStats.damageRadius * 2f / minUnitDistance);
        int cannonballMaxHits = unitsInDamageDiameter * unitsInDamageDiameter;
        int cannonBallCount = army1Model.GetUnitCount(UnitType.Cannon) + army2Model.GetUnitCount(UnitType.Cannon);
        var cannonBalls = new CannonBall[cannonBallCount];

        for(int i = 0; i < cannonBallCount; i++)
        {
            cannonBalls[i] = Instantiate(cannonPrefab.CannonBallPrefab);
            cannonBalls[i].MaxHits = cannonballMaxHits;
            cannonBalls[i].Gravity = gravity;
        }

        Pool = new Pool(new[]
        {
            new Pool.CategoryData
            {
                Category = PoolableCategory.ArcherArrow,
                Objects = arrows
            },
            new Pool.CategoryData
            {
                Category = PoolableCategory.CannonBall,
                Objects = cannonBalls
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
        if(IsGameOver && !gameOverAnnounced)
        {
            OnGameOver?.Invoke(Winner);
            gameOverAnnounced = true;
        }
    }

    private Vector3 CalculateCenter()
    {
        return (Army1.Center * Army1.UnitCount + Army2.Center * Army2.UnitCount) / (Army1.UnitCount + Army2.UnitCount);
    }
}