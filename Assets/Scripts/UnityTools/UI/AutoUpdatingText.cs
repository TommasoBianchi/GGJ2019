using UnityEngine;
using UnityEngine.UI;

namespace UnityTools.UI
{
    [RequireComponent(typeof(Text))]
    public class AutoUpdatingText : MonoBehaviour
    {

        [SerializeField, Tooltip("Must be implementing the IPrintableValue interface")]
        private ScriptableObject printableValue;

        private IPrintableValue value;

        private Text text;

        private void Awake()
        {
            text = GetComponent<Text>();

            value = printableValue as IPrintableValue;
            value.onPrintableValueChanged += UpdateText;
        }

        private void UpdateText(string newText)
        {
            text.text = newText;
        }

        private void OnValidate()
        {
            if (printableValue == null)
            {
                return;
            }

            if (!(printableValue is IPrintableValue))
            {
                Debug.LogError("Must assign an IPrintableValue to the printableValue field of AutoUpdatingText");
                printableValue = null;
            }
        }
    }
}