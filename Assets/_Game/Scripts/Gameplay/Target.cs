using UnityEngine;

/// <summary>
/// Diana con hit detection por ángulo + distancia.
/// Radios y sectores vienen del TargetData asignado en LevelConfig.
/// Llama a TargetData.InitForCombat() al iniciar cada combate.
/// </summary>
public class Target : MonoBehaviour
{
    [Header("Datos")]
    public TargetData data;

    [Header("Referencias")]
    public TargetVisual visual;  // Opcional — si está asignado, redibuja al iniciar combate

    void OnEnable()
    {
        EventBus.OnCombatStarted += OnCombatStarted;
    }

    void OnDisable()
    {
        EventBus.OnCombatStarted -= OnCombatStarted;
    }

    void OnCombatStarted()
    {
        if (data == null) return;
        data.InitForCombat();
        visual?.Redraw(data);
    }

    /// <summary>
    /// Evalúa un punto de impacto y devuelve el ScoreResult completo.
    /// </summary>
    public ScoreResult Evaluate(Vector2 hitPoint)
    {
        if (data == null)
        {
            Debug.LogWarning("[Target] No hay TargetData asignado");
            return new ScoreResult { isWood = true, hitPosition = hitPoint };
        }

        Vector2 delta = hitPoint - (Vector2)transform.position;
        float   dist  = delta.magnitude;
        // Atan2(x,y) para que 0° sea arriba
        float   angle = Mathf.Atan2(delta.x, delta.y) * Mathf.Rad2Deg;

        ScoreResult result = new ScoreResult();
        result.hitPosition = hitPoint;
        result.angle       = angle;
        result.distance    = dist;

        // Bullseye interior (50pts)
        if (dist <= data.bullseyeInnerRadius)
        {
            result.points     = 50;
            result.isBullseye = true;
            result.zone       = HitZone.BullseyeInner;
            return result;
        }

        // Bullseye exterior (25pts)
        if (dist <= data.bullseyeOuterRadius)
        {
            result.points = 25;
            result.zone   = HitZone.BullseyeOuter;
            return result;
        }

        // Fuera de la diana
        if (dist > data.doubleOuterRadius)
        {
            result.points = 0;
            result.isWood = true;
            result.zone   = HitZone.Wood;
            return result;
        }

        // Dentro — determinar sector
        TargetSector sector = data.GetSectorAtAngle(angle);
        if (sector == null)
        {
            result.isWood = true;
            result.zone   = HitZone.Wood;
            return result;
        }

        result.sector      = sector;
        result.baseValue   = sector.baseValue;
        result.specialType = sector.specialType;

        // Determinar zona: triple, doble o simple
        if (dist >= data.tripleInnerRadius && dist <= data.tripleOuterRadius)
        {
            result.zone       = HitZone.Triple;
            result.multiplier = 3;
            result.points     = sector.baseValue * 3;
        }
        else if (dist >= data.doubleInnerRadius && dist <= data.doubleOuterRadius)
        {
            result.zone       = HitZone.Double;
            result.multiplier = 2;
            result.points     = sector.baseValue * 2;
        }
        else
        {
            result.zone       = HitZone.Single;
            result.multiplier = 1;
            result.points     = sector.baseValue;
        }

        // Sector especial sobreescribe puntos
        if (sector.specialType == SectorSpecialType.Penalty)
        {
            result.points    = -Mathf.Abs(result.points);
            result.isPenalty = true;
        }

        return result;
    }

    /// <summary>
    /// Asigna un nuevo TargetData en runtime (llamado por RunManager al cambiar de combate).
    /// </summary>
    public void SetTargetData(TargetData newData)
    {
        data = newData;
        data.InitForCombat();
        visual?.Redraw(data);
    }
}

public struct ScoreResult
{
    public int              points;
    public int              baseValue;
    public int              multiplier;
    public bool             isBullseye;
    public bool             isWood;
    public bool             isPenalty;
    public HitZone          zone;
    public float            angle;
    public float            distance;
    public Vector2          hitPosition;
    public TargetSector     sector;
    public SectorSpecialType specialType;
}

public enum HitZone
{
    Wood,
    Single,
    Double,
    Triple,
    BullseyeOuter,
    BullseyeInner
}