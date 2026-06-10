using UnityEngine;

public class AimVisual : MonoBehaviour
{
    public AimSystem aimSystem;
    public Transform circleVisual;

    void Update()
    {
        circleVisual.position = aimSystem.GetCenter();

        float diameter = aimSystem.GetCurrentRadius() * 2f;
        circleVisual.localScale = new Vector3(diameter, diameter, 1f);
    }
}