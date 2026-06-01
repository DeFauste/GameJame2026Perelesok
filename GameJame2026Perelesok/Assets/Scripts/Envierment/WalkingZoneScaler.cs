using UnityEngine;

public class WalkingZoneScaler : MonoBehaviour
{
    public void CahngeScale(float scale)
    {
        transform.localScale = (Vector3.up + Vector3.right) * scale;
    }

    public void ChangeScalesConst(Vector3 scales)
    {
        transform.localScale = scales * 2;
    }
}
