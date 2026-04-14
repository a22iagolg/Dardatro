using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public AimController aim;

    void Update()
    {
        transform.position = aim.aimPosition;
    }
}