using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


public enum GameEmits { adduser, gamerequest, playerReady, pawnplacements, leave_room, message_received, turnSubmited, turnstart, sparePawnTurn, newmessage, gameOver, leave };
public enum GameListen { user_connected, connected_room, enter_user, gameinit, gamestart, initialpawnplacements, turndata, playerturn, turn_start, leave_room, game_winner, sparePawnData, message_received };

public enum TurnType { normalPawn, sparePawn};

public enum AudioSetType { MUSIC, SOUND};
public enum AnimationType { TILT, VAPORIZE, BURN, BURST, PIXEL , RANDOM };

public enum EffectType { PIXEL, VAPORIZE, BURN, BURST};
public static class Constants {

    #region Server Variables
    public static string SocketURL = "http://52.66.82.72:2095/";//"http://192.168.2.66:3009/";//"http://18.219.52.107:3009/";
    public static string PASSWORD = "password";
    public static string NAME = "name";
    public static string USER_NAME = "username";

    public static string TRUE = "true";
    public static string FALSE = "false";
    public static string GENDER = "gender";
    public static string REGISTRATION_TYPE = "registration_type";


    public static string defaultImagePath = "http://52.66.9.228:3000/images/avatar/defaultimage.png";

    #endregion

   

    public static bool isAI = true;
    public static int noOfSquarePerRow = 6;
    public static int threeRowsSetupTime = 300;
    public static int threeRowsSetupWarning = 240;
    public enum SquareState { MOVABLE, CLICKABLE, EMPTY, OCCUPIED, SELECTED, DESELECTED};
    public enum PlayerType
    {
        LOCAL,
        REMOTE,
        NONE
    };
    public enum BoardType
    {
        SINGLE,
        DOUBLE
    }
    public enum ViewType
    {
        TWO_D,
        THREE_D
    };
    public enum LevelType
    {
        EASY,
        MEDIUM,
        HARD
    }

    public enum PlayerTag
    {
        PLAYER_1,
        PLAYER_2
    }

    public static ViewType currentViewType;
    public static LevelType currentLevelType;
    public static BoardType currentBoardType;
    public static int TotalTurnPerBoard = 50;
    public static float PAWN_DUMP_SPEED = 0.8f;

    public static string SAVEIMAGES = "saveimagesinfo";
    public static int GetMaximumRank(Pawn.PawnType type)
    {
        switch(type)
        {
            case Pawn.PawnType.STAR:
                return 4;
            default:
                return 3;
        }
    }

    public static int GetPercentageForAILevel()
    {
        switch (currentLevelType)
        {
            case LevelType.EASY:
                return 40;
            case LevelType.MEDIUM:
                return 15;
            case LevelType.HARD:
                return 0;
        }
        return 100;
    }
    public static int GetPercentageForDetectingOpponent()
    {
        switch (currentLevelType)
        {
            case LevelType.EASY:
                return 50;
            case LevelType.MEDIUM:
                return 75;
            case LevelType.HARD:
                return 100;
        }
        return 100;
    }

    public static int GetMaximumDistanceCanTowerPawnMove()
    {
        switch (currentLevelType)
        {
            case LevelType.EASY:
                return 15;
            case LevelType.MEDIUM:
                return 7;
            case LevelType.HARD:
                return 4;
        }
        return 4;
    }

    #region Multiplayer
   
    #endregion

}
[Serializable]
public class PawnAndSquareMaterialInfo
{
    public List<int> indexforPawnAndSquare;

    public PawnAndSquareMaterialInfo()
    {
        indexforPawnAndSquare = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            indexforPawnAndSquare.Add(0);
        }
    }
}
[Serializable]
public class PawnMovementData
{
    public int fromSquareId;
    public List<int> fromSquarePawnId;
    public int toSquareId;
    public List<int> toSquarePawnId;
    public int removedPawnId;

    public PawnMovementData(int fromSquareId, List<int> fromSquarePawnId, int toSquareId, List<int> toSquarePawnId, int removedPawnId)
    {
        this.fromSquareId = fromSquareId;
        this.fromSquarePawnId = fromSquarePawnId;
        this.toSquareId = toSquareId;
        this.toSquarePawnId = toSquarePawnId;
        this.removedPawnId = removedPawnId;
    }
}
[Serializable]
public class SquareWithDistance
{
    public Square fromSquare;
    public Square square;
    public int distance;

    public SquareWithDistance(Square fromSquare,Square square, int distance)
    {
        this.fromSquare = fromSquare;
        this.square = square;
        this.distance = distance;
    }
}
[Serializable]
public class PossibleMoveData
{
    public List<SquareWithDistance> possibleMoves;
    public List<SquareWithDistance> possibleTakes;

