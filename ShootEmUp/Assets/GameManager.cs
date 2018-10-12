using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static new Camera camera;

    /// <summary>
    /// To hold the music state for when the scene reloads
    /// </summary>
    static bool musicMuted = false;

    [SerializeField]
    AudioSource Music;

    [Tooltip("The audio to play on level up")]
    [SerializeField]
    AudioSource LevelUp;

    [Tooltip("The object to activate on pause")]
    [SerializeField]
    GameObject PauseMenu;

    [Tooltip("The object to activate on game over")]
    [SerializeField]
    GameObject GameOverIndicator;

    [SerializeField]
    Camera MainCamera;

    [HideInInspector]
    public int Score = 0;

    [SerializeField]
    Equation LevelCurve;

    [HideInInspector]
    public int Level;

    [Tooltip("The GUI text that should show score and level")]
    [SerializeField]
    TextMeshProUGUI ScoreShower;

    bool gameOver;

    string MainScene;

    float LastScoreAdd;

    void Awake()
    {
        Music.mute = musicMuted;

        camera = MainCamera;
        instance = this;
        MainScene = SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// When the player dies:
    /// </summary>
    public static void GameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        instance.gameOver = true;

        instance.GameOverIndicator.SetActive(true);

        Time.timeScale = 0f;
    }

    // Use this for initialization
    void Start()
    {
        LastScoreAdd = Time.fixedTime;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void FixedUpdate()
    {
        //Progressively increases score
        if (Time.fixedTime - LastScoreAdd >= 2f)
        {
            LastScoreAdd = Time.fixedTime;
            Score++;
        }
    }
    // Update is called once per frame
    void Update()
    {
        //Handles paused controls
        if (Time.timeScale != 0f)
        {
            //Handles Level ups and score
            int lvl = (int)LevelCurve.GetValue(Score);
            if (Level != lvl)
            {
                Level = lvl;
                LevelUp.Play();
            }
            ScoreShower.text = "Score: " + Score + "   Level: " + Level;
            

            //Handles Pause input
            if (Input.GetButtonDown("Cancel"))
            {
                Music.volume = 0.25f;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                Time.timeScale = 0f;

                if (!PauseMenu.activeSelf) PauseMenu.SetActive(true);
            }
        }
        else
        {
            //Resumes game case it is paused and "Cancel" is pressed
            if (!gameOver && Input.GetButtonDown("Cancel"))
            {
                Music.volume = 1f;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                Time.timeScale = 1;

                if (PauseMenu.activeSelf) PauseMenu.SetActive(false);
            }
            //Restarts Game case R is pressed
            if (Input.GetKeyDown(KeyCode.R))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(MainScene, LoadSceneMode.Single);
            }
        }
        //mutes music case M is pressed
        if (Input.GetKeyDown(KeyCode.M))
        {
            Music.mute = !Music.mute;
            musicMuted = Music.mute;
        }
        //puts game in full screen case F is pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }
}
