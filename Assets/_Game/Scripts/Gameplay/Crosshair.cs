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
        // Visible siempre, solo se oculta si no hay fase activa
        _sprite.enabled = true;
    }
}