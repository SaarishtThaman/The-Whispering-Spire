using UnityEngine;
using UnityEngine.UI;
using System;

public class BehemothScript : MonoBehaviour, ICharacter
{
    float maxHealth = 250f;
    float health;
    bool stateComplete = false;
    Animator animator;
    public Image bossHealth;
    float maxDist = 3.5f, minDist = 6f;
    Rigidbody2D rb;
    public AudioClip biteClip, fireClip;
    float moveSpeed = 2f;
    bool playerBit = false;
    float timer = 0f, biteTime = 0.517f, fireAnimTimer = 1.1f, fireAnimTime = 1.1f, fireWaitTime = 10f, fireWaitTimer = 10f;
    public Transform fireSpawn;
    public GameObject fireObject;

    public enum BossState
    {
        Idle, BiteAttack, WalkTowards, WalkAway, FireAttack, ChargeAttack
    }
    BossState state;
    GameObject target;
    AudioSource audioSource;
    Vector2 direction = Vector2.zero;
    float distFromPlayer = 0f;

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
        RotateTowardsPlayer();
        fireWaitTimer -= Time.deltaTime;
        distFromPlayer = Math.Abs((target.transform.position - transform.position).magnitude);
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
                animator.Play("behemoth-idle");
                break;
            case BossState.WalkTowards:
                animator.Play("behemoth-walk");
                break;
            case BossState.WalkAway:
                animator.Play("behemoth-walk");
                break;
            case BossState.BiteAttack:
                playerBit = false;
                animator.Play("behemoth-bite");
                audioSource.clip = biteClip;
                audioSource.Play();
                timer = biteTime;
                break;
            case BossState.FireAttack:
                fireAnimTimer = fireAnimTime;
                animator.Play("behemoth-fire");
                audioSource.clip = fireClip;
                audioSource.Play();
                Instantiate(fireObject, fireSpawn.position, transform.rotation);
                break;
            case BossState.ChargeAttack:
                break;
        }
    }

    void UpdateState()
    {
        switch (state)
        {
            case BossState.Idle:
                if (distFromPlayer > maxDist)
                {
                    stateComplete = true;
                    state = BossState.WalkTowards;
                }
                if (fireWaitTimer <= 0)
                {
                    fireWaitTimer = fireWaitTime;
                    stateComplete = true;
                    state = BossState.FireAttack;
                }
                break;
            case BossState.WalkTowards:
                direction = (target.transform.position - transform.position).normalized;
                if (distFromPlayer <= maxDist)
                {
                    stateComplete = true;
                    state = BossState.BiteAttack;
                }
                break;
            case BossState.WalkAway:
                if (distFromPlayer >= minDist)
                {
                    stateComplete = true;
                    state = BossState.WalkTowards;
                }
                if (fireWaitTimer <= 0)
                {
                    fireWaitTimer = fireWaitTime;
                    stateComplete = true;
                    state = BossState.FireAttack;
                }
                break;
            case BossState.BiteAttack:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    stateComplete = true;
                    state = BossState.WalkAway;
                }
                break;
            case BossState.FireAttack:
                fireAnimTimer -= Time.deltaTime;
                if (fireAnimTimer <= 0)
                {
                    audioSource.Stop();
                    stateComplete = true;
                    state = BossState.Idle;
                }
                break;
            case BossState.ChargeAttack:
                break;
        }
    }

    void RotateTowardsPlayer()
    {
        if (state == BossState.FireAttack) return;
        Vector2 direction = new Vector2(target.transform.position.x, target.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name == "Player" && state == BossState.BiteAttack && !playerBit)
        {
            other.gameObject.GetComponent<ICharacter>().TakeDamage(15f);
            playerBit = true;
        }
    }

    void FixedUpdate()
    {
        rb.drag = 0;
        if (state == BossState.WalkTowards)
        {
            rb.velocity = direction * moveSpeed;
        }
        else if (state == BossState.WalkAway)
        {
            rb.velocity = -direction * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

}
