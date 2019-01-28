using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Image))]
public class EndMatchUI : MonoBehaviour
{

    private Image image;
    private float startTransitionTime;

    private void Update()
    {
        float perc = (Time.time - startTransitionTime) / 3;
        Color color = image.color;
        color.a = perc;
        image.color = color;

        if(perc >= 1)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void OnEnable()
    {
        image = GetComponent<Image>();
        Color color = image.color;
        color.a = 0;
        image.color = color;
        startTransitionTime = Time.time;
    }
}
