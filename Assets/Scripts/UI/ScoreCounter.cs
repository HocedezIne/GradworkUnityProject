using System;
using TMPro;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    private TextMeshProUGUI _scoreText;
    private GameObject _gameObject;

    [Header("Counter Milestone")]
    [SerializeField] private int _counterMilestoneAmount;
    
    public event EventHandler<EventArgs> CounterMilestoneReached;

    public event EventHandler<EventArgs> ScoreUpdated;

    public RectTransform RectTransform { get; private set; }

    private void Awake()
    {
        _scoreText = GetComponentInChildren<TextMeshProUGUI>();
        _scoreText.text = "0";
        _gameObject = gameObject;
        RectTransform = GetComponent<RectTransform>();
    }

    public void UpdateScore(int score, float delay)
    {
        LeanTween.delayedCall(_gameObject, delay, () =>
        {
            ActualScoreUpdate(score);
        });
    }

    private void ActualScoreUpdate(int score)
    {
        _scoreText.text = score.ToString();
        OnScoreUpdated();

        if (score % _counterMilestoneAmount == 0)
        {
            OnCounterMilestoneReached();
        }
    }

    private void OnScoreUpdated()
    {
        var handler = ScoreUpdated;
        handler?.Invoke(this,EventArgs.Empty);
    }

    private void OnCounterMilestoneReached()
    {
        var handler = CounterMilestoneReached;
        handler?.Invoke(this,EventArgs.Empty);
    }
}
