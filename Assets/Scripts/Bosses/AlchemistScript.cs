using UnityEngine.UI;
using UnityEngine;
using System;

public class AlchemistScript : MonoBehaviour, ICharacter
{

    float maxHealth = 250f;
    float health;
    bool stateComplete = false;
    Animator animator;
    public Image bossHealth;
    public Transform magicSpawn, vialSpawn;
    float moveSpeed = 3f;
    float maxDist = 6f, minDist = 2f;
    GameObject target;
    float distFromPlayer;
    Vector2 direction = Vector2.zero;
    Rigidbody2D rb;
    float timer = 0f, maxTime = 1.25f, timeToTeleport = 0.5f;
    public GameObject poisonOrb, poisonVial;
    float throwSpeed = 4.5f, magicSpeed = 7f;
    Vector2 teleportPosition = Vector2.zero;
    public AudioClip throwClip, magicClip;
    AudioSource audioSource;

    public enum BossState
    {
        Walk,
        Magic,
        Throw,
        Idle,
        Teleport,
    }

    BossState state;

    public bool IsDefending()
    {
        return false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        bossHealth.rectTransform.localScale = new Vector2(health / maxHealth, 1);
        if (health < 0) health = 0;
        if (health == 0)
        {
            GameManager.instance.SetBossDeathState();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        state = BossState.Idle;
        health = maxHealth;
        target = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.GetState() != GameManager.GameState.Combat)
        {
            return;
        }
        distFromPlayer = Math.Abs((target.transform.position - transform.position).magnitude);
        RotateTowardsPlayer();
        if (stateComplete)
        {
            SwitchState();
        }
        UpdateState();
    }

    void SwitchState()
    {
        stateComplete = false;
        switch (state)
        {
            case BossState.Idle:
                timer = maxTime;
                animator.Play("alchemist-idle");
                break;
            case BossState.Walk:
                animator.Play("alchemist-walk");
                break;
            case BossState.Magic:
                audioSource.clip = magicClip;
                audioSource.Play();
                animator.Play("alchemist-magic");
                break;
            case BossState.Throw:
                audioSource.clip = throwClip;
                audioSource.Play();
                animator.Play("alchemist-throw");
                break;
            case BossState.Teleport:
                teleportPosition = transform.position + (target.transform.position - transform.position) * 2f;
                timer = timeToTeleport;
                animator.Play("alchemist-teleport");
                break;
        }
    }

    void UpdateState()
    {
        switch (state)
        {
            case BossState.Idle:
                //start timer for attack
                //switch to walk if distance goes out of bounds
                if (distFromPlayer > maxDist || distFromPlayer < minDist)
                {
                    state = BossState.Walk;
                    stateComplete = true;
                }
                else
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        float randomNum = UnityEngine.Random.Range(0f, 1f);
                        if (randomNum > 0.5f)
                        {
                            state = BossState.Magic;
                        }
                        else
                        {
                            state = BossState.Throw;
                        }
                        stateComplete = true;
                    }
                }
                break;
            case BossState.Walk:
                if (distFromPlayer > maxDist)
                {
                    //move towards
                    direction = (target.transform.position - transform.position).normalized;

                }
                else if (distFromPlayer < minDist)
                {
                    //move away
                    direction = (transform.position - target.transform.position).normalized;
                }
                else if (distFromPlayer <= maxDist && distFromPlayer >= minDist)
                {
                    state = BossState.Idle;
                    stateComplete = true;
                }
                break;
            case BossState.Magic:
                UpdateMagic();
                break;
            case BossState.Throw:
                UpdateThrow();
                break;
            case BossState.Teleport:
                UpdateTeleport();
                break;
        }
    }

    void UpdateTeleport()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            transform.position = teleportPosition;
            state = BossState.Idle;
            stateComplete = true;
        }
    }

    void UpdateMagic()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 >= 0.98f)
        {
            GameObject poisonOrbIns = Instantiate(poisonOrb, magicSpawn.position, transform.rotation);
            poisonOrbIns.GetComponent<Rigidbody2D>().velocity = (target.transform.position - magicSpawn.position).normalized * magicSpeed;
            // audioSource.clip = throwSound;
            // audioSource.Play();
            state = BossState.Idle;
            stateComplete = true;
        }
    }

    void UpdateThrow()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 >= 0.98f)
        {
            GameObject vialIsc = Instantiate(poisonVial, vialSpawn.position, transform.rotation);
            vialIsc.GetComponent<Rigidbody2D>().velocity = (target.transform.position - magicSpawn.position).normalized * throwSpeed;
            // audioSource.clip = throwSound;
            // audioSource.Play();
            state = BossState.Idle;
            stateComplete = true;
        }
    }

    void RotateTowardsPlayer()
    {
        Vector2 direction = new Vector2(target.transform.position.x, target.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    public void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Wall")
        {
            state = BossState.Teleport;
            stateComplete = true;
        }
    }

    void FixedUpdate()
    {
        rb.drag = 0;
        if (state == BossState.Walk)
        {
            rb.velocity = direction * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

}
