using System;
using UnityEngine;
using UnityEngine.UI;


public class UserData: MonoBehaviour
{
    [SerializeField]
    private string userName;
    [SerializeField]
    private int userId;
    [SerializeField]
    private int score;
    [SerializeField]
    private Color32 playerColor = Color.white;
    [SerializeField]
    private Image[] playerIcons;

    public string UserName
    {
        get { return this.userName; }
        set { this.userName = value; }
    }

    public int UserId
    {
        get { return this.userId; }
        set { this.userId = value; }
    }

    public int Score
    {
        get { return this.score; }
        set { this.score = value; }
    }

    public Color32 PlayerColor
    {
        get { return this.playerColor; }
        set { this.playerColor = value; }
    }

    public Image[] PlayerIcons
    {
        get { return this.playerIcons; }
        set { this.playerIcons = value; }
    }

}
