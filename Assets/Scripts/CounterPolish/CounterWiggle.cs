using System;
using System.Collections;
using UnityEngine;

public class CounterWiggle : MonoBehaviour
{
    private ScoreCounter _scoreCounter;

    [Header("Components")]
    [SerializeField]private RectTransform _wiggleParent;

    [Header("Wiggle")]
    [SerializeField] private float _durationPerWiggle;
    [SerializeField] private int _amountOfWiggles;
    [SerializeField] private float _wiggleDistance;
    [SerializeField] private float _distanceMultiplierPerWiggle = 1;
    [SerializeField] private LeanTweenType _wiggleType;

    private int _wiggleTweenId;
    private Coroutine _shakeRoutine;

    private float _currentShakeDistance;
    private int _currentAmountOfShakes;

    private void Awake()
    {
        _scoreCounter = GetComponent<ScoreCounter>();
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
        _currentShakeDistance = _wiggleDistance;
        _currentAmountOfShakes = _amountOfWiggles;

        if (_shakeRoutine == null)
            _shakeRoutine = StartCoroutine(Wiggle());
    }

    private IEnumerator Wiggle()
    {
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;

        while (_currentAmountOfShakes > 0)
        {
            _currentAmountOfShakes--;
            randomDirection = randomDirection * -1;
            bool shakeDone = false;
            WiggleOnce(randomDirection,_currentShakeDistance, () => shakeDone = true);
            _currentShakeDistance *= _distanceMultiplierPerWiggle;
            yield return new WaitUntil(() => shakeDone);
        }

        _shakeRoutine = null;
    }

    private void WiggleOnce(Vector2 direction,float currentShakeDistance, Action onComplete)
    {

        LeanTween.cancel(_wiggleTweenId);
        _wiggleTweenId = LeanTween.move(_wiggleParent, _wiggleParent.anchoredPosition + direction * currentShakeDistance, _durationPerWiggle)
            .setEase(_wiggleType)
            .setLoopPingPong(1)
            .setOnComplete(() =>
            {
                onComplete?.Invoke();
            })
            .id;
    }
}
