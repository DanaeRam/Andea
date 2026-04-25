using UnityEngine;
using UnityEngine.UI;

public class CreditsPanelUI : MonoBehaviour
{
    [Header("Panel de créditos")]
    public GameObject creditsPanel;

    [Header("Botones")]
    public Button openCreditsButton;
    public Button closeCreditsButton;

    private void Start()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        if (openCreditsButton != null)
        {
            openCreditsButton.onClick.RemoveAllListeners();
            openCreditsButton.onClick.AddListener(OpenCredits);
        }

        if (closeCreditsButton != null)
        {
            closeCreditsButton.onClick.RemoveAllListeners();
            closeCreditsButton.onClick.AddListener(CloseCredits);
        }
    }

    public void OpenCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }
}