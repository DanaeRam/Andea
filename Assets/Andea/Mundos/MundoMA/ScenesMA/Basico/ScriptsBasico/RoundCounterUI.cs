using TMPro;
using UnityEngine;

public class RoundCounterUI : MonoBehaviour
{
    public TextMeshProUGUI counterText;

    void Start()
    {
        if (counterText == null)
            counterText = GetComponent<TextMeshProUGUI>();

        UpdateCounter();
    }

    void OnEnable()
    {
        UpdateCounter();
    }

    public void UpdateCounter()
    {
        if (counterText == null) return;

        counterText.text = MathSceneTransitionData.currentRound + " / " + MathSceneTransitionData.maxRounds;
    }
}