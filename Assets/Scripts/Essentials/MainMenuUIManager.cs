using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;


public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _exitButton;

    public void Awake()
    {
        if (_playButton != null)
            _playButton.onClick.AddListener(OnPlayButtonClicked);

        if (_exitButton != null)
            _exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        GameManager.Instance.SetGameState(GameManager.GameState.Playing);
        SceneManager.LoadScene("GameScene");
    }

    private void OnExitButtonClicked()
    {
        Application.Quit();
    }
}
