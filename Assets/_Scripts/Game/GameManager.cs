using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState{ Menu, CutScene, Playing, Paused, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState currentState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    
    public void ChangeState(GameState newState)
    {
        currentState = newState;
        ActiveState(currentState);
    }
    
    private void ActiveState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Menu:
                Time.timeScale = 0f;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
            case GameState.CutScene:
                Time.timeScale = 1f;
                break;
        }
    }
}
