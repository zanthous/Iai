using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum Level
{
    farm,
    dungeon
}


public class GameManager : MonoBehaviour
{
    //This script essentially will move the player along the different scenes in one way or another
    //and enable/disable the PlaneUsageExample [name subject to change] scripts as necessary
    //or some equivalent

    public enum SliceInfo
    {
        Miss,
        Katsu,
        Geki
    }

    public static Action<List<BoxCollider>, int> SliceResult;
    public static Action EndLevel;

    //TODO fix capitalization (which would break references..) 
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private float LevelLoopTimeSpeedupFactor = 0.15f;
    [SerializeField] private float CleanCutThreshold = 0.4f;
    [SerializeField] private Animator X;
    [SerializeField] private Animator Geki;
    [SerializeField] private Animator Katsu;
    [SerializeField] private GameObject PlayerRef;

    [SerializeField] private List<GameObject> SliceHandlerZones;
    [SerializeField] private List<GameObject> PlayerSpawnPositions;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private SliceInfo sliceInfo = SliceInfo.Miss;
    private int currentZone = 0;
    private GameData data;

    private bool waitingForContinue = false;
    
    private void Awake()
    {
        SliceResult += HandleSlice;
        EndLevel += DoEndLevel;
        PlayerRef.transform.position = PlayerSpawnPositions[0].transform.position;
        foreach(GameObject g in SliceHandlerZones)
        {
            g.SetActive(false);
        }
        SliceHandlerZones[0].SetActive(true);
    }

    private void Start()
    {
        data = GameData.instance;
        data.Timer = 0;
        UpdateScoreText();
    }

    private void Update()
    {
        data.Timer += Time.deltaTime;

        if(waitingForContinue && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(1);
        }
        //Debug.Log(timer);
    }
    private void OnDestroy()
    {
        SliceResult -= HandleSlice;
        EndLevel -= DoEndLevel;
    }
    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + data.Score.ToString();
    }

    private void DoEndLevel()
    {
        switch(sliceInfo)
        {
            case SliceInfo.Miss:
                X.gameObject.SetActive(true);
                X.Play("ScorePopup",-1,0);
                StartCoroutine(CompleteAnimationAndChangeZoneAndScene(X, sliceInfo));
                //TODO: End game text
                break;
            case SliceInfo.Katsu:
                Katsu.gameObject.SetActive(true);
                Katsu.Play("ScorePopup", -1, 0);
                data.Score += (int)(100 * Settings.GlobalTimeScale);
                StartCoroutine(CompleteAnimationAndChangeZoneAndScene(Katsu, sliceInfo));
                break;
            case SliceInfo.Geki:
                Geki.gameObject.SetActive(true);
                Geki.Play("ScorePopup", -1, 0);
                data.Score += (int)(200 * Settings.GlobalTimeScale);
                StartCoroutine(CompleteAnimationAndChangeZoneAndScene(Geki, sliceInfo));
                break;
            default:
                Debug.Log("what");
                break;
        }
    }

    private IEnumerator CompleteAnimationAndChangeZoneAndScene(Animator anim, SliceInfo sliceInfo)
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        UpdateScoreText();
        ChangeZoneAndScene(sliceInfo);
    }

    private void ChangeZoneAndScene(SliceInfo sliceInfo)
    {
        if(sliceInfo == SliceInfo.Miss)
        {
            gameOverPanel.SetActive(true);
            finalScoreText.text = "Final score: " + data.Score.ToString();
            Time.timeScale = 0.0f;
            data.CurrentLevel = 0;
            data.Timer = 0.0f;
            data.Score = 0;
            Settings.GlobalTimeScale = 1.0f;
            waitingForContinue = true;
            return;
        }

        currentZone++;
        if(currentZone >= PlayerSpawnPositions.Count)
        {
            //Go to next scene or loop back to first and increase global timescale
            var nLevels = Settings.nScenes;

            //+1 since menu is 0
            data.CurrentLevel++;

            //Loop levels and speedup on loop
            if(data.CurrentLevel >= nLevels)
            {
                //Speedup
                Settings.GlobalTimeScale += LevelLoopTimeSpeedupFactor;
                //Go back to first level (scene)
                data.CurrentLevel = 0;
                Time.timeScale = 1.0f + Settings.GlobalTimeScale;

            }
            SceneManager.LoadScene(data.CurrentLevel + 1);
        }
        else
        {
            //TODO better transition
            SliceHandlerZones[currentZone - 1].SetActive(false);
            SliceHandlerZones[currentZone].SetActive(true);
            PlayerRef.transform.position = PlayerSpawnPositions[currentZone].transform.position;
        }
    }

    private void HandleSlice(List<BoxCollider> colliders, int targetCount)
    {
        if(colliders.Count <= 0 || colliders.Count / 2 < targetCount)
        {
            sliceInfo = SliceInfo.Miss;
            return; 
        }
            
        Debug.Log(colliders.Count / 2 + " Objects cut");

        sliceInfo = SliceInfo.Geki;
        for(int i = 0; i < colliders.Count; i+=2)
        {
            var volume1 = colliders[i].bounds.size.x * colliders[i].bounds.size.y * colliders[i].bounds.size.z;
            var volume2 = colliders[i+1].bounds.size.x * colliders[i + 1].bounds.size.y * colliders[i + 1].bounds.size.z;

            var result = volume1 > volume2 ? volume1 / volume2 : volume2 / volume1;

            if(result >  1.0f + CleanCutThreshold)
            {
                Debug.Log("Bad cut, result difference: " + result);
                sliceInfo = SliceInfo.Katsu;
            }
            else
            {
                Debug.Log("Clean cut, result difference: " + result);
                //sliceInfo = SliceInfo.Geki;
            }
        }
    }
}
