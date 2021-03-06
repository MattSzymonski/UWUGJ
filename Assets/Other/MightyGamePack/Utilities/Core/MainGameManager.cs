using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MightyGamePack;
using NaughtyAttributes;

public class MainGameManager : MightyGameManager, IMainGameManager
{
    public float score;
    [HideInInspector] public static MainGameManager mainGameManager;

    public PlayerController pc;

    public void Awake()
    {
        InitializeMighty(this); // Initialize mighty pack
        mainGameManager = this;
    }

    public void Start()
    {
        
    }

    public void Update()
    {
        if (gameState == GameState.Playing)
        {
            score += Time.unscaledDeltaTime; // Seconds
            UIManager.SetScore((int)Mathf.Floor(score));
        }
    }

    //------------------------------------------------ GAME STATE FUNCTIONS ---------------------------------------------------- (CODE IN THESE FUNCTIONS WILL AUTOMATICALY BE EXECUTED ON GAME STATE CHANGES)
    public void PlayGame() { }

    public void GameOver()
    {
        if (!debugHideUI && gameState == GameState.Playing)
        {
            UIManager.GameOver();
        }
    }

    public void PauseGame() { }

    public void UnpauseGame() { }

    public void BackToMainMenu() { }

    public void RestartGame() { 
        score = 0; 
        UIManager.ResetScore();


        pc.RestartGame();
    
    } //Clearing the scene, removing enemies, respawning player, reseting score, etc

    public void OpenOptions() { }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                Debug.Log("Cannot quit game in editor");
        #else
                Application.Quit();      
        #endif
    }

    //--------------------------------------------------- OTHER FUNCTIONS ------------------------------------------------------

}
