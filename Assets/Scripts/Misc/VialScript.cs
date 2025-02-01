using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VialScript : MonoBehaviour
{

    public float damage;
    public GameObject bloodObject;
    float timer = 1f;
    AudioSource audioSource;
    public AudioClip glassShatter;
    public GameObject poisonCloud;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GameObject.Find("Boss").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            //play glass shatter sound
            audioSource.clip = glassShatter;
            audioSource.Play();
            //instantiate poison cloud
            Instantiate(poisonCloud, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision2D)
    {
        GameObject collidedWith = collision2D.gameObject;
        //play glass shatter sound
        audioSource.clip = glassShatter;
        audioSource.Play();
        //instantiate poison cloud
        Instantiate(poisonCloud, new Vector2(collision2D.contacts[0].point.x, collision2D.contacts[0].point.y), Quaternion.identity);
        Destroy(gameObject);
        return;
    }

}
