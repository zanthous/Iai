using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance = null;

    private int score = 0;
    private int currentLevel;
    private float timer;

    public int Score { get => score; set => score = value; }
    public int CurrentLevel { get => currentLevel; set => currentLevel = value; }
    public float Timer { get => timer; set => timer = value; }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this);
    }
    
}
