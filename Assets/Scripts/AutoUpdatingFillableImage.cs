using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AutoUpdatingFillableImage : MonoBehaviour
{

    [SerializeField]
    private FloatValue floatValue;

    public FloatValue FloatValue { get { return floatValue; } }

    private Image image;

    protected void Awake()
    {
        image = GetComponent<Image>();
        floatValue.onValueChanged += UpdateImage;
    }

    private void UpdateImage(float newValue)
    {
        image.fillAmount = floatValue.Value;
    }

    private void OnDestroy()
    {
        floatValue.RemoveAllListeners();
    }
}