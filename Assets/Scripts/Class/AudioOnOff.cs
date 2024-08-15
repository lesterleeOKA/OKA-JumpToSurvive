using System;
using UnityEngine;

[Serializable]
public class AudioOnOff
{
    public CanvasGroup musicOnPanel;
    public float startPosY = 200f;
    public bool isAnimated = false;

    public void Init(bool status)
    {
        SetUI.Set(this.musicOnPanel, status, status ? 0.5f : 0f);
        Vector2 startPosition = new Vector2(0f, this.startPosY);
        SetUI.SetMove(this.musicOnPanel, status, status? Vector2.zero : startPosition, status ? 0.75f : 0f);
    }

    public void set(bool status)
    {
        AudioController.Instance?.changeBGMStatus(status);
        AudioController.Instance?.PlayAudio(0);
    }

    public void setPanel(bool status)
    {
        SetUI.Set(this.musicOnPanel, status, 0f);
    }

    public void setAnimated(bool status, Action completed = null)
    {
        if(!this.isAnimated)
        {
            this.isAnimated = true;
            Vector2 startPosition = new Vector2(0f, this.startPosY);
            SetUI.SetMove(this.musicOnPanel, status, status ? Vector2.zero : startPosition, status ? 0.75f : 0f, ()=> {
                this.isAnimated = false;
                completed?.Invoke();
            });
        }
    }
}
