using UnityEngine;
using UnityEngine.UI;

public class RoundsWonUI : MonoBehaviour
{

    [SerializeField]
    private Sprite roundNotWonSprite;
    [SerializeField]
    private Sprite roundWonSprite;
    [SerializeField]
    private IntValue roundsWonValue;

    private void Awake()
    {
        SetupSprites(0);

        roundsWonValue.onValueChanged += SetupSprites;
    }

    private void SetupSprites(int roundsWonAmount)
    {
        for (int i = 0; i < 2; i++)
        {
            transform.GetChild(i).GetComponent<Image>().sprite = i < roundsWonAmount ? roundWonSprite : roundNotWonSprite;
        }
    }
}
