using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    public AudioClip audio;
    private void Start()
    {
        audioSource.clip = audio;
        audioSource.Play();
    }
}