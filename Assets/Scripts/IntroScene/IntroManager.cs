using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroManager : MonoBehaviour
{

    //variables to control de intro scene
    public Image fadePanel;
    public TextMeshProUGUI introText;
    public float fadeDuration = 2f;
    public float textDisplayDuration = 10f;
    public string nextSceneName = "GameScene";

    private void Start()
    {
        introText.alpha = 0f;
        fadePanel.color = new Color(0, 0, 0, 1f);
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        //fade from black
        yield return FadePanel(1f, 0f);

        //fade in text
        yield return FadeText(0f, 1f);

        //wait the text visible time
        yield return new WaitForSeconds(textDisplayDuration);

        //fade out text
        yield return FadeText(1f, 0f);

        //fade to black
        yield return FadePanel(0f, 1f);

        //load game scene
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadePanel(float startAlpha, float endAlpha)
    {
        float timer = 0f;
        Color color = fadePanel.color;

        while(timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
            fadePanel.color = new Color(color.r, color.g, color.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        fadePanel.color = new Color(color.r, color.g, color.b, endAlpha);
    }

    IEnumerator FadeText(float startAlpha, float endAlpha)
    {
        float timer = 0f;

        while(timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
            introText.alpha = alpha;
            timer += Time.deltaTime;
            yield return null;
        }

        introText.alpha = endAlpha;
    }
}
