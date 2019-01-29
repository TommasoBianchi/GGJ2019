using UnityEngine;
using TMPro;
using UnityTools.DataManagement;
using UnityEngine.SceneManagement;

public class StartLevelCountDown : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private Settings settings;

    private bool isStarted;
    private float timeLeft;

    private void Update()
    {
        if (isStarted)
        {
            timeLeft -= Time.deltaTime;
            text.text = Mathf.CeilToInt(timeLeft).ToString();

            if(timeLeft <= 0)
            {
                // Start game
                settings.NumberOfPlayers = InputChecker.ReadyPlayersCount();
                InputChecker.ClearData();
                SceneManager.LoadScene("ArenaScene");
                text.enabled = false;
            }
        }
    }

    public void StartCountDown()
    {
        if (isStarted)
        {
            return;
        }

        isStarted = true;
        timeLeft = ConstantsManager.CountdownForJoystickSelection;
        text.text = Mathf.CeilToInt(timeLeft).ToString();
        text.enabled = true;
    }
}
