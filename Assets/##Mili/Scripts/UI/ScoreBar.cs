using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScoreBar : MonoBehaviour
{
    
    public TextMeshProUGUI win;
    public TextMeshProUGUI lost;
    public TextMeshProUGUI draw;
    public TextMeshProUGUI forfeit;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI rank;
    public TextMeshProUGUI serialNo;

    public void SetScoreBar(string number,string score, string win, string lost, string draw, string forfeit, string playerName, string rank)
    {
        this.serialNo.text = number;       
        this.win.text = win;
        this.lost.text = lost;
        this.draw.text = draw;
        this.forfeit.text = forfeit;
        this.playerName.text = playerName;
        this.rank.text = rank;
    }

    public void SetScoreBar(string number, UserScores userScores, Color thisColor)
    {
        this.serialNo.text = number;
        
        this.win.text = userScores.win;
        this.lost.text = userScores.lost;
        this.draw.text = userScores.draw;
        this.forfeit.text = userScores.forfeit;
        this.playerName.text = userScores.name;
        this.rank.text = userScores.rank;

        Image[] allImages = GetComponentsInChildren<Image>();
        for (int i = 0; i < allImages.Length; i++)
        {
            allImages[i].color = thisColor;
        }

        gameObject.SetActive(true);
    }
}
