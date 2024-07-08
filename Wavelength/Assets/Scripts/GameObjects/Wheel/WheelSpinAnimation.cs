using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class WheelSpinAnimation : NetworkBehaviour
{
    [SerializeField] private float spinSlowDownTime;
    [SerializeField] private Button spinButton;

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
        animator.enabled = false;

        spinButton.onClick.AddListener(() => {
            ChangeOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
            float spinSpeed = Random.Range(1, 5);
            animator.speed = spinSpeed;
            animator.enabled = true;
            StartCoroutine(SpinWheel());
        });
    }

    private IEnumerator SpinWheel() {
        float elapsedTime = 0f;
        float startSpeed = animator.speed;

        while (elapsedTime < spinSlowDownTime) {
            animator.speed = Mathf.Lerp(startSpeed, 0f, elapsedTime / spinSlowDownTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        animator.enabled = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeOwnershipServerRpc(ulong clientId) {
        GetComponent<NetworkObject>().ChangeOwnership(clientId);
    }
}
