﻿
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public List<Sprite> rankSprites;
    public GameObject winlosePopUp;
    public Text winLoseText;

    public enum GAMESTATE
    {
        NONE,
        PLACE_PAWN,
        PLAY,
        END
    }

    public GAMESTATE currentGameState;

    public Constants.PlayerType currentPlayerTurn;
    private int playerTurnIndex = -1;

    public bool isEndRuleGameActivated;

    public static Action<Constants.PlayerType, bool> OnTurnChanged;
    public static Action OnGameResultDeclare;

    AnimationType animationType;

    public Dictionary<string, Effect> particleSystemKeys = new Dictionary<string, Effect>();
    Effect[] allEffects;

    public AnimationType AnimationType { get => animationType; set => animationType = value; }

    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        BoardManager.OnPawnPlacementComplete += PawnPlacementComplete;
        BoardManager.OnActiveEndRule += ActiveEndGameRule;
        BoardManager.OnDeactiveEndRule += DeactiveEndGameRule;

        allEffects = GetComponentsInChildren<Effect>();

        for (int i = 0; i < allEffects.Length; i++)
        {
            particleSystemKeys.Add(allEffects[i].effectType.ToString(), allEffects[i]);
        }
        Debug.Log("Total Effects: "+ particleSystemKeys.Count);
    }

    private void OnDestroy()
    {
        BoardManager.OnPawnPlacementComplete -= PawnPlacementComplete;
        BoardManager.OnActiveEndRule -= ActiveEndGameRule;
        BoardManager.OnDeactiveEndRule -= DeactiveEndGameRule;
    }

    private void ActiveEndGameRule(Constants.PlayerType type)
    {
        isEndRuleGameActivated = true;
        // Change possible moves and takes to 1
    }
    private void DeactiveEndGameRule()
    {
        isEndRuleGameActivated = false;
        // // Change possible moves and takes to actual rank
    }

    private void PawnPlacementComplete()
    {
        currentGameState = GAMESTATE.PLAY;

        if (Constants.isAI)
        {
            playerTurnIndex = UnityEngine.Random.Range(0, 2);
            IncreaseTurn();
        }
    }

    public void IncreaseTurn()
    {

        if (!Constants.isAI)
        {
            return;
            //submit local player's data to server
        }
        playerTurnIndex++;
        if (playerTurnIndex == 2)
        {
            playerTurnIndex = 0;
        }

        currentPlayerTurn = (Constants.PlayerType)playerTurnIndex;
        if (GameManager.instance.currentGameState != GAMESTATE.END)
            OnTurnChanged?.Invoke(currentPlayerTurn, isEndRuleGameActivated);
        // OnTurnChanged(currentPlayerTurn, isEndRuleGameActivated);
        Debug.Log("Current Game State: " + currentGameState);
    }

    public void IncreaseTurn(Constants.PlayerType playersTurn)
    {
        currentPlayerTurn = playersTurn;
        OnTurnChanged(playersTurn, isEndRuleGameActivated);
    }

    public void OnGameEnd(Constants.PlayerType player, bool sendData = true)
    {
        if (currentGameState == GAMESTATE.END)
            return;
        currentGameState = GAMESTATE.END;
        Debug.Log(player.ToString() + " loss the game.");
        if (player == Constants.PlayerType.LOCAL)
        {
            winLoseText.text = "YOU FAIL";
            MilitakiriAudioManager.OnGameFinished?.Invoke(-1);
            if(sendData)
            {
                SocketController.instance.PingServerAboutGameOver();
            }
        }
        else if (player == Constants.PlayerType.REMOTE)
        {
            winLoseText.text = "YOU WIN";
            MilitakiriAudioManager.OnGameFinished?.Invoke(1);
        }
        else
        {
            winLoseText.text = "GAME DRAW";
            MilitakiriAudioManager.OnGameFinished?.Invoke(0);
        }

        winlosePopUp.SetActive(true);

        if (OnGameResultDeclare != null)
            OnGameResultDeclare();
    }


}
