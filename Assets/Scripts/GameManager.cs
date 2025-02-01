using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{

    public enum GameState {
        Dialog, Combat, Death, BossDeath
    }

    HashSet<int> dialogScenes = new HashSet<int>(new List<int>(new int[] {0,1,2,4,6,8,10}));

    GameState state;
    public static GameManager instance;

    void Awake() {
        instance = this;
    }

    IEnumerator WaitAndPlayScene() {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void PlayNextEvent() {
        if(dialogScenes.Contains(SceneManager.GetActiveScene().buildIndex)) {
            PlayNextSceneWithWait();
        } else {
            state = GameState.Combat;
        }
    }

    public GameState GetState() {
        return state;
    }

    public void SetDeathState() {
        if(SceneManager.GetActiveScene().buildIndex == 9) {
            SetBossDeathState();
            return;
        }
        state = GameState.Death;
        StartCoroutine(PlayDeath());
    }

    public void SetBossDeathState() {
        state = GameState.BossDeath;
        StartCoroutine(PlayBossDeath());
    }

    IEnumerator PlayBossDeath() {
        Time.timeScale = 0.2f;
        //Giant blood splatter
        yield return new WaitForSeconds(1.25f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        Time.timeScale = 1f;
    }

    IEnumerator PlayDeath() {
        Time.timeScale = 0.2f;
        //Giant blood splatter
        yield return new WaitForSeconds(1.25f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public void PlayNextSceneWithWait() {
        StartCoroutine(WaitAndPlayScene());
    }

    // Start is called before the first frame update
    void Start()
    {
        state = GameState.Dialog;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
