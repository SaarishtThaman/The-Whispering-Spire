using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour, ICharacter
{

    public enum PlayerState
    {
        Idle,
        Walk
    }
    public GameObject cursorObject, bulltetObject;
    public Transform spawnTransform;
    AudioSource audioSource;
    public AudioClip gunshotSound, reloadSound, deathSound;
    PlayerState state;
    Rigidbody2D rb;
    Animator animator;
    float moveSpeed = 3.5f, maxHealth = 100f, bulletSpeed = 30f, deadZone = 1f;
    float health;
    int clipSize = 5, currentClip;
    Vector2 movement;
    bool stateComplete;
    bool isReloading = false;
    float fireRate = 0.325f, nextFireTime = 0f;


    //UI
    public Image[] bulletsUI;
    public TextMeshProUGUI reloadingText;
    public TextMeshProUGUI healthText;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        healthText.text = "100";
        reloadingText.text = "";
        currentClip = clipSize;
        health = maxHealth;
        stateComplete = false;
        movement = Vector2.zero;
        state = PlayerState.Idle;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.GetState() != GameManager.GameState.Combat)
        {
            cursorObject.SetActive(false);
            return;
        }
        cursorObject.SetActive(true);
        UpdateMousePosition();
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            DoShoot();
        }
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(DoReload());
        }
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (stateComplete)
        {
            SwitchState();
        }
        UpdateState();
    }

    void DoShoot()
    {
        if (currentClip == 0 && !isReloading)
        {
            StartCoroutine(DoReload());
        }
        else if (currentClip > 0)
        {
            if ((cursorObject.transform.position - transform.position).magnitude < new Vector2(deadZone, deadZone).magnitude)
            {
                return;
            }
            bulletsUI[currentClip - 1].enabled = false;
            currentClip--;
            audioSource.clip = gunshotSound;
            audioSource.Play();
            GameObject bullet = Instantiate(bulltetObject, spawnTransform.position, transform.rotation);
            bullet.GetComponent<Rigidbody2D>().velocity = (cursorObject.transform.position - spawnTransform.position).normalized * bulletSpeed;
        }
    }

    IEnumerator DoReload()
    {
        reloadingText.text = "RELOADING...";
        isReloading = true;
        for (int i = 0; i < bulletsUI.Length; i++)
        {
            bulletsUI[i].enabled = false;
        }
        audioSource.clip = reloadSound;
        audioSource.Play();
        yield return new WaitForSeconds(1.704f);
        isReloading = false;
        for (int i = 0; i < bulletsUI.Length; i++)
        {
            bulletsUI[i].enabled = true;
        }
        currentClip = clipSize;
        reloadingText.text = "";
    }

    void SwitchState()
    {
        stateComplete = false;
        switch (state)
        {
            case PlayerState.Idle:
                animator.Play("Idle");
                break;
            case PlayerState.Walk:
                animator.Play("Walk");
                break;
        }
    }

    void UpdateState()
    {
        switch (state)
        {
            case PlayerState.Idle:
                if (movement != Vector2.zero)
                {
                    state = PlayerState.Walk;
                    stateComplete = true;
                }
                break;
            case PlayerState.Walk:
                if (movement == Vector2.zero)
                {
                    state = PlayerState.Idle;
                    stateComplete = true;
                }
                break;
        }
    }

    void UpdateMousePosition()
    {
        Cursor.visible = false;
        Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 cursorPosition = cursorObject.transform.position;
        cursorObject.transform.Translate(15f * Time.deltaTime * (targetPosition - cursorPosition), Space.World);

        Vector2 direction = cursorPosition - new Vector2(transform.position.x, transform.position.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void FixedUpdate()
    {
        rb.drag = 0;
        if(state == PlayerState.Walk) {
            rb.velocity = movement.normalized * moveSpeed;
        } else {
            rb.velocity = Vector2.zero;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0) health = 0;
        healthText.text = "" + health;
        if (health == 0)
        {
            Death();
        }
    }

    void Death()
    {
        audioSource.clip = deathSound;
        audioSource.Play();
        GameManager.instance.SetDeathState();
    }

    public bool IsDefending()
    {
        return false;
    }
}