    public PossibleMoveData()
    {
        possibleMoves = new List<SquareWithDistance>();
        possibleTakes = new List<SquareWithDistance>();
    }
}
[Serializable]
public class SquareWithTowerPawn
{
    public Pawn pawn;
    public Square square;
    public int distance;

    public SquareWithTowerPawn(Pawn pawn, Square square, int distance)
    {
        this.pawn = pawn;
        this.square = square;
        this.distance = distance;
    }
}

[Serializable]
public class PawnPlaced
{
    public List<int> positions = new List<int>();

    public void AddSquareID(int id)
    {
        positions.Add(id); ;
    }
}

//[Serializable]
//public class TurnData
//{
//    public int from;
//    public int to;

//    public TurnData(int from, int to)
//    {
//        this.from = from;
//        this.to = to;
//    }
//}

#region GameInitCallBack
/*
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
* 
* 
* */ 
#endregion
[Serializable]
public class GameInitInfo
{

}

[Serializable]
public struct PawnPlacements
{
    public string room_name;
    public string room_id;
    public string user_id;

    public List<GridData> GridDatas;  

    public PawnPlacements(string room_name, string room_id, string user_id, List<GridData> gridDatas)
    {
        this.room_name = room_name;
        this.room_id = room_id;
        this.user_id = user_id;
        GridDatas = gridDatas;
    }
}
[Serializable]
public struct GridData
{
    public string SquareID;
    public string PawnType;
    public string PawnRank;

    public GridData(string squareType, string pawnID, string pawnRank)
    {
        SquareID = squareType;
        PawnType = pawnID;
        PawnRank = pawnRank;
    }
}

[Serializable]
public struct FirstPawnData
{
   public PawnPlacements[] pawnlist;

    public FirstPawnData(PawnPlacements[] pawnlist)
    {
        this.pawnlist = pawnlist;
    }
}


[Serializable]
public struct TurnData
{
    public GridData from;
    public GridData to;

    public TurnData(GridData from, GridData to)
    {
        this.from = from;
        this.to = to;
    }
}

[Serializable]
public struct TurnDataWrapper
{
    public string room_name;
    public string room_id;
    public string user_id;
    public string turnType;
    public TurnData turnData;

    public TurnDataWrapper( string room_id, string user_id, TurnData turnData, string room_name = "", string turnType = "normalPawn")
    {
        this.room_name = room_name;
        this.room_id = room_id;
        this.user_id = user_id;
        this.turnData = turnData;
        this.turnType = turnType;
    }
}

[Serializable]
public class GetTurnData
{

    //"message":"Your oponent turn data",
    public TurnData turnData;

    public GetTurnData(TurnData turnData)
    {
        this.turnData = turnData;
    }
}
/*
 * 
 * {
"status":"1",
"chat":{
"id":"5d68ef84b759572749c636fb",
"sender_id":"5cf64d845732bb2515e975d0",
"receiver_id":"5c335181e4cbb927887d42d8",
"room_id":"5d68ef12b759572749c636f8",
"message":"Hello"
},
"message":"New message has been received"
}
 * 
 * 
 */

[Serializable]
public class ChatClass
{
    public string status;
    public string message;
    public Chat chat;

    public ChatClass(Chat chat, string status ="1", string message = "" )
    {
        this.status = status;
        this.message = message;
        this.chat = chat;
    }
}
[Serializable]
public class Chat
{
    public string id;
    public string sender_id;
    public string receiver_id;
    public string room_id;
    public string message;

    public Chat(string sender_id, string receiver_id, string room_id, string message, string id = "")
    {
        this.id = id;
        this.sender_id = sender_id;
        this.receiver_id = receiver_id;
        this.room_id = room_id;
        this.message = message;
    }
}

/*
 *  "_id": "5c0e4145a02ad62a8f371ced",
            "name": "avatar_11",
            "image": "avatar_11.png",
            "status": 1

 * 
 */
[System.Serializable]
public class ImageInfo
{
    public string _id;
    public string name;
    public string image;
    public string status;
}
[System.Serializable]
public class AllImages
{
    public string status;
    public string image_path;
    public ImageInfo[] result;

    public AllImages(ImageInfo[] avatarImages)
    {
        this.result = avatarImages;
    }
}

[System.Serializable]
public class UserScores
{
    public string registration_type;
    public string score;
    public string win;
    public string lost;
    public string draw;
    public string forfeit;
    public string id;
    public string name;
    public string rank;
}

[System.Serializable]
public class UserScoreWrapper
{
    public UserScores[] userlist;
}

[System.Serializable]
public class AllUserScores
{
    public string status;
    public UserScoreWrapper result;
}


[System.Serializable]
public class PlayerStats
{
    public string registration_type;
    public int score;
    public int win;
    public int lost;
    public int draw;
    public int forfeit;
    public string _id;
    public string name;
    public string username;
    public string rank;
}

[System.Serializable]
public class SearchFriendsData
{
    public PlayerStats[] userlists;
}






