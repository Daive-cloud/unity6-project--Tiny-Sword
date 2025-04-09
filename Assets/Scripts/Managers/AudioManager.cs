using UnityEngine;
using UnityEngine.InputSystem.XInput;

public class AudioManager : SingletonManager<AudioManager>
{
    [SerializeField] private AudioSource[] SFX;

    public void PlaySFX(int index)
    {
        SFX[index].pitch = Random.Range(0.9f, 1.15f);
        SFX[index].Play();
    }

}
