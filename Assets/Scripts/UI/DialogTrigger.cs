using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public string[] names;
    public string[] sentence;
    public Sprite[] sprite;
    public AudioClip[] audioClip;

    public void TriggerDialog() {
        List<DialogObject> dialogObjects= new List<DialogObject>();
        for(int i=0;i<names.Length;i++) {
            DialogObject obj = new DialogObject();
            obj.name = names[i];
            obj.sentence = sentence[i];
            obj.sprite = sprite[i];
            obj.audioClip = audioClip[i];
            dialogObjects.Add(obj);
        }
        DialogManager.instance.StartDialog(dialogObjects);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
