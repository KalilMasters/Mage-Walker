using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Read Me//
    /*
    This script is used to manage every sound that happens in the game

    Object Pooling:
    Creating a pool of objects that can be used many times instead of 
    instantiating the same object over and over again.
    This can help the game save resources and reduces lag.

    Not using object pooling is the eqivalent of buying a plate every time you want to eat
    and then smashing it when you're done using it.
    Using object pooling is like going to the store and buying a set of plates and then
    washing them when you're done using them.

    *TLDR* Creating and destroying game objects makes lag.
    Create all that you will need at once and use them when you need them and reuse them.
    */
    #region SelfExplanitoryVariables
    //Instance for other scripts to find this one at
    public static AudioManager instance;
    public bool LoadedSoundSettings = false;

    [SerializeField] private AudioSource _musicSource; //Source that music comes from
    private AudioSource _audioSourcePrefab; //Source that music comes from
    #endregion
    #region VolumeControlVariables
    //Volume types
    public float MasterVolume; public float MusicVolume; public float SFXVolume;
    public SoundProfile CurrentMusicProfile;
    #endregion
    #region SourceRecyclingVariables
    [SerializeField] int NumOfSFXSources = 10;
    //Sources that are currently being used
    [SerializeField] private List<AudioSource> UsedSources = new List<AudioSource>();
    //Sources that are not being Used
    [SerializeField] private List<AudioSource> UnusedSources = new List<AudioSource>();
    //A list of all currently registered sound profiles
    [SerializeField] List<SoundProfile> ProfilesThatExist = new List<SoundProfile>();
    //Put in string for profile, output saved profile for that sound effect
    Dictionary<string, SoundProfile> SoundsDict = new Dictionary<string, SoundProfile>();
    Dictionary<AudioSource, SoundProfile> SFXDict = new Dictionary<AudioSource, SoundProfile>();
    Dictionary<SoundProfile, List<AudioSource>> SoundProfileAudioSources = new Dictionary<SoundProfile, List<AudioSource>>();
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
        _audioSourcePrefab = new GameObject("Source").AddComponent<AudioSource>();
        _audioSourcePrefab.playOnAwake = false;
        _audioSourcePrefab.loop = false;
        _audioSourcePrefab.spatialBlend = 0;
        _audioSourcePrefab.rolloffMode = AudioRolloffMode.Logarithmic;
        _audioSourcePrefab.gameObject.SetActive(false);
        _audioSourcePrefab.transform.parent = transform;
        CheckSourceAmount();
    }
    private void Update()
    {
        ManageAudioSources();
        CheckSourceAmount();

        _musicSource.volume = MusicVolume * (CurrentMusicProfile != null ? CurrentMusicProfile.Volume : 1);
        AudioListener.volume = MasterVolume;
        void ManageAudioSources()
        {
            Queue<SoundProfile> soundsLeftToCheck = new(SoundsDict.Values);
            while (soundsLeftToCheck.Count > 0)
            {
                SoundProfile profile = soundsLeftToCheck.Dequeue();
                Queue<AudioSource> profileSourcesLeftToCheck = new(SoundProfileAudioSources[profile]);
                int index = 0;
                while (profileSourcesLeftToCheck.Count > 0)
                {
                    AudioSource sfx = profileSourcesLeftToCheck.Dequeue();
                    if (sfx == null)
                    {
                        SoundProfileAudioSources[profile].RemoveAt(index);
                        index--;
                    }
                    else
                    {
                        sfx.volume = SFXVolume * profile.Volume;
                        //if the sfx is done playing, recycle it
                        if (!sfx.isPlaying)
                        {
                            RecycleAudioSource(sfx);
                            index--;
                        }
                    }
                    index++;
                }
            }
        }
    }
    void CheckSourceAmount()
    {
        while (UsedSources.Count + UnusedSources.Count < NumOfSFXSources)
        {
            AudioSource sfx = Instantiate(_audioSourcePrefab, transform);
            sfx.gameObject.name = "ExtraSFX" + UnusedSources.Count;
            UnusedSources.Add(sfx);
            SFXDict.Add(sfx, null);
        }
        if (_musicSource) return;
        _musicSource = Instantiate(_audioSourcePrefab, transform);
        _musicSource.transform.SetSiblingIndex(0);
        _musicSource.gameObject.name = "Music";
        _musicSource.gameObject.SetActive(true);
    }
    public SoundProfile RegisterSound(SoundProfile newSoundProfile)
    {

        if (SoundsDict.ContainsKey(newSoundProfile.SoundName)) return SoundsDict[newSoundProfile.SoundName];
        SoundsDict.Add(newSoundProfile.SoundName, newSoundProfile);
        ProfilesThatExist.Add(newSoundProfile);
        SoundProfileAudioSources.Add(newSoundProfile, new());
        newSoundProfile.playingSounds = SoundProfileAudioSources[newSoundProfile];
        return newSoundProfile;
    }
    public bool HasSoundPlaying(SoundProfile profile)
    {
        if (profile == CurrentMusicProfile) return true;
        string soundName = profile.SoundName;
        if (!SoundsDict.ContainsKey(soundName))
        {
            print("Sound not registered yet... creating new");
            return false;
        }
        return SoundProfileAudioSources[SoundsDict[soundName]].Count != 0;
    }
    public void StopSound(SoundProfile soundToStop)
    {
        RegisterSound(soundToStop);
        if (!HasSoundPlaying(soundToStop)) return;
        SoundProfile profile = SoundsDict[soundToStop.SoundName];
        while (SoundProfileAudioSources[profile].Count > 0)
        {
            RecycleAudioSource(SoundProfileAudioSources[profile][0]);
        }
    }

    public void SetAudio(AudioSettings newSettings)
    {
        LoadedSoundSettings = true;
        SetVolume(AudioType.Master, newSettings.MasterVolume);
        SetVolume(AudioType.Music, newSettings.MusicVolume);
        SetVolume(AudioType.SFX, newSettings.SFXVolume);
    }
    //Use this method to play any overarching sounds. only 1 can be played at a time
    public void PlayMusic(SoundProfile newProfile, bool loop = false)
    {
        _musicSource.Stop();
        _musicSource.clip = newProfile.Audio;
        _musicSource.loop = loop;
        CurrentMusicProfile = newProfile;
        _musicSource.Play();
    }
    public void PauseMusic(bool Pause)
    {
        if (Pause) _musicSource.Stop();
        else _musicSource.Play();
    }
    public void PlaySound(SoundProfile profile, Transform parentTransform)
    {
        profile = RegisterSound(profile);
        PlaySound(profile, parentTransform, Vector3.zero);
    }
    public void PlaySound(SoundProfile profile, Vector3 Position)
    {
        profile = RegisterSound(profile);
        PlaySound(profile, null, Position);
    }
    public void PlaySound(string clipName, Transform parent, Vector2 positon, float soundLevel = -1, bool loopSound = false, float spatialBlend = 0)
    {
        SoundProfile profile;
        if (SoundsDict.ContainsKey(clipName))
            profile = SoundsDict[clipName];
        else
        {
            AudioClip clip = Resources.Load<AudioClip>(clipName);
            if (clip == null)
                throw new System.NullReferenceException("AudioClip not found:\n" + clipName);

            profile = new SoundProfile(clipName, clip, soundLevel);

            if (soundLevel == -1)
                profile.Volume = 1;
            profile.Loop = loopSound;
            profile.SpatialBlend = spatialBlend;
            profile.Pitch = 1;

            RegisterSound(profile);
        }
        PlaySound(profile, parent, positon);
    }
    public void PlaySound(SoundProfile profile, Transform parent, Vector3 localPosition)
    {
        if (profile.Audio == null)
            throw new System.NullReferenceException("Please assign an audio clip to:\n" + profile.SoundName);

        profile = RegisterSound(profile);
        AudioSource sfx = GetFreeSource();

        sfx.clip = profile.Audio;

        sfx.transform.parent = parent;
        sfx.transform.localPosition = localPosition;

        UsedSources.Add(sfx);
        UnusedSources.Remove(sfx);
        SoundProfileAudioSources[profile].Add(sfx);
        SFXDict[sfx] = profile;

        sfx.spatialBlend = profile.SpatialBlend;

        sfx.pitch = profile.Pitch;
        sfx.loop = profile.Loop;

        sfx.Play();
    }
    private AudioSource GetFreeSource()
    {
        //print("Getting new sources");
        if (UnusedSources.Count == 0)
            RecycleAudioSource(UsedSources[UsedSources.Count - 1]);
        var sfx = UnusedSources[0];
        UnusedSources.Remove(sfx);
        sfx.gameObject.SetActive(true);
        return sfx;
    }
    private void RecycleAudioSource(AudioSource sfx)
    {
        //This method will recycle and clean any Audio Source so it can be used for the next
        //sound needed.
        if (UsedSources.Contains(sfx))
        {
            //If the source doesnt exist, get rid of it and make a new one to replace
            if (sfx == null)
            {
                print("Source is null");
                return;
            }
            var profile = SFXDict[sfx];
            SFXDict[sfx] = null;
            SoundProfileAudioSources[profile].Remove(sfx);
            //Resetting all modified values in the Audio Source
            sfx.Stop();
            sfx.pitch = 1;
            sfx.spatialBlend = 0f;

            sfx.transform.parent = transform;
            sfx.transform.position = transform.position;
            sfx.clip = null;
            sfx.rolloffMode = AudioRolloffMode.Logarithmic;
            sfx.gameObject.SetActive(false);

            UsedSources.Remove(sfx);
            UnusedSources.Add(sfx);
        }
    }
    public void SetVolume(AudioType type, float value)
    {
        switch (type)
        {
            case AudioType.Master:
                SetMasterVolume(value);
                break;
            case AudioType.Music:
                SetMusicVolume(value);
                break;
            case AudioType.SFX:
                SetSFXVolume(value);
                break;
            default: break;
        }
    }

    public void SetMasterVolume(float value)
    {
        if (MasterVolume == value) return;
        MasterVolume = value;
    }

    public void SetMusicVolume(float value)
    {
        if (MusicVolume == value) return;
        MusicVolume = value;
    }

    public void SetSFXVolume(float value)
    {
        if (SFXVolume == value) return;
        SFXVolume = value;
    }
    public enum AudioType { Master, Music, SFX }
    public struct AudioSettings
    {
        public float MasterVolume;
        public float MusicVolume;
        public float SFXVolume;
    }
}

[System.Serializable]
public class SoundProfile
{
    public string SoundName;
    [Range(0, 1)] public float Volume, SpatialBlend;
    public float Pitch;
    public AudioClip Audio;
    public bool Loop;
    public List<AudioSource> playingSounds;
    public SoundProfile(string Name, AudioClip Audio, float volume = 1)
    {
        SoundName = Name;
        Volume = volume;
        this.Audio = Audio;
    }
}
