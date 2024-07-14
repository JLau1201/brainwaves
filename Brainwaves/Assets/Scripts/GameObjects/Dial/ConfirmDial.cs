using UnityEngine;
using UnityEngine.UI;

public class ConfirmDial : MonoBehaviour
{
    [SerializeField] private Transform hitMarker;

    public int GetGuessScore() {
        Collider2D hitCollider = Physics2D.OverlapPoint(hitMarker.position);
        if (hitCollider != null) {
            if (hitCollider.TryGetComponent<IWheelPoint>(out IWheelPoint wheelPoint)) {
                return wheelPoint.GetPointValue();
            }
        }

        return 0;
    }
}
