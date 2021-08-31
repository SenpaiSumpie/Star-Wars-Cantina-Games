using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGameMusic : MonoBehaviour
{
    [SerializeField] AudioClip[] soundtrack;

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (!audioSource.playOnAwake)
        {
            audioSource.clip = soundtrack[Random.Range(0, soundtrack.Length)];
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!audioSource.isPlaying)
        {
            audioSource.clip = soundtrack[Random.Range(0, soundtrack.Length)];
            audioSource.Play();
        }
    }
}
