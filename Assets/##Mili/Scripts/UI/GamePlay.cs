﻿using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
public class GamePlay : UIPage
{
    public static GamePlay instance;
    public Camera cam;
    public Camera effectCam;
    public float speedforCameraSwitch = 0.25f;
    public Ease easeTypeForCamera;
    public Text descriptionForPlacingPawn;
    public RectTransform glowForCurrentTurn;
    public List<RectTransform> playerPicPosition;

    private Constants.PlayerType currentPlayerType;

    // public RectTransform turnBowl;
    public Ease easeType;

    public List<Image> viewTypeButtons;
    public List<Sprite> viewTypeSprite;

    public GameObject gameQuitConfirmationPopup;
    public GameObject warningMessageBox;

    public static Action OnTimerComplete;
    public static Action<Constants.ViewType> OnViewTypeChange;
    public static Action<float> avatarTime;
    public AvatarTimerUpdate localTimer;
    public AvatarTimerUpdate remoteTimer;

    public Vector3 camDefaultPosition;
    public Transform camera3DDefault;
    public Transform camera2DDefault;
    [SerializeField]
    private float camAnimTime = 2;
    [SerializeField]
    private Text nameText_local;
    [SerializeField]
    private Text nameText_remote;
    public GameObject backButton;

    public GameObject clockToResize;

    public Transform messagePanel;
    public Transform messageOpponent;
    public Text opponentMessage;


    public Image localPlayerDP;
    public Image opponentDP;

  
    
    private void Awake()
    {
        instance = this;
        GameManager.OnTurnChanged += OnTurnChanged;
        GameManager.OnGameResultDeclare += StopTurnTimer;
        PopUp.OnClosePopUp += OnClosePopUp;
        ViewTypeClicked((int)Constants.ViewType.THREE_D);
        StartCoroutine(SetViewType());
        //if (Constants.isAI)
        //{
        //    backButton.SetActive(true);
        //}
        //else
        //{
        //    backButton.SetActive(false);
        //}
        // effectCam.gameObject.SetActive(false);
        // cam.gameObject.SetActive(true);
        // ViewTypeClicked((int)Constants.ViewType.THREE_D);
        // BoardManager.InitializeGameRules?.Invoke();
        // Invoke("InitializeGameRules", 1);
    }

    //for testing fast
    void InitializeGameRules()
    {
        BoardManager.InitializeGameRules?.Invoke();
    }

    private void Start()
    {
        nameText_local.text = Database.GetString(Database.Key.FIRST_NAME);
        nameText_remote.text = Database.GetString(Database.Key.OPPONENT_NAME);
        SocketController.GotAMessage += ShowMessage;
        // SendMessage("ShowMessage", "Good Game");
        
    }

    private void ShowMessage(string arg0)
    {
        opponentMessage.text = arg0;       
        messageOpponent.DOMoveY(messageOpponent.position.y-70f, 0.3f);
        DOVirtual.DelayedCall(3, () => messageOpponent.DOMoveY(messageOpponent.position.y + 70f, 0.3f));
    }

    IEnumerator SetViewType()
    {
        yield return new WaitForSeconds(13.5f);// (13.5f);
        effectCam.gameObject.SetActive(false);
        cam.gameObject.SetActive(true);
        ViewTypeClicked((int)Constants.ViewType.THREE_D);
        //if multiplayer, socket Controller invokes this
        if (Constants.isAI)
            BoardManager.InitializeGameRules?.Invoke();
        else
            SocketController.instance.LoadedBoardScene();
    }

    public void SetOpponentName(string nam)
    {
        nameText_remote.text = nam;
    }

   public void SendAMessage(string buttonMessage)
    {
        SocketController.SendAMessage?.Invoke(buttonMessage);
        ChatClicked();
    }

    

    //void CamInitAnimation()
    //{
    //    cam.transform.position = waypoints[0];
    //    cam.transform.LookAt(Vector3.zero);
    //    cam.transform.DOPath(waypoints, camAnimTime, PathType.Linear).OnUpdate(() => { cam.transform.LookAt(Vector3.zero); }).SetDelay(3);
    //}

