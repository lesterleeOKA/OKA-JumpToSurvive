using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[Serializable]
public class UIImage
{
    public CanvasGroup[] cg;
    public float duration = 0f;
    public int currentId = 0;
    public bool isAnimated = false;

    public void Init()
    {
        this.currentId = 0;
        isAnimated = false;
        this.toImage(this.currentId);
    }
    public void toImage(int _nextId)
    {
        if (this.isAnimated) return;
        for (int i=0; i< this.cg.Length; i++)
        {
            if (this.cg[i] != null)
            {
                if (i == _nextId)
                {
                    this.currentId = _nextId;
                    this.isAnimated = true;
                    this.cg[_nextId].DOFade(1f, this.duration).OnComplete(()=> this.isAnimated = false);
                    this.cg[_nextId].interactable = true;
                    this.cg[_nextId].blocksRaycasts = true;
                }
                else
                {
                    this.cg[i].DOFade(0f, 0f);
                    this.cg[i].interactable = false;
                    this.cg[i].blocksRaycasts = false;
                }
            }
        }
    }
}


public static class SetUI
{
    public static void Set(CanvasGroup _cg=null, bool _status=false, float _duration=0f)
    {
        if (_cg != null)
        {
            _cg.DOFade(_status? 1f : 0f, _duration);
            _cg.interactable = _status;
            _cg.blocksRaycasts = _status;
        }
    }
}