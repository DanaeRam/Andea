using System.Collections;
using UnityEngine;

public class BookUIReveal : MonoBehaviour
{
    [Header("Animator del libro")]
    public Animator bookAnimator;
    public string openAnimationName = "BookTest2";
    public float openAnimationDuration = 3.6f;

    [Header("Elementos que deben aparecer después")]
    public GameObject RibbonTitle;
    public GameObject QuestionBox;
    public GameObject QuestionText;
    public GameObject Button1;
    public GameObject Button2;
    public GameObject Button3;
    public GameObject Button4;
    public GameObject FeedbackBox;
    public GameObject FeedbackText;

    private void OnEnable()
    {
        HideUIElements();
        StartCoroutine(OpenBookThenShowUI());
    }

    public void HideUIElements()
    {
        if (RibbonTitle != null) RibbonTitle.SetActive(false);
        if (QuestionBox != null) QuestionBox.SetActive(false);
        if (QuestionText != null) QuestionText.SetActive(false);
        if (Button1 != null) Button1.SetActive(false);
        if (Button2 != null) Button2.SetActive(false);
        if (Button3 != null) Button3.SetActive(false);
        if (Button4 != null) Button4.SetActive(false);
        if (FeedbackBox != null) FeedbackBox.SetActive(false);
        if (FeedbackText != null) FeedbackText.SetActive(false);
    }

    private IEnumerator OpenBookThenShowUI()
    {
        if (bookAnimator != null)
        {
            bookAnimator.Play(openAnimationName, 0, 0f);
        }

        yield return new WaitForSecondsRealtime(openAnimationDuration);

        if (RibbonTitle != null) RibbonTitle.SetActive(true);
        if (QuestionBox != null) QuestionBox.SetActive(true);
        if (QuestionText != null) QuestionText.SetActive(true);

        yield return new WaitForSecondsRealtime(0.08f);
        if (Button1 != null) Button1.SetActive(true);

        yield return new WaitForSecondsRealtime(0.08f);
        if (Button2 != null) Button2.SetActive(true);

        yield return new WaitForSecondsRealtime(0.08f);
        if (Button3 != null) Button3.SetActive(true);

        yield return new WaitForSecondsRealtime(0.08f);
        if (Button4 != null) Button4.SetActive(true);

        yield return new WaitForSecondsRealtime(0.05f);
        if (FeedbackBox != null) FeedbackBox.SetActive(true);
        if (FeedbackText != null) FeedbackText.SetActive(true);
    }
}