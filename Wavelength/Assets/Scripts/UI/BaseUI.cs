using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class BaseUI : NetworkBehaviour
{
    // Inherited base class for all UI scripts

    public void Show() {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
    }
    public void Hide() {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
    }
}
