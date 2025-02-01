using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatterScript : MonoBehaviour {
    public AudioClip bloodSplatterClip;
    AudioSource audioSource;
    void Start() {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = bloodSplatterClip;
        audioSource.Play();
        Destroy(gameObject, 0.41f);
    }
}