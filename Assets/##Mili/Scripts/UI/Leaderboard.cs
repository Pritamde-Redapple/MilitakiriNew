using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GWebUtility;
using UnityEngine.UI;

public class Leaderboard : UIPage
{
    public ScoreBar[] scoreBar;
    public Transform parentOfScoreBar;
    public Color oddColor;
    public Color evenColor;
    public Color thisPlayer;
    public VerticalLayoutGroup verticalLayout;
   
    private void OnEnable()
    {
        verticalLayout.enabled = false;
        GetScores();
    }

    public void GetScores()
    {
        Debug.Log("Access: "+ Database.GetString(Database.Key.ACCESS_TOKEN));
        Web.Create()
          .SetUrl("http://52.66.82.72:2095/allUsers", Web.RequestType.POST, Web.ResponseType.TEXT)
          // .AddHeader("Content-Type", "application/x-www-form-urlencoded")
          .AddField(Constants.REGISTRATION_TYPE, "normal")
          .AddHeader("access_token", Database.GetString(Database.Key.ACCESS_TOKEN))
          
          .SetOnSuccessDelegate((Web _web, Response _response) =>
          {
              Debug.Log(_response.GetText());
              string thisPlayerName = Database.GetString(Database.Key.FIRST_NAME);
              AllUserScores allUserScores = JsonUtility.FromJson<AllUserScores>(_response.GetText());
              for (int i = 0; i < allUserScores.result.userlist.Length; i++)
              {
                  Color newColor;
                 if(i % 2 == 0)
                  {
                      newColor = evenColor;
                  }
                 else
                  {
                      newColor = oddColor;
                  }

                 if(allUserScores.result.userlist[i].name == thisPlayerName)
                  {
                      newColor = thisPlayer;
                  }

                  scoreBar[i].SetScoreBar(i.ToString(), allUserScores.result.userlist[i], newColor);
                 // newScoreBar.SetScoreBar(i.ToString(), allUserScores.result.userlist[i], newColor);

                  //scoreBar[i].SetScoreBar(i.ToString(), allUserScores.result.userlist[i], newColor);
              }

              verticalLayout.enabled = true;
              _web.Close();

          })
         .SetOnFailureDelegate((Web _web, Response _response) =>
         {
             Debug.Log(_response.GetText());
             _web.Close();
         })
         .Connect();
    }

    public void GoBack()
    {
        UIManager.instance.TransitionTo(PageType.MAINMENU);
    }
}
