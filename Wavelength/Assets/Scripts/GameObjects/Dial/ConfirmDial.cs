using UnityEngine;
using UnityEngine.UI;

public class ConfirmDial : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Transform hitMarker;

    private void Awake() {
        button.onClick.AddListener(() => {
            Collider2D hitCollider = Physics2D.OverlapPoint(hitMarker.position);
            if (hitCollider != null) {
                if (hitCollider.TryGetComponent<IWheelPoint>(out IWheelPoint wheelPoint)) {
                    int points = wheelPoint.GetPointValue();
                    Debug.Log(points);
                }
            }
        });
    }
}
