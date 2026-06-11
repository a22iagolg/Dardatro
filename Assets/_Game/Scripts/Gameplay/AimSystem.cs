using UnityEngine;

public class AimSystem : MonoBehaviour
{
    public enum AimPhase
    {
        Phase1_Move,
        Phase2_Bar
    }

    public AimPhase currentPhase = AimPhase.Phase1_Move;

    [Header("Phase 1 - Crosshair")]
    public float trembleAmount = 0.15f;
    public float trembleSpeed = 6f;

    [Header("Phase 2 - Barra")]
    public float barSpeed = 2f;
    public float perfectZone = 0.15f;
    public float goodZone = 0.35f;

    // Output público
    public Vector2 aimPosition { get; private set; }
    public float barValue { get; private set; }
    public float phase2StartTime { get; private set; }

    // Calculadas en base a los valores del Inspector
    public bool isPerfectAim => Mathf.Abs(barValue - _zoneCenter) <= perfectZone / 2f;
    public bool isGoodAim => Mathf.Abs(barValue - _zoneCenter) <= goodZone / 2f && !isPerfectAim;

    // State privado
    private Vector2 _lockedPosition;
    private float _trembleTimer;
    private float _barDirection = 1f;
    private float _zoneCenter = 0.5f;


    void Update()
    {
        switch (currentPhase)
        {
            case AimPhase.Phase1_Move: Phase1(); break;
            case AimPhase.Phase2_Bar: Phase2(); break;
        }
    }

    void Phase1()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = new Vector2(mouse.x, mouse.y);

        _trembleTimer += Time.deltaTime * trembleSpeed;
        float noiseX = (Mathf.PerlinNoise(_trembleTimer, 0f) - 0.5f) * 2f;
        float noiseY = (Mathf.PerlinNoise(0f, _trembleTimer) - 0.5f) * 2f;

        aimPosition = mousePos + new Vector2(noiseX, noiseY) * trembleAmount;
    }

    void Phase2()
    {
        barValue += _barDirection * barSpeed * Time.deltaTime;

        if (barValue >= 1f) { barValue = 1f; _barDirection = -1f; }
        if (barValue <= 0f) { barValue = 0f; _barDirection = 1f; }
    }

    public void LockPosition()
    {
        _lockedPosition = aimPosition;
        currentPhase = AimPhase.Phase2_Bar;
        barValue = 0f;
        _barDirection = 1f;
        float margin = (goodZone / 2f) + 0.05f;
        _zoneCenter = Random.Range(margin, 1f - margin);
        phase2StartTime = Time.time;
    }

    public float GetBarAccuracy()
    {
        float dist = Mathf.Abs(barValue - _zoneCenter);
        float maxDist = Mathf.Max(_zoneCenter, 1f - _zoneCenter);

        if (dist <= perfectZone / 2f) return 0f;

        if (dist <= goodZone / 2f)
            return Mathf.InverseLerp(perfectZone / 2f, goodZone / 2f, dist) * 0.5f;

        return 0.5f + Mathf.InverseLerp(goodZone / 2f, maxDist, dist) * 0.5f;
    }
    public void ResetAim()
    {
        _zoneCenter = 0.5f;
        currentPhase = AimPhase.Phase1_Move;
        _trembleTimer = 0f;
        barValue = 0f;
    }

    public Vector2 GetLockedPosition() => _lockedPosition;
    public float GetZoneCenter() => _zoneCenter;

}