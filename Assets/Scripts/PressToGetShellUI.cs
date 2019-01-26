using UnityEngine;
using UnityEngine.UI;
using UnityTools.DataManagement;

public class PressToGetShellUI : MonoBehaviour
{

    [SerializeField]
    private Image fillImage;

    private float targetPressDuration;
    private float startPressingTime;
    private bool isPressing;
    private Player pressingPlayer;
    private Shell targetShell;

    private void Awake()
    {
        transform.SetParent(FindObjectOfType<Canvas>().transform);

        targetPressDuration = ConstantsManager.TimeToPressToPickupShell;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        Position();

        if (isPressing)
        {
            float t = Mathf.Clamp01((Time.time - startPressingTime) / targetPressDuration);
            fillImage.fillAmount = t;

            if(t >= 1)
            {
                pressingPlayer.PickupShell(targetShell);
                StopPressing();
                Hide();
            }
        }
    }

    public void Show(Player player, Shell shell)
    {
        pressingPlayer = player;
        targetShell = shell;
        gameObject.SetActive(true);
        fillImage.fillAmount = 0;

        Position();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void StartPressing()
    {
        if (isPressing)
        {
            return;
        }

        isPressing = true;
        startPressingTime = Time.time;
        fillImage.fillAmount = 0;
    }

    public void StopPressing()
    {
        isPressing = false;
        fillImage.fillAmount = 0;
    }

    private void Position()
    {
        Vector3 offset = new Vector3(0, 3, 0); // In world coordinates
        //Vector3 position = Camera.main.WorldToScreenPoint(pressingPlayer.transform.position + offset);
        Vector3 position = Camera.main.WorldToScreenPoint(targetShell.transform.position + offset);
        transform.position = position;
    }
}
