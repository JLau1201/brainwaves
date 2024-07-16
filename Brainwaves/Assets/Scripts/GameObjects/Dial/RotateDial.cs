using Unity.Netcode;
using UnityEngine;

public class RotateDial : MonoBehaviour
{

    [Header("Rotation")]
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float rotationalBounds = 30;

    private Collider2D dialCollider;
    private bool isHoldingDial = false;
    private bool canSpin = false;

    private void Awake() {
        GameInputs.Instance.OnLeftMouseDown += GameInputs_OnLeftMouseDown;
        GameInputs.Instance.OnLeftMouseRelease += GameInputs_OnLeftMouseRelease;

        dialCollider = GetComponentInChildren<Collider2D>();
    }

    private void GameInputs_OnLeftMouseDown(object sender, System.EventArgs e) {
        // Check if mouse click is on the dial
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] colliders = Physics2D.OverlapPointAll(mousePos);
        foreach(Collider2D collider in colliders) {
            if (dialCollider == collider) {
                isHoldingDial = true;
                break;
            }
        }
    }

    private void GameInputs_OnLeftMouseRelease(object sender, System.EventArgs e) {
        isHoldingDial = false;
    }

    private void Update() {
        if (isHoldingDial && canSpin) {
            RotateWithMouse();
        } else if(GameInputs.Instance.GetKeyRotate().x != 0 && canSpin){
            RotateWithKeys();
        }
    }

    public void SetCanSpin(bool can) {
        canSpin = can;
    }

    private void RotateWithMouse() {
        // Get the direction to the mouse
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - transform.position;

        // Calculate angle from dial to mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < -90) {
            angle += 360;
        }
        ComputeRotation(angle);
    }

    private void RotateWithKeys() {
        float rotInput = GameInputs.Instance.GetKeyRotate().x;

        float angle = transform.rotation.eulerAngles.z - rotInput * rotateSpeed * Time.deltaTime;
        ComputeRotation(angle);
    }

    private void ComputeRotation(float angle) {
        // Keep within the rotational bounds
        float clampedAngle = Mathf.Clamp(angle, rotationalBounds, 180 - rotationalBounds);
        // Rotate the z axis
        Vector3 newRotation = new Vector3(0f, 0f, clampedAngle);
        transform.rotation = Quaternion.Euler(newRotation);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeOwnershipServerRpc(ulong clientId) {
        GetComponent<NetworkObject>().ChangeOwnership(clientId);
    }

    public void ChangeOwnership(ulong clientId) {
        ChangeOwnershipServerRpc(clientId);
    }
}
