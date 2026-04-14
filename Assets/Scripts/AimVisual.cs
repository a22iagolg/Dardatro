using UnityEngine;

public class AimVisual : MonoBehaviour
{
    public AimController aim;
    public Transform circleVisual;

    void Update()
    {
        // posición
        circleVisual.position = aim.GetCenter();

        // tamaño
        float diameter = aim.GetCurrentRadius() * 2f;
        circleVisual.localScale = new Vector3(diameter, diameter, 1f);
    }
}