    private void OnClosePopUp(PopUp.PopUpType obj)
    {
        if (obj == PopUp.PopUpType.ERROR)
            SceneManager.LoadScene("UI");
        else if (obj == PopUp.PopUpType.OK)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else
        {
            GameManager.instance.OnGameEnd(Constants.PlayerType.LOCAL);
        }
    }

    private void OnDestroy()
    {
        GameManager.OnTurnChanged -= OnTurnChanged;
        GameManager.OnGameResultDeclare -= StopTurnTimer;
        PopUp.OnClosePopUp -= OnClosePopUp;
    }

    private void OnTurnChanged(Constants.PlayerType obj, bool isEndRuleGameActivated)
    {

       // Debug.LogError("OnTurnChanged: " + (int)obj + " player name" + obj.ToString());
        currentPlayerType = obj;
        int playerIndex = (int)obj;
        glowForCurrentTurn.anchoredPosition = playerPicPosition[playerIndex].anchoredPosition;

        //Vector2 targetPos;
        //if(playerIndex == 1)
        //{
        //    turnBowl.anchoredPosition = new Vector2(0, -1245f);
        //    targetPos = Vector2.zero;
        //}
        //else
        //{
        //    turnBowl.anchoredPosition = Vector2.zero;
        //    targetPos = new Vector2(0, -1245f);
        //}
        if (playerIndex == 1)
        {
            avatarTime -= localTimer.UpdateFill;
            avatarTime += remoteTimer.UpdateFill;
            localTimer.ResetFill();
        }
        else
        {

            avatarTime -= remoteTimer.UpdateFill;
            avatarTime += localTimer.UpdateFill;
            remoteTimer.ResetFill();
        }

        //turnBowl.DOAnchorPos(targetPos, 0.7f).SetEase(easeType);
        //  Debug.Log("Turn Changed: "+ GameManager.instance.currentPlayerTurn.ToString());
        StopTurnTimer();
        StartTurnTimer(totalTime);
    }

    public void SetDesriptionForPlacingPawn(string text)
    {
        descriptionForPlacingPawn.text = text;
    }

    bool isOpen = false;
    public void ChatClicked()
    {
        isOpen = !isOpen;

        if(isOpen)
        {
            messagePanel.gameObject.SetActive(true);
            messagePanel.DOScale(Vector3.one, 0.2f);
        }
        else
        {
            messagePanel.DOScale(Vector3.zero, 0.2f).OnComplete(()=>messagePanel.gameObject.SetActive(false));
        }
    }

    public void BackClicked()
    {
        gameQuitConfirmationPopup.SetActive(true);
    }

    public void ViewTypeClicked(int type)
    {
        CamController.SettingViewType?.Invoke();
        for (int i = 0; i < viewTypeButtons.Count; i++)
        {
            if (type == i)
                viewTypeButtons[i].sprite = viewTypeSprite[1];
            else
                viewTypeButtons[i].sprite = viewTypeSprite[0];
        }
        Constants.currentViewType = (Constants.ViewType)Enum.ToObject(typeof(Constants.ViewType), type);

        //   Vector3 pos = cam.transform.position;
        //   Vector3 angle = cam.transform.eulerAngles;
        //  Debug.Log(angle + "angle");
        if (Constants.currentViewType == Constants.ViewType.TWO_D)
        {
            cam.orthographic = true;
            SetPositionAndCamRotation(camera2DDefault.position, camera2DDefault.eulerAngles);
            clockToResize.transform.DOScale(new Vector3(0.8f, 0.8f, 1), 1);
        }
        else
        {
            cam.orthographic = false;
            SetPositionAndCamRotation(camera3DDefault.position, camera3DDefault.eulerAngles);
            clockToResize.transform.DOScale(Vector3.one, 1);
            if (OnViewTypeChange != null)
            {
                OnViewTypeChange(Constants.currentViewType);
            }
        }
    }

