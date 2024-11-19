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

    public readonly Army army1 = new Army();
    public readonly Army army2 = new Army();
    public static BattleInstantiator instance { get; private set; }

    public Color Army1Color => army1Color;
    public Color Army2Color => army2Color;

    private void Awake()
    {
        instance = this;

        army1.color = army1Color;
        army1.enemyArmy = army2;

        army2.color = army2Color;
        army2.enemyArmy = army1;

        InstanceArmy(army1Model, army1, leftArmySpawnBounds.bounds);
        InstanceArmy(army2Model, army2, rightArmySpawnBounds.bounds);

        cameraController.SetArmies(army1, army2);
    }

    private void Update()
    {
        if(army1.GetUnits().Count == 0 || army2.GetUnits().Count == 0)
        {
            gameOverMenu.gameObject.SetActive(true);
            gameOverMenu.Populate();
        }
    }

    private void InstanceArmy(IArmyModel model, Army army, Bounds instanceBounds)
    {
        for(int i = 0; i < model.warriors; i++)
        {
            Warrior warrior = Instantiate(warriorPrefab);
            warrior.transform.position = Utils.GetRandomPosInBounds(instanceBounds);

            warrior.army = army;
            warrior.armyModel = model;
            warrior.GetComponentInChildren<Renderer>().material.color = army.color;

            army.warriors.Add(warrior);
        }

        for(int i = 0; i < model.archers; i++)
        {
            Archer archer = Instantiate(archerPrefab);
            archer.transform.position = Utils.GetRandomPosInBounds(instanceBounds);

            archer.army = army;
            archer.armyModel = model;
            archer.GetComponentInChildren<Renderer>().material.color = army.color;

            army.archers.Add(archer);
        }
    }
}