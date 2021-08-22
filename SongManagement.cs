using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SongManagement : MonoBehaviour
{
    public GameRules gamesRules;
    private bool enableGameplayMusic;
    public bool enableMainMenuMusic;
    public bool enableSoundEffects;
    public AudioClip[] backgroundGameplaySong;
    public AudioClip[] backgroundMainMenuSong;
    public AudioSource explosion;
    public AudioSource jetFire;
    public AudioSource buttonPress;
    public AudioSource buttonPressError;
    public AudioSource record;
    public AudioSource skyboxChange_nextLevel;
    public AudioSource unlockedSkin;
    public AudioSource gameplaySong;
    public AudioSource mainMenuSong;
    public int currentGameplaySong;
    public AudioSource[] SoundsEffects;
    public AudioSource[] Music;
    // Start is called before the first frame update
    void Start()
    {
        enableSoundEffects = true;
        enableMainMenuMusic = true;
        enableGameplayMusic = false;
        PlayMainMenuMusic();
    }
    public void PauseGameplaySong()//button Paused
    {
        if (gameplaySong.isPlaying)
        {
            gameplaySong.Pause();
            enableGameplayMusic = false;
        }
        else if (gameplaySong.isPlaying == false)
        {
            gameplaySong.UnPause();
            enableGameplayMusic = true;
        }
    }
    public void PlayMainMenuMusic()
    {
        if (enableMainMenuMusic == true)
        {
            mainMenuSong.clip = backgroundMainMenuSong[Random.Range(0, backgroundMainMenuSong.Length)];
            mainMenuSong.Play();
        }
    }
    public void ChangeTheGameplaySongWhenEnds()
    {
        if (gameplaySong.isPlaying == false && enableGameplayMusic == true)
        {
            currentGameplaySong++;
            gameplaySong.clip = backgroundGameplaySong[currentGameplaySong % backgroundGameplaySong.Length];
            gameplaySong.Play();
        }
    }
    public void PlayGameplayMusic(bool enable)
    {
        enableGameplayMusic = enable;
        if (enableGameplayMusic == true && gameplaySong.clip == null)
        {
            currentGameplaySong = Random.Range(0, backgroundGameplaySong.Length);
            gameplaySong.clip = backgroundGameplaySong[currentGameplaySong];
            gameplaySong.Play();
        }
        if (mainMenuSong.isPlaying)
        {
            mainMenuSong.Pause();
        }
        if (gameplaySong.isPlaying == false)
        {
            gameplaySong.UnPause();
        }
    }
    public void PlayExplosion()
    {
        if (enableSoundEffects == true)
        {
            explosion.Play();
        }
    }
    public void PlayJetfire()
    {
        if (enableSoundEffects == true)
        {
            jetFire.Play();
        }
    }
    public void PlayButtonPressed()
    {
        if (enableSoundEffects == true)
        {
            buttonPress.Play();
        }
    }
    public void PlayButtonPressedError()
    {
        if (enableSoundEffects == true)
        {
            buttonPressError.Play();
        }
    }
    public void PlayRecord()
    {
        if (enableSoundEffects == true)
        {
            record.Play();
        }
    }
    public void PlayNextLevel()
    {
        if (enableSoundEffects == true)
        {
            skyboxChange_nextLevel.Play();
        }
    }
    public void PlayUnlockedSkin()
    {
        if (enableSoundEffects == true)
        {
            unlockedSkin.Play();
        }
    }
    public void MuteSoundsEffects(bool mute)
    {
        if (mute == true)
        {
            for (int i = 0; i < SoundsEffects.Length; i++)
            {
                SoundsEffects[i].volume = 0;
            }
        }
        if (mute == false)
        {
            for (int i = 0; i < SoundsEffects.Length; i++)
            {
                SoundsEffects[i].volume = 1;
            }
        }
    }
    public void MuteMusic(bool mute)
    {
        if (mute == true)
        {
            for (int i = 0; i < Music.Length; i++)
            {
                Music[i].volume = 0;
            }
        }
        if (mute == false)
        {
            for (int i = 0; i < Music.Length; i++)
            {
                Music[i].volume = 1;
            }
        }
    }
}
