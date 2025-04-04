using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIBoard : MonoBehaviour
{
    [SerializeField] private GameObject panelGameOver;

    private TextMeshProUGUI _timerText;
    private TextMeshProUGUI _player1TextScore;
    private TextMeshProUGUI _player2TextScore;
    
    private float _remainingTime = 180f;

    private int _player1Score;
    private int _player2Score;

    private bool _isGameOver;
    
    private void Awake()
    {
        _timerText = GetComponentText("Time");
        _player1TextScore = GetComponentText("Player 1 Score");
        _player2TextScore = GetComponentText("Player 2 Score");
    }

    private TextMeshProUGUI GetComponentText(string nameGameObject)
    {
        TextMeshProUGUI text = null;
        Transform child = GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == nameGameObject);
        if (child != null)
            text = child.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        return text;
    }
    
    private void Update()
    {
        if(GameManager.Instance.currentState == GameState.Playing)
            GameTimer();
        
        // Phan nay phai toi uu hon
        _player1Score = PlayerController.Instance.score;
        if (BotController.Instance.gameObject.activeSelf)
        {
            _player2Score = BotController.Instance.score;
        }
        _player1TextScore.text = _player1Score.ToString();
        _player2TextScore.text = _player2Score.ToString();
    }
    
    private void GameTimer()
    {
        if (_remainingTime <= 0)
        {
            Debug.Log("Hết thời gian");
            panelGameOver.SetActive(true);
            GameManager.Instance.ChangeState(GameState.GameOver);
            return;
        }
        
        _remainingTime -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(_remainingTime / 60);
        int seconds = Mathf.FloorToInt(_remainingTime % 60);
        if(_timerText != null)
            _timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        
    }
}
