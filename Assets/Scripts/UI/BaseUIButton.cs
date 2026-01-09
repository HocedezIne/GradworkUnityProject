using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BaseUIButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Visuals")]
    [Space(7)]
    [SerializeField] private UnityEvent _onClickedVisual;
    [SerializeField] private UnityEvent _onStartHoverVisual;
    [SerializeField] private UnityEvent _onStopHoverVisual;

    [Header("Button Settings")]
    [Range(0, 1)]
    [SerializeField] private float _executionDelay = 0;

    public bool IsInteractable { get; set; } = true;

    public event EventHandler<EventArgs> Clicked;
    public event EventHandler<EventArgs> ClickedVisual;
    public event EventHandler<EventArgs> StartHoverVisual;
    public event EventHandler<EventArgs> StopHoverVisual;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsInteractable == false)
            return;
        _onClickedVisual?.Invoke();
        OnClickedVisual();

        LeanTween.delayedCall(_executionDelay, () =>
        {
            OnClicked();
        });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsInteractable == false)
            return;
        _onStartHoverVisual?.Invoke();
        OnStartHoverVisual();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsInteractable == false)
            return;
        _onStopHoverVisual?.Invoke();
        OnStopHoverVisual();
    }

    private void OnClicked()
    {
        EventHandler<EventArgs> handler = Clicked;
        handler?.Invoke(this, EventArgs.Empty);
    }

    public void OnClickedVisual()
    {
        EventHandler<EventArgs> handler = ClickedVisual;
        handler?.Invoke(this, EventArgs.Empty);
    }

    public void OnStartHoverVisual()
    {
        EventHandler<EventArgs> handler = StartHoverVisual;
        handler?.Invoke(this, EventArgs.Empty);
    }

    public void OnStopHoverVisual()
    {
        EventHandler<EventArgs> handler = StopHoverVisual;
        handler?.Invoke(this, EventArgs.Empty);
    }
}
