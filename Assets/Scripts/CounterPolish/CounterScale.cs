using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CounterScale : MonoBehaviour
{
    private ScoreCounter _scoreCounter;
    
    [Header("Components")]
    [SerializeField] private RectTransform _scaleParent;
    
    [Header("Scale up")]
    [SerializeField] private LeanTweenType _scaleUpType;
    [SerializeField] private float _scaleUpTime;
    [SerializeField] private float _scaleIncreaseAmount;
    [SerializeField] private float _scaleMax;
    [SerializeField, Tooltip("Determines the minimum scale a tween should slide across, gives more pop")] private float _minScaleDifference;

    [Header("Scale down")]
    [SerializeField] private LeanTweenType _scaleDownType;
    [SerializeField] private float _scaleDownTime;
    
    private Vector3 _startScale;
    private float _baseTweenScale;
    private float _currentScale = 1;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _scoreCounter = GetComponent<ScoreCounter>();
        _startScale = _scaleParent.localScale;
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
        _currentScale += _scaleIncreaseAmount;

        //Set the scale increase to max if it exceeds it
        _currentScale = Mathf.Min(_currentScale, _scaleMax);

        //Determine the base tween scale of the scale up => makes sure the scale tween is always at least a certain amount
        _baseTweenScale = (_currentScale-_minScaleDifference);
        if (_baseTweenScale < 1)
        {
            _baseTweenScale = 1;
        }


        ScaleOnce();
    }

    private void ScaleOnce()
    {
        LeanTween.cancel(_scaleParent);
        _scaleParent.localScale = _baseTweenScale*_startScale;
        LeanTween.scale(_scaleParent, Vector3.one * _currentScale, _scaleUpTime).setEase(_scaleUpType).setOnComplete(()=>
        {
            LeanTween.scale(_scaleParent, _startScale, _scaleDownTime).setEase(_scaleDownType).setOnComplete(()=>_currentScale = 1);
        });
    }
}
