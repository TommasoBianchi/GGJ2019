using UnityEngine;
using TMPro;
using UnityTools.DataManagement;

public class StartRoundCountDown : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI text;

    private bool isStarted;
    private float timeLeft;

    private void Update()
    {
        if (isStarted)
        {
            timeLeft -= Time.deltaTime;
            text.text = Mathf.CeilToInt(timeLeft).ToString();

            if (timeLeft <= 0)
            {
                // Start round
                GameManager.StartRound();
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
        timeLeft = ConstantsManager.CountdownForRoundStart;
        text.text = Mathf.CeilToInt(timeLeft).ToString();
        text.enabled = true;
    }
}
