
using System;
using UnityEngine;

/// <summary>
/// Controls the overall game state and flow. Singleton pattern for global access.
/// Other systems can subscribe to OnGameStateChanged to react to state changes.
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    // Enum for all possible game states
    public enum GameState { MainMenu, Playing, Paused, GameOver }

    // Event for state changes. Subscribe to this to react to state changes.
    public static event Action<GameState> OnGameStateChanged;

    // Backing field for current state
    private GameState _currentState = GameState.MainMenu;
    public GameState CurrentState
    {
        get => _currentState;
        private set
        {
            if (_currentState != value)
            {
                _currentState = value;
                OnGameStateChanged?.Invoke(_currentState);
            }
        }
    }

    private void Awake()
    {
        // Singleton enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: persist across scenes
    }

    // Call this to change the game state
    public void SetGameState(GameState newState)
    {
        CurrentState = newState;
    }

    // Example: Start the game from main menu
    private void Start()
    {
        // Set initial state if needed
        SetGameState(GameState.MainMenu);
    }

    // Example: Listen for pause input (expand as needed)
    private void Update()
    {
      
    }
}
