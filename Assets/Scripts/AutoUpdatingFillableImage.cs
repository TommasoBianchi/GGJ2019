using UnityEngine;
using UnityEngine.UI;
using UnityTools.DataManagement;

[RequireComponent(typeof(Image))]
public class AutoUpdatingFillableImage : MonoBehaviour
{

    [SerializeField]
    private FloatValue floatValue;

    public FloatValue FloatValue { get { return floatValue; } }

    private Image image;
    private float targetValue;
    private float transitionStartValue;
    private float transitionDuration;
    private float startTransitionTime;

    protected void Awake()
    {
        image = GetComponent<Image>();
        floatValue.onValueChanged += UpdateImage;
        transitionDuration = ConstantsManager.HealthTransitionTime;
    }

    private void Update()
    {
        float t = (Time.time - startTransitionTime) / transitionDuration;
        
        if(t <= 1)
        {
            image.fillAmount = Mathf.Lerp(transitionStartValue, targetValue, t);
        }
    }

    private void UpdateImage(float newValue)
    {
        image.fillAmount = targetValue;
        targetValue = newValue;
        transitionStartValue = image.fillAmount;
        startTransitionTime = Time.time;
    }

    private void OnDestroy()
    {
        floatValue.RemoveAllListeners();
    }
}