using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleInstantiator : MonoBehaviour
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

    // TODO consider returning IEnumerable instead, performance implications?
    public List<UnitBase> AllUnits { get; private set; }

    public Vector3 Center { get; private set; }
    
    public Army Army1 { get; private set; }
    public Army Army2 { get; private set; }

    // TODO consider making non-singleton for unit testing friendliness
    public static BattleInstantiator instance { get; private set; }

    public Color Army1Color => army1Color;
    public Color Army2Color => army2Color;

    private void Awake()
    {
        instance = this;

        Army1 = new Army();
        Army1.InstantiateUnits(army1Model, leftArmySpawnBounds.bounds, warriorPrefab, archerPrefab, army1Color);
        Army2 = new Army();
        Army2.InstantiateUnits(army2Model, rightArmySpawnBounds.bounds, warriorPrefab, archerPrefab, army2Color);
        Army1.EnemyArmy = Army2;
        Army2.EnemyArmy = Army1;

        AllUnits = new List<UnitBase>(Army1.UnitCount + Army2.UnitCount);
        AllUnits.AddRange(Army1.Units);
        AllUnits.AddRange(Army2.Units);
        
        Center = Utils.GetCenter(AllUnits);    
        
        cameraController.SetArmies(Army1, Army2);
    }

    private void Update()
    {
        Army1.Update();
        Army2.Update();
        Center = Utils.GetCenter(AllUnits);
        
        // TODO event upon unit death instead of polling
        if(Army1.UnitCount == 0 || Army2.UnitCount == 0)
        {
            gameOverMenu.gameObject.SetActive(true);
            gameOverMenu.Populate();
        }
    }
}