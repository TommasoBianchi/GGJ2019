using UnityEngine;
using UnityEngine.UI;

namespace UnityTools.UI
{
    [RequireComponent(typeof(Slider))]
    public class AutoUpdatingSlider : MonoBehaviour
    {

        [SerializeField]
        private FloatValue floatValue;

        private Slider slider;

        protected void Awake()
        {
            slider = GetComponent<Slider>();

            floatValue.onValueChanged += UpdateSlider;
        }

        private void UpdateSlider(float newValue)
        {
            slider.value = newValue;
        }

        private void OnDestroy()
        {
            floatValue.RemoveAllListeners();
        }
    }
}