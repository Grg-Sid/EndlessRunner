using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

    private bool isStart = false;
    private SwipeDetector swipeDetector;

    public UnityEngine.UI.Text coinText, scoreText, modifierText;
    private float score, coinScore, modifierScore;

    private void Awake()
    {
        Instance = this;
        swipeDetector = GameObject.FindGameObjectWithTag("Player").GetComponent<SwipeDetector>();
        UpdateScores();
    }

    private void Update()
    {
        if(SwipeDetector.isTap && !isStart)
        {
            isStart = true;
            swipeDetector.StartRunning();
        }
    }

    public void UpdateScores()
    {
        scoreText.text = score.ToString();
        coinText.text = coinScore.ToString();
        modifierText.text = modifierScore.ToString();
    }
}
