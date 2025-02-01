using UnityEngine;
using UnityEngine.UI;

public class BelisariusScript : MonoBehaviour, ICharacter
{
    float maxHealth = 150f;
    float health;
    public GameObject target, spear;
    float throwSpeed = 15f;
    public Transform spawnTransform;
    Animator animator;
    bool stateCompelete = false;
    public AudioClip throwSound;
    AudioSource audioSource;
    public Image bossHealth;

    public enum BossState
    {
        ShieldUp,
        ShieldDown,
        Throw
    }

    BossState state;

    float timer;
    float shieldUpTimer = 4f;
    float shieldDownTimer = 1f;
    float timerToUse;

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
        health = maxHealth;
        timerToUse = shieldDownTimer;
        timer = timerToUse;
        state = BossState.ShieldDown;
        target = GameObject.Find("Player");
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
        if (state != BossState.Throw)
        {
            timer -= Time.deltaTime;
        }
        if (timer < 0)
        {
            //transitions:
            //shield up -> shield down
            //shield down -> shield up or throw
            if (state == BossState.ShieldUp)
            {
                state = BossState.ShieldDown;
                stateCompelete = true;
            }
            else if (state == BossState.ShieldDown)
            {
                float randomNum = Random.Range(0f, 1f);
                if (randomNum >= 0.6f)
                {
                    state = BossState.ShieldUp;
                }
                else
                {
                    state = BossState.Throw;
                }
                stateCompelete = true;
            }
        }
        RotateTowardsPlayer();
        if (stateCompelete)
        {
            SwitchState();
        }
        UpdateState();
    }

    void SwitchState()
    {
        stateCompelete = false;
        switch (state)
        {
            case BossState.ShieldUp:
                animator.Play("shield-up-idle");
                timerToUse = shieldUpTimer;
                timer = timerToUse;
                break;
            case BossState.ShieldDown:
                animator.Play("shield-down-idle");
                timerToUse = shieldDownTimer;
                timer = timerToUse;
                break;
            case BossState.Throw:
                animator.Play("throw-attack");
                break;
        }
    }

    void UpdateState()
    {
        switch (state)
        {
            case BossState.ShieldUp:
                break;
            case BossState.ShieldDown:
                break;
            case BossState.Throw:
                UpdateThrow();
                break;
        }
    }

    void UpdateThrow()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 >= 0.91f)
        {
            GameObject spearIns = Instantiate(spear, spawnTransform.position, transform.rotation);
            spearIns.GetComponent<Rigidbody2D>().velocity = (target.transform.position - spawnTransform.position).normalized * throwSpeed;
            audioSource.clip = throwSound;
            audioSource.Play();
            state = BossState.ShieldUp;
            stateCompelete = true;
        }
    }

    void RotateTowardsPlayer()
    {
        Vector2 direction = new Vector2(target.transform.position.x, target.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    public bool IsDefending()
    {
        return state == BossState.ShieldUp;
    }
}
