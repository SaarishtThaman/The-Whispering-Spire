using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SovereignScript : MonoBehaviour, ICharacter
{


    public Tilemap tilemap;
    AudioSource audioSource;
    Animator animator;
    public AudioClip spaceSound, galaxySound;
    float timer = 12f;
    Rigidbody2D rb;
    public enum BossState
    {
        Idle, Attack
    };
    public Transform spawn;
    public GameObject galaxy;
    BossState state;
    public bool IsDefending()
    {
        return true;
    }

    public void TakeDamage(float damage)
    {
        //Do nothing
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        state = BossState.Idle;
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        //tilemap.color = new Color(1, 1, 1, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.GetState() != GameManager.GameState.Combat)
        {
            return;
        }
        if (state == BossState.Idle)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                state = BossState.Attack;
                StartCoroutine(GalaxyAttack());
            }
        }
    }

    IEnumerator GalaxyAttack()
    {
        audioSource.clip = spaceSound;
        audioSource.Play();
        while (tilemap.color.a > 0)
        {
            tilemap.color = new Color(1, 1, 1, tilemap.color.a - 0.1f);
            yield return new WaitForSeconds(0.125f);
        }
        audioSource.clip = galaxySound;
        audioSource.Play();
        animator.Play("sovereign-attack");
        yield return new WaitForSeconds(0.5f);
        Instantiate(galaxy, spawn.position, Quaternion.identity);
        yield return new WaitForSeconds(0.767f);
        animator.Play("sovereign-idle");
    }

    void FixedUpdate()
    {
        rb.velocity = Vector2.zero;
    }
}
