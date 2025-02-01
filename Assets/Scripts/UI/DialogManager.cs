using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.ComponentModel;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;
    AudioSource audioSource;
    public GameObject dialogBox;
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI nameText;
    public Image sprite;
    public float typingSpeed = 0.05f;
    private bool isTyping = false;
    public GameObject continuePrompt;
    private Queue<DialogObject> dialogs;
    float timer = 1.5f, maxTime = 1.5f;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        dialogBox.SetActive(false);
        dialogs = new Queue<DialogObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (continuePrompt.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            DisplayNextSentence();
        }
        //Skip functionality
        //TODO: Add UI
        if (Input.GetKey(KeyCode.R))
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                EndDialog();
            }
        }
        else
        {
            timer = maxTime;
        }
    }

    public void StartDialog(List<DialogObject> dialogObjects)
    {
        dialogBox.SetActive(true);
        dialogs.Clear();
        foreach (DialogObject dialogObject in dialogObjects)
        {
            dialogs.Enqueue(dialogObject);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (isTyping) return;
        continuePrompt.SetActive(false);

        if (dialogs.Count == 0)
        {
            EndDialog();
            return;
        }

        DialogObject dialog = dialogs.Dequeue();
        sprite.sprite = dialog.sprite;
        nameText.text = dialog.name;
        audioSource.clip = dialog.audioClip;
        typingSpeed = audioSource.clip.length / dialog.sentence.Length;
        audioSource.Play();
        StartCoroutine(TypeSentence(dialog));
    }

    IEnumerator TypeSentence(DialogObject dialogObject)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (char letter in dialogObject.sentence.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
        continuePrompt.SetActive(true);
    }

    public void EndDialog()
    {
        audioSource.Stop();
        dialogBox.SetActive(false);
        GameManager.instance.PlayNextEvent();
    }
}
