using System;
using UnityEngine;

[RequireComponent(typeof(BaseUIButton))]
public class UIButtonClickScale : BasicScaler
{
    private BaseUIButton _button;

    protected override void Awake()
    {
        base.Awake();
        _button = GetComponent<BaseUIButton>();
        _button.ClickedVisual += Button_Clicked;
    }

    private void OnDestroy()
    {
        _button.ClickedVisual -= Button_Clicked;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        Scale(null);
    }
}
