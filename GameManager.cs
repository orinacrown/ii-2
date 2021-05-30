using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public Sprite[] dispSprites;
    public SpriteRenderer currentSprite;
    public AudioClip inst_BGM;
    public AudioClip SE_I;
    public AudioClip SE_II;
    public AudioClip SE_2;
    public AudioClip end_BGM;
    public GameObject[] displays;
    public Sprite[] scoreNumSprites;
    public SpriteRenderer[] scores;

    private enum PushStatus
    {
        I,
        II,
        Two
    }

    private enum GameStatus
    {
        Title,
        Inst,
        StandBy,
        Game,
        Result,
        Count
    }
    private PushStatus status;
    private byte score;
    private DateTime startTime;
    private GameStatus gameStatus;
    private AudioSource audioSource;
    private bool oldClipPlay;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameStatus = GameStatus.Title;
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameStatus)
        {
            case GameStatus.Title:
                TitleMode();
                break;
            case GameStatus.Inst:
                InstMode();
                break;
            case GameStatus.StandBy:
                StandByMode();
                break;
            case GameStatus.Game:
                GameMode();
                break;
            case GameStatus.Result:
                ResultMode();
                break;
            default:
                break;
        }
        for(int i = 0; i < (int)GameStatus.Count; i++)
        {
            bool isActive = (int)gameStatus == i;
            displays[i].SetActive(isActive);
        }
    }

    private void TitleMode()
    {
        displays[(int)GameStatus.Title].SetActive(true);
        if (Input.GetKeyDown(KeyCode.Return) == true)
        {
            gameStatus = GameStatus.Inst;
            status = PushStatus.I;
            score = 0;
            audioSource.Stop();
            audioSource.PlayOneShot(inst_BGM);
        }
    }

    private void InstMode()
    {
        displays[(int)GameStatus.Inst].SetActive(true);
        if ((oldClipPlay==true) && (audioSource.isPlaying == false))
        {
            gameStatus = GameStatus.StandBy;
        }
        oldClipPlay = audioSource.isPlaying;
    }

    private void StandByMode()
    {
        displays[(int)GameStatus.StandBy].SetActive(true);
        if (Input.GetKeyDown(KeyCode.I) == true)
        {
            score++;
            startTime = DateTime.Now;
            status = PushStatus.II;
            gameStatus = GameStatus.Game;
        }
        SpriteUpdate();
    }
    private void GameMode()
    {
        displays[(int)GameStatus.Game].SetActive(true);
        DateTime now = DateTime.Now;
        TimeSpan span = now - startTime;
        if (span.TotalMilliseconds >= 2000)
        {
            gameStatus = GameStatus.Result;
            audioSource.Stop();
            audioSource.PlayOneShot(end_BGM);
            return;
        }
        StatusUpdate();
        SpriteUpdate();
    }

    private void ResultMode()
    {
        displays[(int)GameStatus.Result].SetActive(true);
        byte score_10;
        byte score_1;
        score_10 = (byte)(score / 10);
        score_1 = (byte)(score % 10);
        scores[0].sprite = scoreNumSprites[score_10];
        scores[1].sprite = scoreNumSprites[score_1];
        if (Input.GetKeyDown(KeyCode.Return) == true)
        {
            gameStatus = GameStatus.Title;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            naichilab.UnityRoomTweet.Tweet("ii-2", $"私の連打力は{score}です", "ii2", "unity1week");
        }
    }

    private void StatusUpdate()
    {
        PushStatus t_status = status;
        switch (status)
        {
            case PushStatus.I:
                if (Input.GetKeyDown(KeyCode.I) == true)
                {
                    t_status = PushStatus.II;
                    audioSource.Stop();
                    audioSource.PlayOneShot(SE_I);
                }
                break;
            case PushStatus.II:
                if (Input.GetKeyDown(KeyCode.I))
                {
                    t_status = PushStatus.Two;
                    audioSource.Stop();
                    audioSource.PlayOneShot(SE_II);
                }
                break;
            case PushStatus.Two:
                if ((Input.GetKeyDown(KeyCode.Keypad2) == true) || (Input.GetKeyDown("2") == true))
                {
                    t_status = PushStatus.I;
                    audioSource.Stop();
                    audioSource.PlayOneShot(SE_2);
                }
                break;
            default:
                break;
        }
        if (t_status != status)
        {
            score++;
            if (score > 99)
            {
                score = 99;
            }
            status = t_status;
        }
    }

    private void SpriteUpdate()
    {
        currentSprite.sprite = dispSprites[(int)status];
    }
}
