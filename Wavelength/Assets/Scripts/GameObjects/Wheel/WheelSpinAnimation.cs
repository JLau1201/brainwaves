using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using System;

public class WheelSpinAnimation : NetworkBehaviour
{
    public event EventHandler OnWheelSpinFinished;

    [SerializeField] private float spinSlowDownTime;
    [SerializeField] private Button spinButton;

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
        animator.enabled = false;

        spinButton.onClick.AddListener(() => {
            ChangeOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
            SpinWheelServerRpc();
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
        if (NetworkManager.Singleton.IsHost) {
            TriggerOnWheelSpinFinishedServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TriggerOnWheelSpinFinishedServerRpc() {
        OnWheelSpinFinished?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeOwnershipServerRpc(ulong clientId) {
        GetComponent<NetworkObject>().ChangeOwnership(clientId);
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpinWheelServerRpc() {
        float spinSpeed = UnityEngine.Random.Range(1, 5);

        SpinWheelClientRpc(spinSpeed);
    }

    [ClientRpc]
    private void SpinWheelClientRpc(float spinSpeed) {
        spinButton.enabled = false;
        animator.speed = spinSpeed;
        animator.enabled = true;
        StartCoroutine(SpinWheel());
    }
   
}
