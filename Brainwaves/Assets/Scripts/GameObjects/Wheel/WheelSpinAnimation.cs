using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

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
        if (IsOwner) {
            SyncWheelRotationServerRpc(transform.rotation.eulerAngles);
        }
        if (NetworkManager.Singleton.IsHost) {
            TriggerOnWheelSpinFinishedServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SyncWheelRotationServerRpc(Vector3 rotation) {
        SyncWheelRotationClientRpc(rotation);
    }

    [ClientRpc]
    private void SyncWheelRotationClientRpc(Vector3 rotation) {
        if (IsOwner) return;
        transform.rotation = Quaternion.Euler(rotation);
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
        float spinSpeed = UnityEngine.Random.Range(3f, 10f);

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
