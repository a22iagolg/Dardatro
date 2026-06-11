using UnityEngine;

/// <summary>
/// Joker de prueba: ralentiza la barra de aim al inicio de cada combate.
/// Restaura la velocidad original al terminar para no acumularse entre combates.
/// Para crear el asset: click derecho → Create/Game/Jokers/SlowBar
/// </summary>
[CreateAssetMenu(menuName = "Game/Jokers/SlowBar")]
public class JokerSlowBar : JokerBase
{
    [Header("Config")]
    public float barSpeedMultiplier = 0.5f;

    private AimSystem _aimSystem;
    private float     _originalBarSpeed;

    public override void OnCombatStart(JokerContext context)
    {
        _aimSystem = Object.FindAnyObjectByType<AimSystem>();
        if (_aimSystem == null) return;

        _originalBarSpeed   = _aimSystem.barSpeed;
        _aimSystem.barSpeed = _originalBarSpeed * barSpeedMultiplier;
        Debug.Log($"[JokerSlowBar] Barra x{barSpeedMultiplier} → velocidad: {_aimSystem.barSpeed}");
    }

    public override void OnCombatCleared(JokerContext context) => RestoreSpeed();
    public override void OnGameOver(JokerContext context)      => RestoreSpeed();

    void RestoreSpeed()
    {
        if (_aimSystem == null) return;
        _aimSystem.barSpeed = _originalBarSpeed;
        Debug.Log($"[JokerSlowBar] Velocidad restaurada: {_originalBarSpeed}");
    }
}