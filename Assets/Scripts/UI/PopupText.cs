using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupText : MonoBehaviour
{
    public TextMeshProUGUI popup;
    public String popupText;
    public float writeSpeed;
    public float timeToDisplay;
    public AudioClip typeSound;
    AudioSource audioSource;
    public DialogTrigger dialogTrigger;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(TypeText());
    }
    IEnumerator TypeText() {
        audioSource.clip = typeSound;
        audioSource.Play();
        popup.text = "";
         foreach (char letter in popupText)
        {
            popup.text += letter;
            yield return new WaitForSeconds(writeSpeed);
        }
        audioSource.Stop();
        yield return new WaitForSeconds(timeToDisplay);
        popup.gameObject.SetActive(false);
        if(dialogTrigger != null) {
            dialogTrigger.TriggerDialog();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
