using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;


public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private Button _playButton;

    public void Awake()
    {
        if (_playButton != null)
            _playButton.onClick.AddListener(OnPlayButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        GameManager.Instance.SetGameState(GameManager.GameState.Playing);
        SceneManager.LoadScene("GameScene"); 
    }
}
