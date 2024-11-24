using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI armyWins;

    [SerializeField]
    private RectTransform content;

    [SerializeField]
    private Button goToMenu;

    [SerializeField]
    private Battle battle;

    private void Awake()
    {
        goToMenu.onClick.AddListener(GoToMenu);
    }

    private void Start()
    {
        content.gameObject.SetActive(false);
        battle.OnGameOver += OnGameOver;
    }

    private void OnGameOver(Army winner)
    {
        content.gameObject.SetActive(true);
        Populate(winner);
    }

    private void Populate(Army winner)
    {
        if(battle.Army1 == winner)
        {
            armyWins.text = "Army 1 wins!";
        }

        if(battle.Army2 == winner)
        {
            armyWins.text = "Army 2 wins!";
        }

    }

    private void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }
}