using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI armyWins;

    [SerializeField]
    private Button goToMenu;

    [SerializeField]
    private Battle battle;

    private void Awake()
    {
        goToMenu.onClick.AddListener(GoToMenu);
    }

    public void Populate()
    {
        if(battle.Army1.UnitCount == 0)
        {
            armyWins.text = "Army 1 wins!";
        }

        if(battle.Army2.UnitCount == 0)
        {
            armyWins.text = "Army 2 wins!";
        }

    }

    private void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }
}