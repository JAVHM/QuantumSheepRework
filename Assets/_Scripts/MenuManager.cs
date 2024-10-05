using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject levelSelector;
    public GameObject credis;
    public bool subMenuActive = false;

    public GameObject audioManager;
    public GameObject BGMSlider;
    public GameObject SFXSlider;

    void Start()
    {
        audioManager = GameObject.Find("AudioManager");

        BGMSlider.GetComponent<Slider>().value = AudioManager.bgMusicVolume;
        BGMSlider.GetComponent<Slider>().onValueChanged.AddListener(audioManager.GetComponent<AudioManager>().updateBGValume);
        updateBGMusic(BGMSlider.GetComponent<Slider>().value);

        SFXSlider.GetComponent<Slider>().value = AudioManager.effectsMusicVolume;
        SFXSlider.GetComponent<Slider>().onValueChanged.AddListener(audioManager.GetComponent<AudioManager>().updateSfxVolume);
        updateSFX(SFXSlider.GetComponent<Slider>().value);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    public void updateBGMusic(float n)
    {
        audioManager.GetComponent<AudioManager>().updateBGValume(n);
    }

    public void updateSFX(float n)
    {
        audioManager.GetComponent<AudioManager>().updateSfxVolume(n);
    }

    public void Close()
    {
        optionsMenu.SetActive(false);
        levelSelector.SetActive(false);
        credis.SetActive(false);
    }

    public void ChangeScene(int n)
    {
        SceneManager.LoadScene(n);
    }

    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
