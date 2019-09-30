using SimpleJSON;
using socket.io;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using GWebUtility;
using UnityEngine.Networking;
using System.Collections;

public class SocketController : MonoBehaviour
{
    public static SocketController instance;
    public static bool isSpareTowerTurn = false;
    private Socket socket;

    public static UnityAction<string> SendAMessage;
    public static UnityAction<string> GotAMessage;

    public static string imageBaseUrl = "http://52.66.82.72:2095/images/avatar/";
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        PopUp.OnClosePopUp += OnQuitMatch;
    }

    void OnDisable()
    {
        PopUp.OnClosePopUp -= OnQuitMatch;
    }
    /*

        "access_token" :    "1bc66eb0-0032-415c-9ec6-260943229576",
        "room_id"      :    "5c2dfa22a545572e838dae2b",
        "room_name"    :    "ROOM1546517026377"}

     */
    private void OnQuitMatch(PopUp.PopUpType obj)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["room_id"] = Database.GetString(Database.Key.ROOM_ID);
        data["room_name"] = Database.GetString(Database.Key.ROOM_NAME);
        data["access_token"] = Database.GetString(Database.Key.ACCESS_TOKEN);
        socket.EmitJson(GameEmits.leave.ToString(), new JSONObject(data).ToString());
        Debug.Log("Match Quit");
    }

    private void Start()
    {
        socket = Socket.Connect(Constants.SocketURL);
        socket.On(SystemEvents.connect, () =>
        {
            Debug.Log("Socket connected successfully");
        });

        socket.On(SystemEvents.disconnect, () =>
        {
            Debug.Log("Socket disconnected successfully");
        });

        socket.On(SystemEvents.reconnect, (int reconnectionAttempt) =>
        {
            Debug.Log("Socket recoonected after " + reconnectionAttempt + " attempt");
        });

        socket.On(GameListen.user_connected.ToString(), UserConnectedCallBack);
        socket.On(GameListen.connected_room.ToString(), GameRequestCallback);  //when room created and waiting for another player
        socket.On(GameListen.enter_user.ToString(), EnterUserCallback); //fired twice
        socket.On(GameListen.gameinit.ToString(), GameInitCallback); //when two player joins and game starts. Change scene here.
        socket.On(GameListen.gamestart.ToString(), GameStart);      
        socket.On(GameListen.initialpawnplacements.ToString(), StartingPawnsPlaced);
        socket.On(GameListen.turndata.ToString(), OpponentsTurnCallback);
        socket.On(GameListen.playerturn.ToString(), WhoWillPlay);
        socket.On(GameListen.turn_start.ToString(), WhoWillPlay);
        socket.On(GameListen.leave_room.ToString(), SomeoneLeftRoom);
        socket.On(GameListen.game_winner.ToString(), GameDone);
        socket.On(GameListen.sparePawnData.ToString(), SparePawnData);
        socket.On(GameListen.message_received.ToString(), MessageReceived);
        SendAMessage += MessageSending;


       // Invoke("GetAvatars", 3);
    }
    /*
     * {"access_token":"5bec63f5-ac31-4587-a6ab-1a62f63df0ac",
     * "room_id":"5c373dc9c23f914c7298258c",
     * "room_name":"ROOM1547124169296",
     * "message":"Greetings"}

     * 
     * 
     */
    #region Chat Methods
    private void MessageSending(string arg0)
    {
        Debug.Log("Sending message: " + arg0);
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["access_token"] = Database.GetString(Database.Key.ACCESS_TOKEN);
        data["room_id"] = Database.GetString(Database.Key.ROOM_ID);
        data["room_name"] = Database.GetString(Database.Key.ROOM_NAME);
        data["message"] = arg0;

        socket.EmitJson(GameEmits.newmessage.ToString(), new JSONObject(data).ToString());

    }

    private void MessageReceived(string newMessageData)
    {
        JSONNode data = JSONNode.Parse(newMessageData);
        string newMessage = data["chat"]["message"].Value;
        Debug.Log("New Message: " + newMessage);
        GotAMessage?.Invoke(newMessage);
    } 
    #endregion

  
        
    

    private void SparePawnData(string obj)
    {      
        Debug.Log("Got Spare Pawn Data: " + obj);       
        JSONNode data = JSONNode.Parse(obj);
        string id = data["result"]["turnData"]["to"]["SquareID"].Value;
        Debug.Log("Got target square id as : "+ id);
        Square targetSquare = BoardManager.instance.GetSquare(id);
        Player player = BoardManager.instance.dic_players[Constants.PlayerType.REMOTE];
        BoardManager.instance.PlaceSparePawnForRemote(id);
    }

    private void GameDone(string obj)
    {
        Debug.Log("Game Over " + obj);

        //  { "status":"1","result":{ "player_id":"5c2de89f2888d32babc596d5"},"message":"game winner declare"}
        JSONNode data = JSONNode.Parse(obj);
        string id = data["result"]["player_id"].Value;
        string thisPlayerId = Database.GetString(Database.Key.PLAYER_ID);
        if(thisPlayerId == id)
        {
            GameManager.instance.OnGameEnd(Constants.PlayerType.LOCAL, false);
        }
        else
        {
            GameManager.instance.OnGameEnd(Constants.PlayerType.REMOTE, false);
        }

    }

    private void SomeoneLeftRoom(string obj)
    {
        Debug.Log("SomeoneLeftRoom " + obj);
    }

  //  {"room_id":"5d68ef12b759572749c636f8","user_id":"5cf64d845732bb2515e975d0"}
    public void  PingServerAboutGameOver()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["room_id"] = Database.GetString(Database.Key.ROOM_ID);
        data["user_id"] = Database.GetString(Database.Key.PLAYER_ID);
        socket.EmitJson(GameEmits.gameOver.ToString(), new JSONObject(data).ToString());
    }
    /*
* {
"status":"1",
"message":"player turn",
"result":{
"user_id":"5d3ec68bf17488071c0fa214",
"name":"Ganesh",
"player_id":"MILT-5d3ec68bf17488071c0fa214"
}
} 
* 
*/
    private void WhoWillPlay(string obj)
    {
        Debug.Log("WhoWillPlay" + obj);
        JSONNode data = JSONNode.Parse(obj);
        string currentPlayerUserId = data["result"]["user_id"].Value;
        Debug.Log(currentPlayerUserId + "____" + Database.GetString(Database.Key.PLAYER_ID));
        if (Database.GetString(Database.Key.PLAYER_ID).CompareTo(currentPlayerUserId) == 0)
        {
            Debug.Log("My Turn");
            BoardManager.instance.canClick = true;
            GameManager.instance.currentGameState = GameManager.GAMESTATE.PLAY;
            GameManager.instance.IncreaseTurn(Constants.PlayerType.LOCAL);
        }
        else
        {
            Debug.Log("Other Players Turn");
            Debug.Log(GameManager.instance.currentGameState.ToString());
            GameManager.instance.currentGameState = GameManager.GAMESTATE.NONE;
            GameManager.instance.IncreaseTurn(Constants.PlayerType.REMOTE);
        }
    }


    #region Emit Functions
    public void AddUser()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["access_token"] = Database.GetString(Database.Key.ACCESS_TOKEN);
        socket.EmitJson(GameEmits.adduser.ToString(), new JSONObject(data).ToString());

