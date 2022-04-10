using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Assertions;

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas = null;
    [SerializeField] private Slider musicVolumeSlider = null;
    [SerializeField] private Toggle musicMutedToggle = null;
    [SerializeField] private AudioSource music = null;

    public static Settings instance;

    private void Awake()
    {
        Assert.IsNotNull(mainMenuCanvas);
        //Assert.IsNotNull(musicVolumeSlider);
        //Assert.IsNotNull(musicMutedToggle);
        //Assert.IsNotNull(music);
    }

    void Start()
    {
        instance = this;
        SetSavedMusicSettings();
    }

    protected void SetSavedMusicSettings()
    {
        if (musicMutedToggle)
        {
            musicMutedToggle.isOn = GlobalControl.Instance.savedValues.MusicMuted;
        }
        if (musicVolumeSlider)
        {
            musicVolumeSlider.value = GlobalControl.Instance.savedValues.MusicVolume;
        }
    }

    public void ToggleSettings()
    {
        if (mainMenuCanvas.activeSelf)
        {
            mainMenuCanvas.SetActive(false);
            Player.PlayerInstance.GivePlayerControl(true);
            GameTime.UnpauseGame();
        }
        else
        {
            mainMenuCanvas.SetActive(true);
            Player.PlayerInstance.GivePlayerControl(false);
            GameTime.PauseGame();
        }
    }

    public void DeactivateButton(GameObject button)
    {
        button.SetActive(false);
    }

    public void ActivateButton(GameObject button)
    {
        button.SetActive(true);
    }

    public void AdjustMusicVolume()
    {
        music.volume = musicVolumeSlider.value;
        GlobalControl.Instance.savedValues.MusicVolume = music.volume;
    }

    public void MuteMusic()
    {
        music.mute = !music.mute;
        GlobalControl.Instance.savedValues.MusicMuted = music.mute;
    }
}