    private void SetPositionAndCamRotation(Vector3 targetPosition, Vector3 targetAngle)
    {
        // Debug.Log("Target Postion : "+ targetPosition);
        cam.transform.DOLocalMove(targetPosition, speedforCameraSwitch).SetEase(easeTypeForCamera).OnComplete(() =>
        {
            if (Constants.currentViewType == Constants.ViewType.TWO_D)
            {

                if (OnViewTypeChange != null)
                {
                    OnViewTypeChange(Constants.currentViewType);
                }
                //   cam.transform.GetChild(0).gameObject.SetActive(true);
            }
        });
        cam.transform.DOLocalRotate(targetAngle, speedforCameraSwitch + 0.2f).SetEase(easeTypeForCamera).OnComplete(() =>
        {
            if (Constants.currentViewType == Constants.ViewType.THREE_D)
            {
                CamController.OnFinishedSettingViewType?.Invoke();
            }
        });
    }


    #region Timer

    public float totalTime = 300f;
    private float timeLeft;
    public Text timerText;
    private bool canStart = false;
    private float totalSeconds = 0;

    // Update is called once per frame
    void Update()
    {
        if (canStart && timeLeft > 0)
        {
            TimeSpan t = TimeSpan.FromSeconds(timeLeft);
            timeLeft -= Time.deltaTime;
           
                
            // float value = 1 -( timeLeft / totalTime);            
            timerText.text = t.Minutes.ToString("00") + ":" + t.Seconds.ToString("00");
            avatarTime?.Invoke(timeLeft / totalSeconds);
            if ((timeLeft) <= 0)
            {
                canStart = false;
                // TIMER HAS EXPIRED
                //  GameManager.instance.currentGameState = GameManager.GAMESTATE.END;
                OnTimerComplete?.Invoke();

                // GameManager.instance.OnGameEnd(currentPlayerType);
            }
        }

    }// END OF UPDATE FUNCTION

    public void StartTurnTimer(float totalTime)
    {
        if (totalTime == 0)
        {

        }
        extraTime = 0f;
        timeLeft = totalTime;
        canStart = true;
        OnTimerComplete += OnFailedPawnPlacement;
        totalSeconds = totalTime;
        Debug.Log("Total Time: " + totalTime);
        if (GameManager.instance.currentPlayerTurn == Constants.PlayerType.LOCAL)
            InvokeRepeating("InvokeTimeEvents", 1, 1);
    }

    void OnFailedPawnPlacement()
    {
        Debug.Log("Failed to placepawns");
        ShowWarning(false);
        GameManager.instance.OnGameEnd(currentPlayerType);
    }
    // callback here call random placement of pawns function after time runs out
    public void StartTurnTimer(Action callback, float totalTime)
    {
        extraTime = 0f;
        timeLeft = totalTime;
        canStart = true;
        OnTimerComplete += callback;
        totalSeconds = totalTime;
        //  Debug.Log("Total Time: " + totalTime);
    }


    public void StopTurnTimer()
    {
        canStart = false;
        OnTimerComplete = null;
        if (GameManager.instance.currentPlayerTurn == Constants.PlayerType.LOCAL)
            CancelInvoke("InvokeTimeEvents");
    }
    private System.DateTime pausedTime;
    private double extraTime;

    public void ShowWarning(float delay)
    {
        Invoke("DisplayWarning", delay);
    }

    public void DisplayWarning()
    {
        ShowWarning();
    }

    public void ShowWarning(bool state = true)
    {
        if (state == false)
        {
            CancelInvoke("DisplayWarning");
        }
        else
        {
            SetDesriptionForPlacingPawn("");
        }

        warningMessageBox.SetActive(state);
    }

    private void OnApplicationPause(bool pause)
    {
        /* if (!UIManager.instance.isBotPlaying)
         {
             if (!pause && canStart)
             {
                 // resume the game 
                 extraTime = (System.DateTime.Now - pausedTime).TotalSeconds;
                 timeLeft -= (float)extraTime;
             }
             else if (canStart)
             {
                 // Pause the game 
                 pausedTime = System.DateTime.Now;
             }
         }*/
    }

    void InvokeTimeEvents()
    {
        MilitakiriAudioManager.TimeEvents?.Invoke(timeLeft);
    }





    #endregion

}