#if SOCKET_EMIT_LOG
        Debug.Log("Add User: " + new JSONObject(data).ToString());
#endif
    }

    public void GameRequest()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["access_token"] = Database.GetString(Database.Key.ACCESS_TOKEN);
        data["board_type"] = Constants.currentBoardType.ToString().ToLower();
        data["score"] = "35";//Database.GetString(Database.Key.SCORE); 
        Debug.Log(" Database.GetString(Database.Key.SCORE)  " + data["score"]);
        socket.EmitJson(GameEmits.gamerequest.ToString(), new JSONObject(data).ToString());

#if SOCKET_EMIT_LOG
        Debug.Log("gamerequest: " + new JSONObject(data).ToString());
#endif
    }

    public void Leave()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["access_token"] = Database.GetString(Database.Key.ACCESS_TOKEN);
        data["room_id"] = Database.GetString(Database.Key.ROOM_ID);
        data["room_name"] = Database.GetString(Database.Key.ROOM_NAME);

        socket.EmitJson(GameEmits.leave_room.ToString(), new JSONObject(data).ToString());
    }

    public void Chat()
    {

    }
    #endregion

    public void FinishedTurn()
    {
        if (Constants.isAI)
            return;
        //{ "room_id":"5d3adfceb7345c267b360b67","room_name":"ROOM1564139470776",
        //    "user_id":"5cf64d845732bb2515e975d0","turn_start":"1"}
        if (isSpareTowerTurn)
            return;
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["room_id"] = Database.GetString(Database.Key.ROOM_ID);
        data["room_name"] = Database.GetString(Database.Key.ROOM_NAME);
        data["user_id"] = Database.GetString(Database.Key.PLAYER_ID);
        data["turn_start"] = "1";
        Debug.Log("Turn Start Fired " + data.ToString());
        socket.EmitJson(GameEmits.turnstart.ToString(), new JSONObject(data).ToString());
    }

    #region On Functions
    public void UserConnectedCallBack(string jsonData)
    {
#if SOCKET_ON_LOG
        Debug.Log("User Connected :" + jsonData);
        //  LoadingCanvas.Instance.ShowLoadingPopUp("Searching Player! Please wait");
#endif
        //  Invoke("UserEntered", 3);
        //  Invoke("StartGame", 5);
        GameRequest();
    }

    public void GameRequestCallback(string jsonData)
    {
#if SOCKET_ON_LOG
        Debug.Log("Game Requested :" + jsonData);
#endif
        JSONNode data = JSONNode.Parse(jsonData);
        if (data["status"].Value == ErrorCode.SUCCESS_CODE)
        {
            Debug.Log("Show Searching player");
            LoadingCanvas.Instance.ShowLoadingPopUp("Searching Player! Please wait");
        }
    }
    #region EnterUser
    /*
 * 
 * {
"status":"1",
"message":"New user enter to room",
"result":{
"room_id":"5c18ff6fb1caa92f5af24dd3",
"room_name":"ROOM1545142127091",
"roomplayer_id":"5c18ff6fb1caa92f5af24dd4",
"pending_time":10,
"rank":"Starter"
}
}
 * 
 * 
 * */
    #endregion
    public void EnterUserCallback(string jsonData)
    {
#if SOCKET_ON_LOG
        Debug.Log("Enter User :" + jsonData);
        // get opponent information here and show user info on screen like name, picture etc
        JSONNode data = JSONNode.Parse(jsonData);
        //now change scene saving the players information
        Database.PutString(Database.Key.ROOM_NAME, data["result"]["room_name"]);
        Database.PutString(Database.Key.ROOM_ID, data["result"]["room_id"]);



        //parse to "PawnPlaced"
        //Call BoardManager to set the pawns
#endif
    }
    #region CallInitJson
    /*
* 
* {
"status":"1",
"message":"game inits",
"result":{
"turncount":50,
"room_id":"5cf65591328267444552b5bd",
"roomplayers":[
{
"user_id":"5cb42218a0f19e2d5a64af93",
"room_player_id":"MILT-5cb42218a0f19e2d5a64af93",
"room_player_name":"pritam"
},
{
"user_id":"5cf5228dbc3b52166dc6d1e7",
"room_player_id":"MILT-5cf5228dbc3b52166dc6d1e7",
"room_player_name":"a"
}
]
}
}
*/
    #endregion
    public void GameInitCallback(string jsonData)
    {
#if SOCKET_ON_LOG
        Debug.Log("Game init :" + jsonData);
#endif
        Debug.Log("Stop Searching player");
       // LoadingCanvas.Instance.HideLoadingPopUp();
        JSONNode data = JSONNode.Parse(jsonData);
        if (data["status"].Value == ErrorCode.SUCCESS_CODE)
        {
            Debug.Log("Take to the Game page");
            //get opponent players name from roomplayers data["result"]["roomplayers"]
            for (int i = 0; i < 2; i++)
            {
                string thisUserId = data["result"]["roomplayers"][i]["user_id"].Value.ToString();
                
                //  Debug.Log("this user id: "+ Database.GetString(Database.Key.PLAYER_ID) + "___other: "+ data["result"]["roomplayers"][i]["user_id"].Value.ToString());
                if (!thisUserId.Equals(Database.GetString(Database.Key.PLAYER_ID)))
                {
                    string opponentsName = data["result"]["roomplayers"][i]["room_player_name"].Value.ToString();
                    Debug.Log("Opponents Name: "+ opponentsName);
                    LoadingCanvas.Instance.ShowOnlyInfo("Starting match with player " + opponentsName);
                    Database.PutString(Database.Key.OPPONENT_NAME, opponentsName);
                    StartCoroutine(GetAvatarImageRemote(data["result"]["roomplayers"][i]["avatar_id"].Value));
                }
                else
                {
                    StartCoroutine(GetAvatarImageLocal(data["result"]["roomplayers"][i]["avatar_id"].Value));
                }
            } //opponent player name           

            string playerOneUserID = data["result"]["roomplayers"][0]["user_id"].Value.ToString();
            if (Database.GetString(Database.Key.PLAYER_ID).Equals(playerOneUserID))
                MultiplayerManager.SetPlayerTag?.Invoke(Constants.PlayerTag.PLAYER_1);
            else
                MultiplayerManager.SetPlayerTag?.Invoke(Constants.PlayerTag.PLAYER_2);
        }
        else if (data["status"].Value == ErrorCode.ERROR_STATUS)
        {
            LoadingCanvas.Instance.HideLoadingPopUp();
            PopupCanvas.Instance.ShowAlertPopUp(data["message"].Value);
            return;
        }

        MultiplayerManager.Instance.SwitchToGameScene();
    }

    //Temporary
    public void LoadedBoardScene()
    {
        //    Debug.Log("Loaded board Scene");
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["room_id"] = Database.GetString(Database.Key.ROOM_ID);
        data["room_name"] = Database.GetString(Database.Key.ROOM_NAME);
        data["user_id"] = Database.GetString(Database.Key.PLAYER_ID);
        data["status"] = "ready"; //!!
        socket.EmitJson(GameEmits.playerReady.ToString(), new JSONObject(data).ToString());
    }

    void GameStart(string jsonData)
    {
        Debug.Log("Game Start :" + jsonData);
        LoadingCanvas.Instance.HideLoadingPopUp();
        BoardManager.InitializeGameRules?.Invoke();
    }
    public void GameWinnerCallback(string jsonData)
    {
#if SOCKET_ON_LOG
        Debug.Log("Game Winner :" + jsonData);
#endif
        JSONNode data = JSONNode.Parse(jsonData);
        if (data["status"].Value == ErrorCode.SUCCESS_CODE)
        {
            if (data["result"]["player_id"].Value == Database.GetString(Database.Key.PLAYER_ID))
            {
                Debug.Log("You win");
            }
            else
            {
                Debug.Log("You lose");
            }
        }
    }

    //dummy callback
    public void StartGame()
    {
        LoadingCanvas.Instance.HideLoadingPopUp();
        SceneManager.LoadScene(1);
    }

    public void UserEntered()
    {
        LoadingCanvas.Instance.ShowLoadingPopUp("Ganesh Joined! Starting Game...");
    }

    public void SendPawnPlacements(PawnPlacements pawnPlacements)
    {
        //Debug.Log("First Pawn" + pawnPlaced.positions[0] + " Seconds Pawn" + pawnPlaced.positions[1] + " Third Pawn" + pawnPlaced.positions[2]);
        string newPawnPlacedData = JsonUtility.ToJson(pawnPlacements);
        Debug.Log("Sending pawn placed data: " + newPawnPlacedData);
        Debug.Log(GameEmits.pawnplacements.ToString());
        socket.EmitJson(GameEmits.pawnplacements.ToString(), newPawnPlacedData);
        GamePlay.instance.StopTurnTimer();
        GameManager.instance.currentGameState = GameManager.GAMESTATE.NONE;
        //Emit("pawnplacement",newpawnPlacedData);
        //PawnPlaced(newPawnPlacedData); //dont  call this from here        
    }

    //Call this with the opponent data.....sets pawns for opponent
    public void StartingPawnsPlaced(string data)
    {
        Debug.Log("Got Pawn Data: " + data);
        LoadingCanvas.Instance.HideLoadingPopUp();

        FirstPawnData opponentPawnPlacement = JsonUtility.FromJson<FirstPawnData>(JSON.Parse(data)["result"].ToString());
        BoardManager.instance.PlacePawnForOpponent(opponentPawnPlacement);
    }

    public void OnTurnStart(string data)
    {
        // get the player id of the player will play first
        //Constants.PlayerType
        GameManager.instance.currentGameState = GameManager.GAMESTATE.PLAY;
        //find out if its the local or remote player. 


        //Apply last remote player's turn if any here
        //if not(this. player)
        /*player input to none. 
         * 
         * notification about players turn, timer start
         * 
         * 
         * 
         * */



        if (true) //local player
        {
            //Call OnTurnChanged if 0 for local player
            GameManager.instance.IncreaseTurn(Constants.PlayerType.LOCAL);
        }
        else //if remote player's turn
        {
            //call OnTurnChanged with 1 for remote player
            GameManager.instance.IncreaseTurn(Constants.PlayerType.REMOTE);
        }
    }


    //Gets all the avatars to use as DP
    



    public void SubmitTurn(Square from, Square to)
    {
        //TurnData turnData = new TurnData(from.squareId, to.squareId);
        //string turnDataString = JsonUtility.ToJson(turnData);
        //Debug.Log("Player turn data: " + turnDataString);
        //emit("submitTurnData", turnDataString);
    }

    public void PrepareTurnData(GridData f, GridData t, string turnType = "normalType")
    {
        if (Constants.isAI)
            return;
        TurnData turnData = new TurnData(f, t);
        TurnDataWrapper turnDataWrapper = new TurnDataWrapper(Database.GetString(Database.Key.ROOM_ID), Database.GetString(Database.Key.PLAYER_ID), turnData, Database.GetString(Database.Key.ROOM_NAME), turnType);
        string data = JsonUtility.ToJson(turnDataWrapper);
        Debug.Log("Turn Submitted: " + data);
        socket.EmitJson(GameEmits.turnSubmited.ToString(), data);
        if(turnType == "normalType")
            GameManager.instance.currentGameState = GameManager.GAMESTATE.NONE;
    }

    public void PrepareSparepawnData(string targetSquareId)
    {
        if (Constants.isAI)
            return;
        GridData from = new GridData("", "", "");
        GridData to = new GridData(targetSquareId, "", "");
        TurnData turnData = new TurnData(from, to);
        TurnDataWrapper turnDataWrapper = new TurnDataWrapper(Database.GetString(Database.Key.ROOM_ID), Database.GetString(Database.Key.PLAYER_ID), turnData, Database.GetString(Database.Key.ROOM_NAME), "sparePawn");
        string data = JsonUtility.ToJson(turnDataWrapper);
        Debug.Log("Spare Pawn Data: "+ data);
        socket.EmitJson(GameEmits.sparePawnTurn.ToString(), data);
        GameManager.instance.currentGameState = GameManager.GAMESTATE.NONE;
    }


    private void OpponentsTurnCallback(string data)
    {
        Debug.Log("Got Opponents Data : " + data);
        var n = JSON.Parse(data);
        string fromSquareID = n["result"]["turnData"]["from"]["SquareID"].Value;
        string toSquareID = n["result"]["turnData"]["to"]["SquareID"].Value;
        string turnType = n["result"]["turnType"].Value;
        
        if(turnType == TurnType.sparePawn.ToString())
        {
            Debug.LogError("Turn Type is Spare Pawn");
            isSpareTowerTurn = true;
        }
        else
        {
            isSpareTowerTurn = false;
        }

        Square newSquare = BoardManager.instance.GetSquare(fromSquareID);
        newSquare.MovePawn(BoardManager.instance.GetSquare(toSquareID));
    }

    #endregion

    public void GetAvatars()
    {
        GetAvatarList(true);
    }

    public void GetAvatarList(bool forceDownload = false)
    {
        if (!forceDownload)
        {
            if (Database.GetString(Database.Key.IMAGES_INFO) != "")
            {
                Debug.Log("File is there");

                string allImagesData = Database.GetString(Database.Key.IMAGES_INFO);
                AllImages allImages = JsonUtility.FromJson<AllImages>(allImagesData);
                for (int i = 0; i < allImages.result.Length; i++)
                {
                    Debug.Log(allImages.result[i].name);
                }
                return;
            }
        }
        Web.Create()
           .SetUrl(Configuration.Instance.GetApi(Configuration.ApiKey.AVATAR_LIST), Web.RequestType.POST, Web.ResponseType.TEXT)

           //.AddHeader("Content-Type", "application/x-www-form-urlencoded")
           .AddHeader("access_token", Database.GetString(Database.Key.ACCESS_TOKEN))
          .SetOnSuccessDelegate((Web _web, Response _response) =>
          {
              JSONNode data = JSONNode.Parse(_response.GetText());
              Database.PutString(Database.Key.IMAGES_INFO, _response.GetText());             
              _web.Close();

          })
          .SetOnFailureDelegate((Web _web, Response _response) =>
          {
              Debug.Log(_response.GetText());
              _web.Close();
          })
          .Connect();
    }


    public void ConfirmAvatarImage(string avartarID)
    {
        string newAvatarID = "ObjectId(\""  + avartarID  + "\")";
        Debug.LogError("Avatar ID selected: " + avartarID);
        Debug.LogError(Configuration.Instance.GetApi(Configuration.ApiKey.UPDATEAVATAR) + "/" + Database.GetString(Database.Key.ACCESS_TOKEN));
        Web.Create()
           .SetUrl(Configuration.Instance.GetApi(Configuration.ApiKey.UPDATEAVATAR), Web.RequestType.POST, Web.ResponseType.TEXT)
           .AddField("avatar_id", avartarID)
           .AddHeader("Content-Type", "application/x-www-form-urlencoded")
           .AddHeader("access_token", Database.GetString(Database.Key.ACCESS_TOKEN))
           
          .SetOnSuccessDelegate((Web _web, Response _response) =>
          {
              Debug.Log(_response.GetText());

              PopupCanvas.Instance.UpdatedAvatarImage(true);
              _web.Close();

          })
          .SetOnFailureDelegate((Web _web, Response _response) =>
          {
              PopupCanvas.Instance.UpdatedAvatarImage(false);
              Debug.Log(_response.GetText());
              _web.Close();
          })
          .Connect();
    }

    public IEnumerator GetAvatarImageLocal(string imageURL)
    {
        string totalUrl = imageBaseUrl + imageURL;
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(totalUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            Texture2D texture2D = ((DownloadHandlerTexture)request.downloadHandler).texture;

            yield return new WaitUntil(() => GamePlay.instance != null);
            GamePlay.instance.localPlayerDP.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);           
        }

        
    }

    public IEnumerator GetAvatarImageRemote(string imageURL)
    {
        string totalUrl = imageBaseUrl + imageURL;
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(totalUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            Texture2D texture2D = ((DownloadHandlerTexture)request.downloadHandler).texture;

            yield return new WaitUntil(() => GamePlay.instance != null);
            GamePlay.instance.opponentDP.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
        }
    }

}
