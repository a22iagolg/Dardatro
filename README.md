# 🎯 Dart Roguelite — Arquitectura Unity

> Inspirado en Balatro. Roguelite por "manos", 2D, con sistema de comodines, tienda y progresión de escenarios.

---

## 1. Visión del loop de juego

```
[Inicio de nivel]
    └─> [Fase de mano]
            ├─> Lanzar dardos (N dardos por mano)
            ├─> Calcular puntuación (modificada por comodines activos)
            ├─> ¿Puntuación >= objetivo?
            │       ├─ SÍ → siguiente mano o pasar nivel
            │       └─ NO → perder mano (recurso)
            └─> ¿Manos agotadas? → Game Over
    └─> [Entre niveles: Tienda / Comodín]
    └─> [Jefe]
    └─> [Siguiente escenario]
```

---

## 2. Estructura de carpetas Unity

```
Assets/
├── _Game/
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── GameManager.cs          ← Estado global del run
│   │   │   ├── RunData.cs              ← ScriptableObject del run actual
│   │   │   ├── LevelConfig.cs          ← SO: objetivos, restricciones por nivel
│   │   │   └── EventBus.cs             ← Sistema de eventos desacoplado
│   │   │
│   │   ├── Gameplay/
│   │   │   ├── HandManager.cs          ← Gestión de manos y dardos disponibles
│   │   │   ├── DartLauncher.cs         ← Input y física del lanzamiento
│   │   │   ├── AimSystem.cs            ← Círculo de aim (shrink/expand loop)
│   │   │   ├── Target.cs               ← Diana: sectores, puntuación
│   │   │   ├── DartProjectile.cs       ← Dardo en vuelo + colisión
│   │   │   └── ScoreCalculator.cs      ← Aplica multiplicadores/comodines
│   │   │
│   │   ├── Jokers/                     ← Sistema de comodines (el corazón del roguelite)
│   │   │   ├── JokerBase.cs            ← Abstract/Interface base
│   │   │   ├── JokerData.cs            ← SO: nombre, icono, descripción, rareza
│   │   │   ├── JokerInventory.cs       ← Lista de comodines activos del run
│   │   │   └── Jokers/
│   │   │       ├── JokerDoubleHand.cs
│   │   │       ├── JokerSpeedDown.cs
│   │   │       ├── JokerCircuit.cs
│   │   │       └── ...
│   │   │
│   │   ├── Progression/
│   │   │   ├── RunManager.cs           ← Flujo del run: niveles, tienda, jefes
│   │   │   ├── ScenarioData.cs         ← SO: bareto, pub, competición...
│   │   │   ├── ShopManager.cs          ← Tienda entre niveles
│   │   │   └── BossEncounter.cs        ← Lógica de jefe
│   │   │
│   │   ├── Meta/
│   │   │   ├── SaveSystem.cs           ← Persistencia (unlocks, stats)
│   │   │   └── UnlockManager.cs        ← Qué comodines/escenarios están desbloqueados
│   │   │
│   │   └── UI/
│   │       ├── HUDController.cs
│   │       ├── ShopUI.cs
│   │       └── JokerCardUI.cs
│   │
│   ├── ScriptableObjects/
│   │   ├── Levels/
│   │   ├── Jokers/
│   │   └── Scenarios/
│   │
│   ├── Prefabs/
│   ├── Scenes/
│   │   ├── MainMenu
│   │   ├── Game
│   │   └── Shop
│   └── Art/
│
└── Plugins/
```

---

## 3. Sistemas clave y cómo se comunican

### EventBus (desacoplamiento total)

Evita que los sistemas se referencien directamente. Todo habla a través de eventos.

```csharp
// EventBus.cs — estático, simple
public static class EventBus
{
    public static event Action<DartHitData> OnDartHit;
    public static event Action<int> OnHandCompleted;       // puntos de la mano
    public static event Action OnHandFailed;
    public static event Action<JokerData> OnJokerAcquired;
    public static event Action OnLevelStarted;
    public static event Action OnRunEnded;

    public static void Publish_DartHit(DartHitData data) => OnDartHit?.Invoke(data);
    public static void Publish_HandCompleted(int score) => OnHandCompleted?.Invoke(score);
    // ...
}

// DartHitData — lo que sabe un impacto
public struct DartHitData
{
    public int basePoints;
    public bool isBullseye;
    public bool isWood;         // "dar a la madera"
    public int handIndex;       // qué mano lo lanzó
    public Vector2 hitPosition;
}
```

---

### AimSystem — el círculo

```csharp
public class AimSystem : MonoBehaviour
{
    [Header("Config")]
    public float maxRadius = 2f;
    public float minRadius = 0.1f;
    public float shrinkSpeed = 1f;     // modificable por comodines

    private float _currentRadius;
    private bool _shrinking = true;

    public float GetCurrentRadius() => _currentRadius;

    // El DartLauncher llama esto al lanzar
    public float ConsumeAccuracy()
    {
        float accuracy = 1f - (_currentRadius / maxRadius); // 0=malo, 1=perfecto
        ResetCycle();
        return accuracy;
    }

    void Update()
    {
        if (_shrinking)
        {
            _currentRadius -= shrinkSpeed * Time.deltaTime;
            if (_currentRadius <= minRadius) _shrinking = false;
        }
        else
        {
            _currentRadius = maxRadius; // reset instantáneo (o con lerp si quieres)
            _shrinking = true;
        }
        // Actualizar visual del círculo aquí
    }
}
```

---

### JokerBase — interfaz de comodines

