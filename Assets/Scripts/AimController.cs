using UnityEngine;

public class AimController : MonoBehaviour
{
    public enum AimPhase
    {
        Phase1_Select,
        Phase2_Precision
    }

    public AimPhase currentPhase = AimPhase.Phase1_Select;

    [Header("Fase 1 (posición + tamaño)")]
    public float maxRadius = 7.0f;
    public float minRadius = 0.3f;
    public float sizeSpeed = 1f;

    private float currentRadius;
    private float sizeTimer;

    private Vector2 currentCenter; // sigue al ratón
    private Vector2 lockedCenter;  // se fija tras click
    public float minTimeBeforeShoot = 0.1f;
    public float phase2StartTime;
    [Header("Fase 2 (random)")]
    public float jumpInterval = 0.4f;
    public float moveSpeed = 8f;

    private float jumpTimer;
    private Vector2 targetPos;

    [Header("Output")]
    public Vector2 aimPosition;

    void Start()
    {
        currentRadius = maxRadius;
    }

    void Update()
    {
        switch (currentPhase)
        {
            case AimPhase.Phase1_Select:
                Phase1();
                break;

            case AimPhase.Phase2_Precision:
                Phase2();
                break;
        }
    }

    // 🟢 FASE 1: seguir ratón + cambiar tamaño
    void Phase1()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentCenter = new Vector2(mouse.x, mouse.y);

        sizeTimer += Time.deltaTime * sizeSpeed;

        float oscillation = Mathf.Sin(sizeTimer * Mathf.PI);

        currentRadius = Mathf.Lerp(minRadius, maxRadius, (oscillation + 1f) / 2f);
        aimPosition = currentCenter;
    }

    // 🔴 FASE 2: random dentro del círculo
    void Phase2()
    {
        jumpTimer -= Time.deltaTime;

        if (jumpTimer <= 0)
        {
            jumpTimer = jumpInterval;
            PickNewTarget();
        }

        aimPosition = Vector2.Lerp(aimPosition, targetPos, Time.deltaTime * moveSpeed);
    }

    public void StartPhase2()
    {
        lockedCenter = currentCenter;
        currentPhase = AimPhase.Phase2_Precision;

        PickNewTarget();
        jumpTimer = jumpInterval;

        aimPosition = lockedCenter;

        phase2StartTime = Time.time;
    }

    void PickNewTarget()
    {
        targetPos = lockedCenter + Random.insideUnitCircle * currentRadius;
    }

    public void ResetAim()
    {
        currentPhase = AimPhase.Phase1_Select;
        sizeTimer = 0f;
        currentRadius = maxRadius;
    }

    public float GetCurrentRadius()
    {
        return currentRadius;
    }

    public Vector2 GetCenter()
    {
        return currentPhase == AimPhase.Phase1_Select ? currentCenter : lockedCenter;
    }
}