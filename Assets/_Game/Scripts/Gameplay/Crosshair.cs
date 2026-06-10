using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public AimSystem aimSystem;
    private SpriteRenderer _sprite;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        transform.position = aimSystem.aimPosition;
        _sprite.enabled = aimSystem.currentPhase == AimSystem.AimPhase.Phase2_Precision;
    }
}