```csharp
// Cada comodín implementa los hooks que le interesan
public abstract class JokerBase : ScriptableObject
{
    public JokerData data;

    // Hooks disponibles — override solo los necesarios
    public virtual void OnDartHit(ref DartHitData hit) { }
    public virtual void OnHandComplete(ref int totalScore, HandResult result) { }
    public virtual void OnHandStart(HandManager hand) { }
    public virtual void OnAimTick(ref float shrinkSpeed) { }   // para JokerSpeedDown
    public virtual void OnLevelStart(LevelConfig level) { }
}

// Ejemplo concreto
[CreateAssetMenu(menuName = "Jokers/SpeedDown")]
public class JokerSpeedDown : JokerBase
{
    public float speedMultiplier = 0.6f;

    public override void OnAimTick(ref float shrinkSpeed)
    {
        shrinkSpeed *= speedMultiplier;   // ralentiza el círculo
    }
}
```

El `JokerInventory` itera sobre todos los jokers activos y llama sus hooks en el momento correcto.

---

### HandManager — recurso central del roguelite

```csharp
public class HandManager : MonoBehaviour
{
    public int maxHands = 4;
    public int dartsPerHand = 3;

    private int _handsRemaining;
    private int _dartsInCurrentHand;

    public void StartHand()
    {
        _dartsInCurrentHand = dartsPerHand;
        // Aplicar jokers OnHandStart
        JokerInventory.Instance.TriggerHandStart(this);
        EventBus.Publish_HandStarted();
    }

    public void UseDart()
    {
        _dartsInCurrentHand--;
        if (_dartsInCurrentHand <= 0) EvaluateHand();
    }

    public void RetireDart() // mechanic de retirar dardos
    {
        _dartsInCurrentHand++;  // recuperas un lanzamiento
    }

    private void EvaluateHand()
    {
        int score = ScoreCalculator.Instance.Calculate();
        if (score >= LevelConfig.current.targetScore)
            EventBus.Publish_HandCompleted(score);
        else
        {
            _handsRemaining--;
            if (_handsRemaining <= 0) EventBus.Publish_RunEnded();
            else EventBus.Publish_HandFailed();
        }
    }
}
```

---

## 4. Flujo de un Run (RunManager)

```
RunManager arranca
    │
    ├─ Cargar ScenarioData (bareto, pub...)
    ├─ Generar secuencia: [nivel, nivel, nivel, tienda, nivel, nivel, JEFE]
    │
    └─ Loop:
        ├─ LevelConfig.current = siguiente nivel
        ├─ Cargar escena de juego
        ├─ Esperar EventBus.OnLevelCleared / OnRunEnded
        ├─ Si cleared → ¿es tienda? → ShopManager
        └─ Si RunEnded → Game Over screen
```

Los `ScenarioData` y `LevelConfig` son **ScriptableObjects**, editables sin código.

---

## 5. ScriptableObjects principales

```csharp
// LevelConfig.cs
[CreateAssetMenu(menuName = "Game/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    public int targetScore;
    public int handsAvailable;
    public int dartsPerHand;
    public List<LevelRestriction> restrictions;  // ej: "solo bullseye cuenta"
    public bool hasBoss;
}

// ScenarioData.cs
[CreateAssetMenu(menuName = "Game/Scenario")]
public class ScenarioData : ScriptableObject
{
    public string scenarioName;      // "Bar de barrio"
    public Sprite background;
    public AudioClip ambience;
    public List<LevelConfig> levels;
    public LevelConfig bossLevel;
    public int shopsBetweenLevels;
}
```

---

## 6. Roadmap de implementación (por fases)

### Fase 1 — Core loop jugable
- [ ] AimSystem (círculo shrink/expand)
- [ ] DartLauncher (input + disparo)
- [ ] Target con sectores y puntuación base
- [ ] HandManager (manos y dardos)
- [ ] ScoreCalculator básico
- [ ] HUD mínimo (puntos, manos restantes)

### Fase 2 — Roguelite skeleton
- [ ] RunManager + LevelConfig (SO)
- [ ] EventBus
- [ ] JokerBase + JokerInventory
- [ ] 3-4 jokers de prueba
- [ ] ShopManager básico (elegir 1 de 3)

### Fase 3 — Contenido y progresión
- [ ] ScenarioData (bareto, pub, competición)
- [ ] BossEncounter
- [ ] 10-15 jokers variados
- [ ] SaveSystem (unlocks meta)
- [ ] UI pulida

### Fase 4 — Juice y polish
- [ ] Feedback visual/sonoro en impactos
- [ ] Animaciones de dardos
- [ ] Tutorial + tiro asistido inicial
- [ ] Balanceo de dificultad

---

## 7. Comodines — catálogo inicial

| Nombre | Trigger | Efecto |
|---|---|---|
| Puntería lenta | OnAimTick | Reduce velocidad del círculo |
| Mano extra | OnLevelStart | +1 mano disponible |
| Dardo de propina | OnHandStart | +1 dardo esta mano |
| Diana doble | OnDartHit (bullseye x2) | x2 puntos si bullseye en manos distintas |
| Madera vale | OnDartHit (wood) | Dar a la madera da puntos en lugar de 0 |
| Par/Impar | OnHandComplete | x2 si puntuación es par |
| Circuito | OnLevelStart | Añade zona de puntos extra en posición random |
| Puntería rápida | OnAimTick + OnHandComplete | +velocidad pero x1.5 puntos |

---

## Notas finales

- **No uses singletons para todo.** GameManager puede serlo, pero ScoreCalculator y HandManager mejor como componentes en escena referenciados via ServiceLocator o Zenject si el proyecto crece.
- **Los jokers como ScriptableObjects** permiten crear contenido sin programar, ideal para iterar rápido en balance.
- **EventBus primero.** Es lo más importante para que el código no se enrede. Ponlo antes de escribir más gameplay.
- Cuando pases tu código existente, vemos qué encaja aquí y qué refactorizamos.