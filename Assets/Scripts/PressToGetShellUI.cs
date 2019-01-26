using UnityEngine;
using UnityEngine.UI;

public class PressToGetShellUI : MonoBehaviour
{

    [SerializeField]
    private Image fillImage;

    private float targetPressDuration;
    private float startPressingTime;
    private bool isPressing;
    private Player pressingPlayer;

    private void Awake()
    {
        transform.SetParent(FindObjectOfType<Canvas>().transform);

        targetPressDuration = 2; // TODO: read from constant manager
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isPressing)
        {
            float t = Mathf.Clamp01((Time.time - startPressingTime) / targetPressDuration);
            fillImage.fillAmount = t;

            if(t >= 1)
            {
                //pressingPlayer
            }
        }
    }

    public void StartPressing(Player player)
    {
        pressingPlayer = player;
        isPressing = true;
        startPressingTime = Time.time;

        gameObject.SetActive(true);
        fillImage.fillAmount = 0;
    }

    public void StopPressing(Player player)
    {
        isPressing = false;

        gameObject.SetActive(false);
        fillImage.fillAmount = 0;
    }
}
