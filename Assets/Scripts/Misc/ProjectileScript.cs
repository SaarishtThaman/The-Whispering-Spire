using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float damage;
    public GameObject bloodObject;
    public AudioClip defendSound;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 10f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCollisionEnter2D(Collision2D collision2D)
    {
        GameObject collidedWith = collision2D.gameObject;
        if (!collidedWith.CompareTag("Player") && !collidedWith.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            return;
        }
        if (collidedWith.TryGetComponent<ICharacter>(out var characterReference))
        {
            if (characterReference.IsDefending())
            {
                AudioSource audioSource = characterReference.GetComponent<AudioSource>();
                audioSource.clip = defendSound;
                audioSource.Play();
            }
            else
            {
                characterReference.TakeDamage(damage);
                ContactPoint2D contactPoint2D = collision2D.contacts[0];
                Instantiate(bloodObject, new Vector2(contactPoint2D.point.x, contactPoint2D.point.y), transform.rotation);
            }

        }
        if(collidedWith.GetComponent<ProjectileScript>() == null) {
            Destroy(gameObject);
        }
    }

}
