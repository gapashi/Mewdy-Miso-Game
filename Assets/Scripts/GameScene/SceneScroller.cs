using UnityEngine;
using UnityEngine.UI;

public class SceneScroller : MonoBehaviour
{
    public float slideDistance = 960f; 
    public float slideDuration = 0.5f;

    public Button rightArrow;
    public Button leftArrow;

    private bool isSliding = false;

    void Start()
    {
        rightArrow.gameObject.SetActive(true);
        leftArrow.gameObject.SetActive(false);

        rightArrow.onClick.AddListener(SlideRight);
        leftArrow.onClick.AddListener(SlideLeft);
    }

    void SlideRight()
    {
        if (isSliding) return;

        StartCoroutine(SlideScene(new Vector3(slideDistance, 0, 0)));

        rightArrow.gameObject.SetActive(false);
        leftArrow.gameObject.SetActive(true);
    }

    void SlideLeft()
    {
        if (isSliding) return;

        StartCoroutine(SlideScene(new Vector3(-slideDistance, 0, 0)));

        rightArrow.gameObject.SetActive(true);
        leftArrow.gameObject.SetActive(false);
    }

    System.Collections.IEnumerator SlideScene(Vector3 offset)
    {
        isSliding = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + offset;
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / slideDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isSliding = false;
    }
}
