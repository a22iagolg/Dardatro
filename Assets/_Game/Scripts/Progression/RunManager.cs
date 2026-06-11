using UnityEngine;
using System.Collections.Generic;

public class RunManager : MonoBehaviour
{
    [Header("Escenarios del run (en orden)")]
    public List<ScenarioData> scenarios;

    [Header("Referencias")]
    public HandManager     handManager;
    public ScoreCalculator scoreCalculator;
    public DartLauncher    dartLauncher;

    private int          _currentScenarioIndex = 0;
    private ScenarioData _currentScenario;

    public static LevelConfig CurrentLevel { get; private set; }

    void OnEnable()
    {
        EventBus.OnCombatCleared += OnCombatCleared;
        EventBus.OnGameOver      += OnGameOver;
    }

    void OnDisable()
    {
        EventBus.OnCombatCleared -= OnCombatCleared;
        EventBus.OnGameOver      -= OnGameOver;
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
        Debug.Log($"Escenario: {scenario.scenarioName}");
        StartCombat(scenario.enemy1);
    }

    void StartCombat(LevelConfig level)
    {
        CurrentLevel                 = level;
        scoreCalculator.targetScore  = level.targetScore;
        handManager.ApplyModifiers(level.handsModifier, level.dartsModifier);
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
        if (CurrentLevel == _currentScenario.enemy1)
        {
            Debug.Log(_currentScenario.hasEventAfterEnemy1 ? "→ Evento" : "→ Tienda");
            StartCombat(_currentScenario.enemy2);
        }
        else if (CurrentLevel == _currentScenario.enemy2)
        {
            Debug.Log(_currentScenario.hasEventAfterEnemy2 ? "→ Evento" : "→ Tienda");
            StartCombat(_currentScenario.boss);
        }
        else if (CurrentLevel == _currentScenario.boss)
        {
            Debug.Log("→ Jefe derrotado");
            NextScenario();
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