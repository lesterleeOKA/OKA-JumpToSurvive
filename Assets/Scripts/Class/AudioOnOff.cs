using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AudioOnOff
{
    public CanvasGroup musicOnPanel;
    public float startPosY = 200f;
    public bool isAnimated = false;

    public void Init(bool status)
    {
        SetUI.Set(this.musicOnPanel, status, status ? 0.5f : 0f);
        if (this.musicOnPanel != null) this.musicOnPanel.transform.DOLocalMoveY(status ? 0f : this.startPosY, status ? 0.75f : 0f);
    }

    public void set(bool status)
    {
        if (LoaderConfig.Instance != null)
        {
            LoaderConfig.Instance.changeBGMStatus(status);
        }

        if (AudioController.Instance != null)
            AudioController.Instance.PlayAudio(0);
    }

    public void setPanel(bool status)
    {
        SetUI.Set(this.musicOnPanel, status, 0f);
    }

    public void setAnimated(bool status, UnityAction completed = null)
    {
        if(!this.isAnimated)
        {
            this.isAnimated = true;
            this.setPanel(status);
            if (this.musicOnPanel != null)
            {
                this.musicOnPanel.transform.DOLocalMoveY(status ? 0f : this.startPosY, status ? 0.75f : 0f).OnComplete(() =>
                {
                    this.isAnimated = false;
                    completed?.Invoke();
                }
               );
            }
        }
    }
}
