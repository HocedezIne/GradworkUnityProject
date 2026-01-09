using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CounterShake : MonoBehaviour
{
    private ScoreCounter _scoreCounter;

    [Header("Components")]
    [SerializeField] private RectTransform _shakeParent;

    [Header("Shake")]
    [SerializeField] private float _durationPerShake;
    [SerializeField] private float _shakeDistance;

    private int _shakeTweenId;
    
    private Vector3 _startPosition;

    private void Awake()
    {
        _scoreCounter = GetComponent<ScoreCounter>();
        _startPosition = _shakeParent.anchoredPosition;
    }

    private void OnEnable()
    {
        _scoreCounter.ScoreUpdated += Counter_ScoreUpdated;
    }

    private void OnDisable()
    {
        _scoreCounter.ScoreUpdated -= Counter_ScoreUpdated;
    }

    private void Counter_ScoreUpdated(object sender, EventArgs e)
    {
        if (LeanTween.isTweening(_shakeTweenId))
            LeanTween.cancel(_shakeTweenId);

        _shakeParent.anchoredPosition = _startPosition;
        
        _shakeTweenId = LeanTween.move(_shakeParent, _shakeParent.anchoredPosition + Random.insideUnitCircle * _shakeDistance, _durationPerShake)
            .setEaseShake()
            .id;
    }
}

