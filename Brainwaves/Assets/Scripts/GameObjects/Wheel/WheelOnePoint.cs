using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelOnePoint : MonoBehaviour, IWheelPoint
{
    public int GetPointValue() {
        return 1;
    }
}
