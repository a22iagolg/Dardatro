using UnityEngine;

public class AimSystem : MonoBehaviour
{
    public enum AimPhase
    {
        Phase1_Select,
        Phase2_Precision
    }

    public AimPhase currentPhase = AimPhase.Phase1_Select;

    [Header("Fase 1")]
    public float maxRadius = 7.0f;
    public float minRadius = 0.3f;
    public float sizeSpeed = 1f;

    [Header("Fase 2")]
    public float jumpInterval = 0.4f;
    public float moveSpeed = 8f;

    [Header("Config")]
    public float minTimeBeforeShoot = 0.1f;

    // State
    private float _currentRadius;
    private float _sizeTimer;
    private Vector2 _currentCenter;
    private Vector2 _lockedCenter;
    private float _jumpTimer;
    private Vector2 _targetPos;

    public Vector2 aimPosition { get; private set; }
    public float phase2StartTime { get; private set; }

    void Start()
    {
        _currentRadius = maxRadius;
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

    void Phase1()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _currentCenter = new Vector2(mouse.x, mouse.y);

        _sizeTimer += Time.deltaTime * sizeSpeed;
        float oscillation = Mathf.Sin(_sizeTimer * Mathf.PI);
        _currentRadius = Mathf.Lerp(minRadius, maxRadius, (oscillation + 1f) / 2f);

        aimPosition = _currentCenter;
    }

    void Phase2()
    {
        _jumpTimer -= Time.deltaTime;

        if (_jumpTimer <= 0)
        {
            _jumpTimer = jumpInterval;
            PickNewTarget();
        }

        aimPosition = Vector2.Lerp(aimPosition, _targetPos, Time.deltaTime * moveSpeed);
    }

    public void StartPhase2()
    {
        _lockedCenter = _currentCenter;
        currentPhase = AimPhase.Phase2_Precision;
        PickNewTarget();
        _jumpTimer = jumpInterval;
        aimPosition = _lockedCenter;
        phase2StartTime = Time.time;
    }

    void PickNewTarget()
    {
        _targetPos = _lockedCenter + Random.insideUnitCircle * _currentRadius;
    }

    public void ResetAim()
    {
        currentPhase = AimPhase.Phase1_Select;
        _sizeTimer = 0f;
        _currentRadius = maxRadius;
    }

    public float GetCurrentRadius() => _currentRadius;
    public Vector2 GetCenter() => currentPhase == AimPhase.Phase1_Select ? _currentCenter : _lockedCenter;
}