using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class IrisTransition : MonoBehaviour
{
    public RectTransform irisMask;
    public float duration = 0.5f;
    public bool isClosing = true; // Bắt đầu với trạng thái đóng

    void Start()
    {
        StartCoroutine(RunIrisSequence());
    }

    IEnumerator RunIrisSequence()
    {
        yield return StartCoroutine(IrisEffect(isClosing ? 3f : 0f, isClosing ? 0f : 3f));

        isClosing = !isClosing;

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(IrisEffect(isClosing ? 3f : 0f, isClosing ? 0f : 3f));

        GameManager.Instance.ChangeState(GameState.Playing);
        gameObject.SetActive(false);
    }

    IEnumerator IrisEffect(float startSize, float endSize)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float scale = Mathf.Lerp(startSize, endSize, t / duration);
            irisMask.localScale = new Vector3(scale, scale, 1);
            yield return null;
        }
        irisMask.localScale = new Vector3(endSize, endSize, 1);
    }
}