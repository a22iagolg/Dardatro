using UnityEngine;

public class AimBarVisual : MonoBehaviour
{
    [Header("Referencias")]
    public AimSystem aimSystem;
    public SpriteRenderer barBackground;
    public SpriteRenderer goodZoneVisual;
    public SpriteRenderer perfectZoneVisual;
    public SpriteRenderer indicator;

    [Header("Config")]
    public float barHeight = 1.5f;

    void Start()
    {
    }

    void UpdateZones()
    {
        // Escalar GoodZone y PerfectZone según los valores del AimSystem
        float bgHeight = barBackground.bounds.size.y;

        SetZoneHeight(goodZoneVisual, aimSystem.goodZone * bgHeight);
        SetZoneHeight(perfectZoneVisual, aimSystem.perfectZone * bgHeight);

        // Centrar ambas zonas en el Background
        goodZoneVisual.transform.localPosition = Vector3.zero;
        perfectZoneVisual.transform.localPosition = Vector3.zero;
    }

    void SetZoneHeight(SpriteRenderer sr, float targetHeight)
    {
        float currentHeight = sr.bounds.size.y;
        float scale = targetHeight / currentHeight;
        Vector3 s = sr.transform.localScale;
        sr.transform.localScale = new Vector3(s.x, s.y * scale, s.z);
    }

    void Update()
    {
        bool isPhase2 = aimSystem.currentPhase == AimSystem.AimPhase.Phase2_Bar;
        barBackground.enabled = isPhase2;
        goodZoneVisual.enabled = isPhase2;
        perfectZoneVisual.enabled = isPhase2;
        indicator.enabled = isPhase2;

        if (!isPhase2) return;

        // Flota sobre el crosshair bloqueado
        Vector2 basePos = aimSystem.GetLockedPosition();
        transform.position = new Vector3(basePos.x + 0.4f, basePos.y, 0f);

        // Mover indicador
        float yOffset = Mathf.Lerp(-barHeight / 2f, barHeight / 2f, aimSystem.barValue);
        indicator.transform.localPosition = new Vector3(0f, yOffset, 0f);

        // Color del indicador según zona
        float accuracy = aimSystem.GetBarAccuracy();
        if (accuracy == 0f)
            indicator.color = Color.red;
        else if (accuracy <= 0.3f)
            indicator.color = new Color(1f, 0.5f, 0f);
        else
            indicator.color = Color.cyan;

        // Posicionar zonas según _zoneCenter aleatorio
        float zoneCenterOffset = Mathf.Lerp(-barHeight / 2f, barHeight / 2f, aimSystem.GetZoneCenter());
        goodZoneVisual.transform.localPosition = new Vector3(0f, zoneCenterOffset, 0f);
        perfectZoneVisual.transform.localPosition = new Vector3(0f, zoneCenterOffset, 0f);
    }
}