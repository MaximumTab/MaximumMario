using System;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] musicSounds, sfxSounds, conditionSounds;
    public AudioSource musicSource, sfxSource, conditionSource;

    private void Awake()
    {
      if (Instance == null){
        Instance = this;
        DontDestroyOnLoad(gameObject);
      }  
      else{
        Destroy(gameObject);
      }
    }

    private void Start()
    {
        PlayMusic("BGM");
    }
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);
        
        if (s == null)
        {
            Debug.Log("sound not found");
        }
        
        else{
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        
        if (s == null)
        {
            Debug.Log("sound not found");
        }
          else{
           sfxSource.PlayOneShot(s.clip);
        }
    }

      public void PlayGameoverSounds(string name)
{
    Sound s = Array.Find(conditionSounds, x => x.name == name);

    if (s == null)
    {
        Debug.Log("sound not found");
    }
    else
    {
        conditionSource.PlayOneShot(s.clip);
    }
}

}
