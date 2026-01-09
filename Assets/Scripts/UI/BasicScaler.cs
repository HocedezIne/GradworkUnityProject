using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BasicScaler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject _scaleParent;

    [Header("Scaling Up")]
    [SerializeField] private float _scaleMultiplier;
    [SerializeField] private LeanTweenType _scaleUpType;
    [SerializeField] private float _scaleUpTime;

    [Header("Scaling Down")]
    [Tooltip("Putting this on notUsed will disable the scale down")]
    [SerializeField] private LeanTweenType _scaleDownType = LeanTweenType.notUsed;
    [SerializeField] private float _scaleDownTime;


    private Vector3 _startScale;
    
    private int _scaleUpTweenId;
    private int _scaleDownTweenId;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Awake()
    {
        _startScale = _scaleParent.transform.localScale;
    }

    
    //Public method to scale the button
    public void Scale(Action onComplete)
    {
        ScaleUp(() => ScaleDown(onComplete));
    }

    public void ScaleUp(Action onComplete)
    {
        CancelTweens();
        _scaleUpTweenId = LeanTween.scale(_scaleParent, _startScale * _scaleMultiplier, _scaleUpTime)
            .setEase(_scaleUpType)
            .setOnComplete(() =>
            {
                onComplete?.Invoke();
            }).id;
    }

    public void ScaleDown(Action onComplete)
    {
        CancelTweens();
        if (_scaleDownType != LeanTweenType.notUsed)
        {
            _scaleDownTweenId = LeanTween.scale(_scaleParent, _startScale, _scaleDownTime)
                .setEase(_scaleDownType)
                .setOnComplete(() => onComplete?.Invoke())
                .id;
        }
    }

    private void CancelTweens()
    {
        LeanTween.cancel(_scaleUpTweenId);
        LeanTween.cancel(_scaleDownTweenId);
    }
}
