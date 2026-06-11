using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Define la estructura completa de una diana.
/// Cada LevelConfig referenciará un TargetData.
/// Los valores de los sectores se pueden shufflear por combate para frescura.
/// </summary>
[CreateAssetMenu(menuName = "Game/TargetData")]
public class TargetData : ScriptableObject
{
    [Header("Radios (en unidades de mundo)")]
    public float bullseyeInnerRadius = 0.15f;  // Bullseye rojo interior (50pts)
    public float bullseyeOuterRadius = 0.30f;  // Bullseye verde exterior (25pts)
    public float tripleInnerRadius   = 0.55f;  // Inicio anillo triple
    public float tripleOuterRadius   = 0.70f;  // Fin anillo triple
    public float doubleInnerRadius   = 1.10f;  // Inicio anillo doble
    public float doubleOuterRadius   = 1.25f;  // Fin anillo doble (borde exterior diana)

    [Header("Sectores (define los valores base en el Inspector)")]
    public List<TargetSector> sectors;

    [Header("Shuffle")]
    [Tooltip("Si true, los valores de los sectores se reordenan aleatoriamente al iniciar cada combate")]
    public bool shuffleSectorsOnCombatStart = false;

    [Header("Mecánica especial (boss)")]
    public TargetSpecialMechanic specialMechanic = TargetSpecialMechanic.None;

    // Lista activa en runtime — puede ser shuffleada sin tocar el asset original
    [System.NonSerialized]
    private List<TargetSector> _runtimeSectors;

    /// <summary>
    /// Llamado por Target al iniciar cada combate.
    /// Si shuffleSectorsOnCombatStart está activo, reordena los valores
    /// manteniendo las posiciones visuales fijas (solo cambian los números).
    /// </summary>
    public void InitForCombat()
    {
        // Copia profunda para no tocar el asset original
        _runtimeSectors = new List<TargetSector>();
        foreach (var s in sectors)
            _runtimeSectors.Add(new TargetSector { label = s.label, baseValue = s.baseValue, specialType = s.specialType });

        if (!shuffleSectorsOnCombatStart) return;

        // Shuffleamos solo los valores, no los specialTypes ni labels visuales
        List<int> values = new List<int>();
        foreach (var s in _runtimeSectors)
            values.Add(s.baseValue);

        // Fisher-Yates shuffle
        for (int i = values.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = values[i];
            values[i] = values[j];
            values[j] = tmp;
        }

        for (int i = 0; i < _runtimeSectors.Count; i++)
            _runtimeSectors[i].baseValue = values[i];
    }

    /// <summary>
    /// Devuelve el sector al que pertenece un ángulo dado (0-360).
    /// Usa _runtimeSectors si están inicializados, si no cae a sectors.
    /// </summary>
    public TargetSector GetSectorAtAngle(float angleDeg)
    {
        List<TargetSector> list = (_runtimeSectors != null && _runtimeSectors.Count > 0)
            ? _runtimeSectors : sectors;

        if (list == null || list.Count == 0) return null;

        float sectorSize = 360f / list.Count;
        angleDeg = (angleDeg % 360f + 360f) % 360f;
        float adjusted = (angleDeg + sectorSize / 2f) % 360f;
        int index = Mathf.Clamp(Mathf.FloorToInt(adjusted / sectorSize), 0, list.Count - 1);

        return list[index];
    }

    /// <summary>
    /// Devuelve los sectores activos en runtime (para que TargetVisual pueda dibujarlos).
    /// </summary>
    public List<TargetSector> GetRuntimeSectors()
    {
        return (_runtimeSectors != null && _runtimeSectors.Count > 0) ? _runtimeSectors : sectors;
    }
}

[System.Serializable]
public class TargetSector
{
    public string           label;
    public int              baseValue;
    public SectorSpecialType specialType = SectorSpecialType.Normal;
}

public enum SectorSpecialType
{
    Normal,
    Hidden,     // Valor oculto ??? — boss
    Penalty,    // Resta puntos — boss
    LoseDart,   // Quita un dardo — boss
    Bonus,      // Da moneda extra — boss
    Curse,      // Pierde una mano — boss
}

public enum TargetSpecialMechanic
{
    None,
    Shadow,         // Boss 1: sombra que invalida zona
    HiddenValues,   // Boss 2: todos los valores son ???
    Rotating,       // Boss 3: diana rota mientras apuntas
    ScoreInverter,  // Boss 4: zona que invierte score acumulado
    ShiftingValues, // Boss 5: sectores cambian valor cada dardo
}