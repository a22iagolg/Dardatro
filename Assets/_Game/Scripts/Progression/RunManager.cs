using UnityEngine;
using System.Collections.Generic;

public class RunManager : MonoBehaviour
{
    [Header("Escenarios del run (en orden)")]
    public List<ScenarioData> scenarios;

    [Header("Referencias")]
    public HandManager handManager;
    public ScoreCalculator scoreCalculator;
    public DartLauncher dartLauncher;
    public Target target;           // Para asignar TargetData por combate

    private int _currentScenarioIndex = 0;
    private ScenarioData _currentScenario;
    private int _currentCombatIndex = 0; // 0=enemy1, 1=enemy2, 2=enemy3, 3=boss

    public static LevelConfig CurrentLevel { get; private set; }

    void OnEnable()
    {
        EventBus.OnCombatCleared += OnCombatCleared;
        EventBus.OnGameOver += OnGameOver;
    }

    void OnDisable()
    {
        EventBus.OnCombatCleared -= OnCombatCleared;
        EventBus.OnGameOver -= OnGameOver;
    }

    void Start() { StartRun(); }

    void StartRun()
    {
        _currentScenarioIndex = 0;
        LoadScenario(scenarios[_currentScenarioIndex]);
    }

    void LoadScenario(ScenarioData scenario)
    {
        _currentScenario = scenario;
        _currentCombatIndex = 0;
        Debug.Log($"Escenario: {scenario.scenarioName}");
        StartCombat(scenario.enemy1);
    }

    void StartCombat(LevelConfig level)
    {
        CurrentLevel = level;
        scoreCalculator.targetScore = level.targetScore;
        handManager.ApplyModifiers(level.handsModifier, level.dartsModifier);

        // Asignar diana del nivel si tiene una, si no conserva la que hay
        if (level.targetData != null && target != null)
            target.SetTargetData(level.targetData);

        handManager.StartCombat();
        scoreCalculator.ResetScore();
        EventBus.Publish_CombatStarted();
        dartLauncher.enabled = true;
        Debug.Log($"Combate: {level.enemyName} | Objetivo: {level.targetScore}");
    }

    void OnCombatCleared()
    {
        dartLauncher.enabled = false;
        Debug.Log($"Combate superado: {CurrentLevel.enemyName}");
        NextStep();
    }

    void OnGameOver()
    {
        Debug.Log("GAME OVER");
    }

    void NextStep()
    {
        _currentCombatIndex++;

        switch (_currentCombatIndex)
        {
            case 1:
                Debug.Log(_currentScenario.hasEventAfterEnemy1 ? "→ Evento" : "→ Tienda");
                StartCombat(_currentScenario.enemy2);
                break;
            case 2:
                Debug.Log(_currentScenario.hasEventAfterEnemy2 ? "→ Evento" : "→ Tienda");
                StartCombat(_currentScenario.enemy3);
                break;
            case 3:
                Debug.Log(_currentScenario.hasEventAfterEnemy3 ? "→ Evento" : "→ Tienda");
                StartCombat(_currentScenario.boss);
                break;
            case 4:
                Debug.Log("→ Jefe derrotado");
                NextScenario();
                break;
        }
    }

    void NextScenario()
    {
        _currentScenarioIndex++;
        if (_currentScenarioIndex >= scenarios.Count)
        {
            Debug.Log("¡Run completado!");
            return;
        }
        LoadScenario(scenarios[_currentScenarioIndex]);
    }
}