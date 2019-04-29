using DitzeGames.Effects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{


    protected Camera Camera;


    public SStats Stats;
    public struct SStats
    {
        public int BestLevel;
        public int Level;
        public int Heart;
        public int Bomb;
        public int Sheels;
    }

    protected SPowers Powers;
    protected struct SPowers
    {
        public int Heart;
        public int Bomb;
        public int FastReload;
        public int Shells;
        public int SlowMotion;
    }

    [Header("GameUI")]
    public Image Curor;
    public Text UIText;

    public Image[] Sheels;
    public Image HeartFill;
    public Image BombFill;
    public Text HeartTxt;
    public Text BombTxt;
    public Text GameOver;
    public Text LevelUpText;

    [Header("MainGameUI")]
    public Text MainLevel;
    public Text MainLivesText;

    public Text MainBombsText;
    public Button MainBombsPlus;
    public Button MainBombsMinus;

    public Text MainFastReloadText;
    public Button MainFastReloadPlus;
    public Button MainFastReloadMinus;

    public Text MainShellsText;
    public Button MainShellsPlus;
    public Button MainShellsMinus;

    public Text MainSlowMotionText;
    public Button MainSlowMotionMinus;
    public Button MainSlowMotionPlus;


    [Header("Audio")]
    public AudioClip ReloadAudio;
    public AudioClip ShotAudio;
    public AudioClip BombAudio;
    public AudioClip ClickAudio;

    public Canvas MainCanvas;
    public Canvas GameCanvas;

    public bool InMenu;

    protected BirdSpawner BirdSpawner;
    protected float NextReload;

    public PostProcessVolume MenuPP;
    public PostProcessVolume GamePP;

    // Start is called before the first frame update
    void Start()
    {
        BirdSpawner = GetComponent<BirdSpawner>();

        Stats.BestLevel = PlayerPrefs.GetInt("BestLevel", 1);

        Camera = Camera.main;
        UpdateGameUI();

        //Init
        Powers.Heart = 20;
        Powers.Bomb = 0;
        Powers.FastReload = 0;
        Powers.Shells = 4;
        Powers.SlowMotion = 0;

        StartCoroutine(StartMenu());

        LevelUpText.gameObject.SetActive(false);
    }

    private void StartGame()
    {
        InMenu = false;
        NextReload = 0;
        Stats.Level  = 1;
        Stats.Bomb   = Powers.Bomb;
        Stats.Heart  = Powers.Heart;
        Stats.Sheels = Powers.Shells;
        GameOver.gameObject.SetActive(false);
        StartLevel();
    }

    private void StartLevel()
    {
        BirdSpawner.StartSpawning(2 + Stats.Level * 2, Stats.Level / 3f);
        UpdateGameUI();
    }

    private IEnumerator StartMenu(float wait = 0f)
    {
        yield return new WaitForSeconds(wait);

        InMenu = true;
        UpdateMainUI();
        GameOver.gameObject.SetActive(false);

        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = InMenu;
        Curor.transform.position = Input.mousePosition;

        GameCanvas.gameObject.SetActive(!InMenu);
        MainCanvas.gameObject.SetActive(InMenu);
        GamePP.gameObject.SetActive(!InMenu);
        MenuPP.gameObject.SetActive(InMenu);

        if (!InMenu && Stats.Heart > 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Stats.Sheels == 0)
                {
                    AudioSource.PlayClipAtPoint(ClickAudio, transform.position);
                }
                else
                {
                    if (Stats.Sheels == Powers.Shells)
                        NextReload = 2f / (Powers.FastReload + 1);
                    var hits = Physics.RaycastAll(Camera.ScreenPointToRay(Input.mousePosition));
                    var birdHit = false;
                    for (var i = 0; i < hits.Length; i++)
                    {
                        var bird = hits[i].collider.GetComponent<Bird>();
                        birdHit = birdHit || bird != null;
                        if (bird != null)
                            bird.SetDead();
                    }

                    if (birdHit)
                    {
                        CameraEffects.ShakeOnce(0.5f);
                        ScoreIteration();
                        UpdateGameUI();
                    }
                    else
                        CameraEffects.ShakeOnce(0.05f);

                    AudioSource.PlayClipAtPoint(ShotAudio, transform.position);
                    Stats.Sheels--;
                    UpdateGameUI();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space) && Stats.Bomb > 0)
            {
                AudioSource.PlayClipAtPoint(BombAudio, transform.position);
                Stats.Bomb--;
                CameraEffects.ShakeOnce(0.5f);
                var birds = new List<Bird>(Bird.AllBirds); //copy because all birds change in the loop
                for (int i = 0; i < birds.Count; i++)
                {
                    birds[i].SetDead();
                    ScoreIteration();
                }
                UpdateGameUI();
            }

            Reload();
        }

    }

    private void Reload()
    {

        if (NextReload > 0)
        {
            NextReload -= Time.deltaTime;
            return;
        }

        if (Stats.Sheels == Powers.Shells)
            return;

        AudioSource.PlayClipAtPoint(ReloadAudio, transform.position);
        Stats.Sheels++;
        NextReload = 1f / (Powers.FastReload + 1);
        UpdateGameUI();

    }

    private void ScoreIteration()
    {
        if (BirdSpawner.BirdsLeftToSpawn == 0 && Bird.AllBirds.Count == 0)
            StartCoroutine(LevelUp());
    }

    private void UpdateGameUI()
    {
        UIText.text = $"Level: { Stats.Level }\nBirds left: { BirdSpawner.BirdsLeftToSpawn + Bird.AllBirds.Count }";
        BombFill.fillAmount = Stats.Bomb > 0 ? 1f : 0f;
        HeartFill.fillAmount = (Powers.Heart == 0) ? 0 : Stats.Heart * 1f / Powers.Heart;
        HeartTxt.text = $"{ Stats.Heart }";
        BombTxt.text = $"{ Stats.Bomb }";
        for (int i = 0; i < Sheels.Length; i++)
            Sheels[i].gameObject.SetActive(i < Stats.Sheels);
    }

    private void UpdateMainUI()
    {
        MainLevel.text = $"Best Level: { Stats.BestLevel }";
        MainLivesText.text = $"{ Powers.Heart }";

        MainBombsText.text = Powers.Bomb.ToString();
        MainBombsMinus.interactable = (Powers.Bomb > 0);
        MainBombsPlus.interactable = (Powers.Heart > 1);

        MainFastReloadText.text = Powers.FastReload.ToString();
        MainFastReloadMinus.interactable = (Powers.FastReload > 0);
        MainFastReloadPlus.interactable = (Powers.Heart > 1);

        MainShellsText.text = Powers.Shells.ToString();
        MainShellsMinus.interactable = (Powers.Shells == 8);
        MainShellsPlus.interactable = (Powers.Heart > 1 && Powers.Shells == 4);

        MainSlowMotionText.text = Powers.SlowMotion.ToString();
        MainSlowMotionMinus.interactable = (Powers.SlowMotion == 1);
        MainSlowMotionPlus.interactable = (Powers.Heart > 1 && Powers.SlowMotion == 0);
    }

    public IEnumerator LevelUp()
    {
        Stats.Level++;
        Stats.BestLevel = Mathf.Max(Stats.BestLevel, Stats.Level);
        PlayerPrefs.SetInt("BestLevel", Stats.BestLevel);
        PlayerPrefs.Save();

        //Animation
        for (int i = 0; i < 10; i++)
        {
            LevelUpText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            LevelUpText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }

        //Start Spawning
        StartLevel();

    }

    internal void BirdMiss()
    {
        Stats.Heart--;
        Stats.Heart = Mathf.Max(0, Stats.Heart);

        if(Stats.Heart == 0)
        {
            //GameOver
            GameOver.gameObject.SetActive(true);
            StartCoroutine(StartMenu(3f));
        }
        else
        {
            ScoreIteration();
        }

        UpdateGameUI();
    }

    public void ChangeBomb(int c)
    {
        Powers.Bomb += c;
        Powers.Heart -= c;
        UpdateMainUI();
    }

    public void ChangeReload(int c)
    {
        Powers.FastReload += c;
        Powers.Heart -= c;
        UpdateMainUI();
    }

    public void ChangeShells(int c)
    {
        Powers.Shells += c;
        Powers.Heart -= c / 4;
        UpdateMainUI();
    }

    public void ChangeSlowMotion(int c)
    {
        Powers.SlowMotion += c;
        Powers.Heart -= c;
        UpdateMainUI();
    }

    public void ResetLevel()
    {
        PlayerPrefs.SetInt("BestLevel", 1);
        PlayerPrefs.Save();
        UpdateMainUI();
    }

    public void Go()
    {
        StartGame();
    }
}
