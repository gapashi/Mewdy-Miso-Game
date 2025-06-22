using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingSceneManager : MonoBehaviour
{
    public Slider loadingBar;
    public string nextSceneName = "IntroScene";
    public float fillDuration = 3f;

    private float timer = 0f;

    void Start()
    {
        if (loadingBar != null)
            loadingBar.value = 0f;

        StartCoroutine(AnimateLoadingBar());
    }

    IEnumerator AnimateLoadingBar()
    {
        while (timer < fillDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / fillDuration);

            if (loadingBar != null)
                loadingBar.value = progress;

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(nextSceneName);
    }
}
