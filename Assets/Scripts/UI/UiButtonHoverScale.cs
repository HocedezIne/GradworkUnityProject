using System;
using UnityEngine;

[RequireComponent(typeof(BaseUIButton))]
public class UiButtonHoverScale : BasicScaler
{
    private BaseUIButton _button;

    protected override void Awake()
    {
        base.Awake();
        _button = GetComponent<BaseUIButton>();
        _button.StartHoverVisual += Button_HoverStarted;
        _button.StopHoverVisual += Button_HoverStopped;
    }

    private void OnDestroy()
    {
        _button.StartHoverVisual -= Button_HoverStarted;
        _button.StopHoverVisual -= Button_HoverStopped;
    }

    private void Button_HoverStarted(object sender, EventArgs e)
    {
        ScaleUp(null);
    }


    private void Button_HoverStopped(object sender, EventArgs e)
    {
        ScaleDown(null);
    }
}
