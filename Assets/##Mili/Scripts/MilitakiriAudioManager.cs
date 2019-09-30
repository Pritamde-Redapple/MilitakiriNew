using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Hellmade.Sound;
using UnityEngine.SceneManagement;
using System;

public class MilitakiriAudioManager : Singleton<MilitakiriAudioManager>
{
    
    public AudioFiles[] gameAudioFiles;
    private Dictionary<AudioTag, AudioClip> dict_AudioClips = new Dictionary<AudioTag, AudioClip>();

    public float music_Volume;
    public float sound_Volume;

    int currentMusicId=0;
    int currentSoundId = 0;


    public static UnityAction SparePawnSound;
    public static UnityAction<int> OnRankChanged;
    public static UnityAction OnTableDrop;
    public static UnityAction OnPawnKnocked;
    public static UnityAction<int> OnWon;
    public static UnityAction<int> OnLost;
    public static UnityAction<int> OnGameFinished;

    public static UnityAction<float> TimeEvents;
   
    // Start is called before the first frame update
    void Start()
    {
        SparePawnSound += SparePawnSoundEffect;
        BoardManager.OnActiveEndRule += EndGameOnSound;
        BoardManager.OnDeactiveEndRule += EndGameOffSound;
        OnRankChanged += RankChanged;
        OnTableDrop += PawnDropped;
        OnPawnKnocked += PawnKnocked;
        OnGameFinished += GameFinished;
        TimeEvents += OnTimeEvents;
    }

  

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SparePawnSound -= SparePawnSoundEffect;
        BoardManager.OnActiveEndRule -= EndGameOnSound;
        BoardManager.OnDeactiveEndRule -= EndGameOffSound;
        OnRankChanged -= RankChanged;
        OnTableDrop -= PawnDropped;
        OnPawnKnocked -= PawnKnocked;
        OnGameFinished -= GameFinished;
    }


    private void GameFinished(int state)
    {
        if (state == 1)
        {
            PlaySound(AudioTag.WIN_GAME);
        }
        else if (state == 0)
        {
            PlaySound(AudioTag.DRAW_GAME);
        }

        else if (state == -1)
        {
            PlaySound(AudioTag.LOSE_GAME);
        }
    }

    private void OnTimeEvents(float currentTime)
    {
        int newTime = (int)currentTime;
        if(newTime == 60)
        {
            PlaySound(AudioTag.ONE_MINUTE_REMAINING);
        }
        else if(newTime == 50)
        {
            PlaySound(AudioTag.FIFTY);
        }
        else if (newTime == 40)
        {
            PlaySound(AudioTag.FOURTY);
        }
        else if (newTime == 30)
        {
            PlaySound(AudioTag.THIRTY);
        }
        else if (newTime == 20)
        {
            PlaySound(AudioTag.TWENTY);
        }
        else if (newTime == 10)
        {
            PlaySound(AudioTag.TEN);
        }
        else if (newTime == 3)
        {
            PlaySound(AudioTag.THREE);
        }
        else if (newTime == 2)
        {
            PlaySound(AudioTag.TWO);
        }
        else if (newTime == 1)
        {
            PlaySound(AudioTag.ONE);
        }
        else if (newTime == 0)
        {
            PlaySound(AudioTag.ZERO);
        }

    }

    private void PawnKnocked()
    {
        PlaySound(AudioTag.PAWN_KNOCK);
    }

    private void PawnDropped()
    {
        PlaySound(AudioTag.TABLE_DROP);
    }

    private void RankChanged(int state)
    {
        if(state == 1)
        {
            PlaySound(AudioTag.RANK_UP);
        }
        else
        {
            PlaySound(AudioTag.RANK_UP);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        music_Volume = PlayerPrefs.GetFloat(AudioSetType.MUSIC.ToString(), 1);
        sound_Volume = PlayerPrefs.GetFloat(AudioSetType.SOUND.ToString(), 1);
        for (int i = 0; i < gameAudioFiles.Length; i++)
        {
            dict_AudioClips.Add(gameAudioFiles[i].audioTag, gameAudioFiles[i].audioClip);
        }
       
       // PlayMusic();
    }

   

    public void PlayMusic()
    {

        currentMusicId =  EazySoundManager.PlayMusic(dict_AudioClips[AudioTag.MAIN_THEME], music_Volume, true, false);
        
    }

    public void PlaySound(AudioTag audioTag)
    {
        if (EazySoundManager.GetAudio(dict_AudioClips[audioTag]) == null )
        {
            EazySoundManager.PlaySound(dict_AudioClips[audioTag], sound_Volume);
        }

        else if(!EazySoundManager.GetAudio(dict_AudioClips[audioTag]).IsPlaying)
        {
            EazySoundManager.PlaySound(dict_AudioClips[audioTag], sound_Volume);
        }
    }

    public void PlayUISound(AudioTag audioTag)
    {
        EazySoundManager.PlayUISound(dict_AudioClips[audioTag], sound_Volume);
    }

    void EndGameOnSound(Constants.PlayerType playerType)
    {      
      if(GameManager.instance.isEndRuleGameActivated == false)
        PlaySound(AudioTag.ENDGAME_ON);
    }

    void EndGameOffSound()    
    {
        if (GameManager.instance.isEndRuleGameActivated == true)
            PlaySound(AudioTag.ENDGAME_OFF);
    }

    void SparePawnSoundEffect()
    {        
        PlaySound(AudioTag.SPARE_PAWN);
    }

    public void OnMusicVolumeChanged(float value)
    {
        music_Volume = value;
       
        Audio audio = EazySoundManager.GetAudio(currentMusicId);
        //if (music_Volume == 0)
        //{
        //    audio.pa
        //}
        audio.SetVolume(music_Volume,0);

        if(audio.Paused)
        {
            audio.Resume();
        }
        else if(!audio.IsPlaying)
        {
            audio.Play();
        }
    }

    public void OnSoundVolumeChanged(float value)
    {
        sound_Volume = value;       
    }

    void OnSceneLoaded( Scene scene, LoadSceneMode loadSceneMode)
    {
        if(scene.buildIndex == 0)
        {
            PlayMusic();           
        }
        else if(scene.buildIndex == 1)
        {
            PlaySound(AudioTag.SPIN_BOARD);
        }
    }
}

public enum AudioTag { BUTTON_CLICK_1, BUTTON_CLICK_2, BUTTON_CLICK_3, BUTTON_CLICK_4, BUTTON_CLICK_5, MUSIC_1, SPIN_BOARD, SLIDE_PAWN, ENDGAME_ON, ENDGAME_OFF, RANK_UP, RANK_DOWN, RANK_UP_STATUS, PAWN_KNOCK, VAPORIZE, BURN, TABLE_DROP,

    PROMOTION2TOWER, WIN_GAME, LOSE_GAME, DRAW_GAME, DISCONNECTED, ONE_MINUTE_REMAINING, MAIN_THEME, BURST, FIFTY, FOURTY, THIRTY, TWENTY, TEN, THREE, TWO, ONE, ZERO, TIME_UP,

    SPARE_PAWN
}

[System.Serializable]
public struct AudioFiles
{
    public AudioClip audioClip;
    public AudioTag audioTag;
}
