using UnityEngine;
using System;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public Sound[] musics;
    public Sound[] sfxs;
    public static AudioManager instance;
    public static float bgMusicVolume = .5f;
    public static float effectsMusicVolume = .5f;
    Sound actualBGM;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        foreach (Sound s in musics)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        foreach (Sound s in sfxs)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    private void Start()
    {
        PlayMusic("Menu Theme Test");
    }

    public void PlaySfx(string name)
    {
        Sound s = Array.Find(sfxs, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("No se encontró el audio!");
            return;
        }
        s.source.Play();
    }

    public void PlaySfx(List<string> names)
    {
        if (names == null || names.Count == 0)
        {
            Debug.LogError("La lista de nombres está vacía o es nula!");
            return;
        }

        // Seleccionar aleatoriamente un nombre de la lista
        string randomName = names[UnityEngine.Random.Range(0, names.Count)];

        // Buscar el sonido con ese nombre
        Sound s = Array.Find(sfxs, sound => sound.name == randomName);
        if (s == null)
        {
            Debug.LogError($"No se encontró el audio con el nombre {randomName}!");
            return;
        }

        s.source.Play();
    }

    public void PlayMusic(string name)
    {
        actualBGM = Array.Find(musics, bgmSounds => bgmSounds.name == name);
        if (actualBGM == null)
        {
            Debug.LogError("No se encontró el audio! " + name);
            return;
        }
        actualBGM.source.Play();
    }
    public void updateBGMusic(string newTheme)
    {
        if (actualBGM.name != newTheme)
        {
            actualBGM.source.Stop();
            PlayMusic(newTheme);
            updateBGValume(bgMusicVolume);
        }
    }
    public void updateBGValume(float volume)
    {
        //print("updateBGValume");
        bgMusicVolume = volume;
        actualBGM.source.volume = volume;
    }
    public void updateSfxVolume(float volume)
    {
        effectsMusicVolume = volume;
        foreach (Sound s in sfxs)
        {
            s.source.volume = volume;
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            updateBGMusic("Menu Theme Test");
        }
        else
        {

            updateBGMusic("Main Theme Test");
        }
    }

}
