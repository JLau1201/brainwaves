using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelCover : MonoBehaviour
{
    [Header("Image")]
    [SerializeField] private Image coverImage;

    [Header("FillTime")]
    [SerializeField] private float fillTime;

    public void OpenCover() {
        StartCoroutine(FillWheelCover(1));
    }

    public void CloseCover() {
        StartCoroutine(FillWheelCover(-1));
    }

    private IEnumerator FillWheelCover(int dir) {
        float elapsedTime = 0f;
        float targetFill = (dir == 1) ? 0.5f : 1f;

        while (elapsedTime < fillTime) {
            elapsedTime += Time.deltaTime;
            coverImage.fillAmount = Mathf.Lerp(coverImage.fillAmount, targetFill, elapsedTime / fillTime);
            yield return null;
        }
    }
}
