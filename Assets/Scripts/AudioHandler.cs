using UnityEngine;
using UnityEngine.UI;

public class AudioHandler : MonoBehaviour
{
    private AudioSource Audio;
    private bool IsPlaying;

    public Sprite ButtonOn;
    public Sprite ButtonOff;

    private void Awake()
    {
            Audio = GetComponent<AudioSource>();
            DontDestroyOnLoad(transform.gameObject);
    }

    /** toggles audio button */
    public void ToggleButton()
    {
        if (IsPlaying)
        {
            PlayMusic();
            GameObject.FindGameObjectWithTag("AudioBtn").GetComponent<Image>().sprite = ButtonOn;
        }
        else
        {
            StopMusic();
            GameObject.FindGameObjectWithTag("AudioBtn").GetComponent<Image>().sprite = ButtonOff;
        }
    }

    public void PlayMusic()
    {
        IsPlaying = false;
        if (Audio.isPlaying) return;
        Audio.Play();
    }

    public void StopMusic()
    {
        Audio.Stop();
        IsPlaying = true;
    }
}
