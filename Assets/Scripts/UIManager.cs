using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public Text scoreText;
    //public Text waveText;

    public CanvasGroup gameOverCanvasGroup;
    public float Duration = 2f;

    public Slider musicVolumeSlider;
    public Toggle soundToggle;
    public AudioSource bgmAudioSource;

    public Transform gameOverTextTransform;
    public Vector3 startScale = new Vector3(0.5f, 0.5f, 0.5f); 
    public Vector3 endScale = Vector3.one;

    public GameObject pauseMenuRoot;

    private bool isPaused = false;

    private void Start()
    {
        if (musicVolumeSlider != null && bgmAudioSource != null)
        {
            musicVolumeSlider.value = bgmAudioSource.volume;

            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (soundToggle != null)
        {
            soundToggle.isOn = AudioListener.volume > 0f ? true : false;

            soundToggle.onValueChanged.AddListener(SetSoundToggle);
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = volume;
        }
    }

    public void SetSoundToggle(bool isOn)
    {
        if (isOn)
        {
            AudioListener.volume = 1f;
        }
        else
        {
            AudioListener.volume = 0f;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuRoot.SetActive(true); 

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuRoot.SetActive(false); 

        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnEnable()
    {
        SetScoreText(0);
        //SetWaveInfo(0, 0);
        //SetActiveGameOverUi(false);
    }

    public void SetScoreText(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    //public void SetWaveInfo(int wave, int count)
    //{
    //    waveText.text = $"Wave: {wave}\nEnemy Left: {count}";
    //}

    //public void SetActiveGameOverUi(bool active)
    //{
    //    GameOverUi.SetActive(active);
    //}

    public void ShowGameOver()
    {
        StartCoroutine(FadeInGameOver());
    }

    private IEnumerator FadeInGameOver()
    {
        gameOverCanvasGroup.blocksRaycasts = true;

        gameOverCanvasGroup.alpha = 0f;
        if (gameOverTextTransform != null)
        {
            gameOverTextTransform.localScale = startScale;
        }

        float timer = 0f;

        while (timer < Duration)
        {
            timer += Time.deltaTime;
            float progress = timer / Duration; 

            gameOverCanvasGroup.alpha = Mathf.Lerp(0f, 1f, progress);

            if (gameOverTextTransform != null)
            {
                gameOverTextTransform.localScale = Vector3.Lerp(startScale, endScale, progress);
            }

            yield return null;
        }

        gameOverCanvasGroup.alpha = 1f;
        if (gameOverTextTransform != null)
        {
            gameOverTextTransform.localScale = endScale;
        }

        yield return new WaitForSeconds(Duration);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClickRestart()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);

    }
}
