using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LingeringDamageScript : MonoBehaviour
{
    public float maxTime = 0.3f, timer = 0.3f, destroyAfter = 6f, damage = 3f;
    public bool followPlayer = false;
    GameObject target;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.Find("Player");
        Destroy(gameObject, destroyAfter);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate() {
        if(followPlayer) {
            rb.velocity = (target.transform.position - transform.position).normalized * 0.5f;
        }
    }

    public void OnTriggerStay2D(Collider2D coll) {
        if(coll.gameObject.name == "Player") {
            timer -= Time.deltaTime;
            if(timer <= 0) {
                coll.gameObject.GetComponent<ICharacter>().TakeDamage(damage);
                timer = maxTime;
            }
        }
    }

    public void OnTriggerExit2D(Collider2D coll) {
        if(coll.gameObject.name == "Player") {
            timer = maxTime;
        }
    }

}
