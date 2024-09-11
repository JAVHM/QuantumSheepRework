using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private bool isPaused = false;
    public bool canPause = true;

    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject victoryMenu;
    public GameObject gameoverMenu;

    public GameObject BGMSlider;
    public GameObject SFXSlider;

    private void Start()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);

        //BGMSlider.GetComponent<Slider>().value = AudioManager.bgMusicVolume;
        BGMSlider.GetComponent<Slider>().onValueChanged.AddListener(AudioManager.instance.updateBGValume);
        //updateBGMusic(BGMSlider.GetComponent<Slider>().value);

        //SFXSlider.GetComponent<Slider>().value = AudioManager.effectsMusicVolume;
        SFXSlider.GetComponent<Slider>().onValueChanged.AddListener(AudioManager.instance.updateSfxVolume);
        //updateSFX(SFXSlider.GetComponent<Slider>().value);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && canPause)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            victoryMenu.SetActive(true);
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;

        pauseMenu.SetActive(true);

        isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);

        isPaused = false;
    }

    public void GoMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void updateBGMusic(float n)
    {
        AudioManager.instance.updateBGValume(n);
    }

    public void updateSFX(float n)
    {
        AudioManager.instance.updateSfxVolume(n);
    }

    public void ChangeScene(int n)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(n);
    }

    public void NextScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameOver()
    {
        gameoverMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        canPause = false;
    }
}
