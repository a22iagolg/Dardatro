using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Dibuja la diana en runtime basándose en los datos del TargetData.
/// No necesitas colocar sprites a mano — este componente genera toda la geometría.
/// Añádelo al mismo GameObject que Target y asígnalo en Target.visual.
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TargetVisual : MonoBehaviour
{
    [Header("Materiales")]
    public Material matSingleA;       // Color alterno A para sectores simples
    public Material matSingleB;       // Color alterno B para sectores simples
    public Material matDouble;        // Anillo doble
    public Material matTriple;        // Anillo triple
    public Material matBullseyeOuter; // Bullseye exterior (verde)
    public Material matBullseyeInner; // Bullseye interior (rojo)
    public Material matPenalty;       // Sector de penalización (boss)
    public Material matLoseDart;      // Sector quita dardo (boss)
    public Material matBonus;         // Sector bonus moneda (boss)
    public Material matCurse;         // Sector maldición (boss)

    [Header("Líneas divisorias")]
    public Material matDivider;           // Unlit/Color negro o blanco
    [Range(0.01f, 2f)]
    public float dividerWidth = 0.03f; // Grosor en unidades de mundo

    [Header("Config visual")]
    public int meshResolution = 32;    // Subdivisiones por sector para suavizar arcos
    public float labelOffset = 0.85f; // Distancia del centro donde aparecen los números

    // GameObjects generados en runtime
    private List<GameObject> _segments = new List<GameObject>();

    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshFilter>().mesh = null;
    }

    /// <summary>
    /// Regenera todo el visual. Llamado por Target.SetTargetData() y Target.OnCombatStarted().
    /// </summary>
    public void Redraw(TargetData data)
    {
        ClearSegments();

        List<TargetSector> sectors = data.GetRuntimeSectors();
        int count = sectors.Count;
        float sectorSize = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float startAngle = i * sectorSize - sectorSize / 2f;
            float endAngle = startAngle + sectorSize;
            TargetSector sector = sectors[i];

            Material matSimple = (i % 2 == 0) ? matSingleA : matSingleB;
            if (sector.specialType == SectorSpecialType.Penalty) matSimple = matPenalty;
            if (sector.specialType == SectorSpecialType.LoseDart) matSimple = matLoseDart;
            if (sector.specialType == SectorSpecialType.Bonus) matSimple = matBonus;
            if (sector.specialType == SectorSpecialType.Curse) matSimple = matCurse;

            // Zona simple: bullseyeOuter → tripleInner y tripleOuter → doubleInner
            CreateRing(data.bullseyeOuterRadius, data.tripleInnerRadius, startAngle, endAngle, matSimple, $"Simple_{i}");
            CreateRing(data.tripleOuterRadius, data.doubleInnerRadius, startAngle, endAngle, matSimple, $"Simple2_{i}");

            // Triple y doble
            CreateRing(data.tripleInnerRadius, data.tripleOuterRadius, startAngle, endAngle, matTriple, $"Triple_{i}");
            CreateRing(data.doubleInnerRadius, data.doubleOuterRadius, startAngle, endAngle, matDouble, $"Double_{i}");

            // Label numérico entre triple y doble
            float labelRadius = (data.tripleOuterRadius + data.doubleInnerRadius) / 2f;
            CreateLabel(sector, startAngle + sectorSize / 2f, labelRadius);
        }

        // Bullseyes
        CreateDisc(0f, data.bullseyeOuterRadius, matBullseyeOuter, "BullseyeOuter");
        CreateDisc(0f, data.bullseyeInnerRadius, matBullseyeInner, "BullseyeInner");

        // Líneas divisorias entre sectores
        CreateDividers(data, count, sectorSize);

        // Línea exterior de la diana
        CreateRing(data.doubleOuterRadius, data.doubleOuterRadius + 0.04f, 0f, 360f, matDivider, "BorderOuter");
    }

    // ——— Divisores ———

    void CreateDividers(TargetData data, int count, float sectorSize)
    {
        if (matDivider == null) return;

        for (int i = 0; i < count; i++)
        {
            // Ángulo del borde entre sector i y sector i+1
            float angleDeg = i * sectorSize - sectorSize / 2f;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            float sin = Mathf.Sin(angleRad);
            float cos = Mathf.Cos(angleRad);

            // Línea desde bullseyeOuter hasta doubleOuter
            Vector3 inner = new Vector3(sin * data.bullseyeOuterRadius, cos * data.bullseyeOuterRadius, -0.01f);
            Vector3 outer = new Vector3(sin * data.doubleOuterRadius, cos * data.doubleOuterRadius, -0.01f);

            CreateLine(inner, outer, dividerWidth, $"Divider_{i}");
        }
    }

    void CreateLine(Vector3 from, Vector3 to, float width, string name)
    {
        Vector3 dir = (to - from).normalized;
        Vector3 perp = new Vector3(-dir.y, dir.x, 0f) * (width / 2f);

        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[]
        {
            from - perp, from + perp,
            to   - perp, to   + perp
        };
        mesh.triangles = new int[] { 0, 2, 1, 1, 2, 3 };
        mesh.RecalculateNormals();

        GameObject go = new GameObject(name);
        go.transform.SetParent(transform, false);
        go.AddComponent<MeshFilter>().mesh = mesh;
        go.AddComponent<MeshRenderer>().material = matDivider;
        _segments.Add(go);
    }

    // ——— Helpers de geometría ———

    void CreateRing(float innerR, float outerR, float startDeg, float endDeg, Material mat, string name)
    {
        if (mat == null) return;
        Mesh mesh = BuildArcMesh(innerR, outerR, startDeg, endDeg, meshResolution);
        GameObject go = new GameObject(name);
        go.transform.SetParent(transform, false);
        go.AddComponent<MeshFilter>().mesh = mesh;
        go.AddComponent<MeshRenderer>().material = mat;
        _segments.Add(go);
    }

    void CreateDisc(float innerR, float outerR, Material mat, string name)
    {
        if (mat == null) return;
        Mesh mesh = BuildArcMesh(innerR, outerR, 0f, 360f, meshResolution * 4);
        GameObject go = new GameObject(name);
        go.transform.SetParent(transform, false);
        go.AddComponent<MeshFilter>().mesh = mesh;
        go.AddComponent<MeshRenderer>().material = mat;
        _segments.Add(go);
    }

    void CreateLabel(TargetSector sector, float angleDeg, float radius)
    {
        string text = sector.specialType == SectorSpecialType.Hidden
            ? "???" : sector.baseValue.ToString();

        GameObject go = new GameObject($"Label_{sector.label}");
        go.transform.SetParent(transform, false);

        float rad = angleDeg * Mathf.Deg2Rad;
        go.transform.localPosition = new Vector3(
            Mathf.Sin(rad) * radius,
            Mathf.Cos(rad) * radius,
            -0.02f
        );

        TextMesh tm = go.AddComponent<TextMesh>();
        tm.text = text;
        tm.fontSize = 24;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = Color.white;
        tm.characterSize = 0.15f;

        _segments.Add(go);
    }

    Mesh BuildArcMesh(float innerR, float outerR, float startDeg, float endDeg, int steps)
    {
        Mesh mesh = new Mesh();
        Vector3[] verts = new Vector3[(steps + 1) * 2];
        int[] tris = new int[steps * 6];

        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            float deg = Mathf.Lerp(startDeg, endDeg, t);
            float rad = deg * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            verts[i * 2] = new Vector3(sin * innerR, cos * innerR, 0f);
            verts[i * 2 + 1] = new Vector3(sin * outerR, cos * outerR, 0f);

            if (i < steps)
            {
                int b = i * 6, v = i * 2;
                tris[b] = v; tris[b + 1] = v + 2; tris[b + 2] = v + 1;
                tris[b + 3] = v + 1; tris[b + 4] = v + 2; tris[b + 5] = v + 3;
            }
        }

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        return mesh;
    }

    void ClearSegments()
    {
        foreach (var go in _segments)
            if (go != null) Destroy(go);
        _segments.Clear();
    }
}