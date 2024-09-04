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
    public void toImage(int _nextId, bool useScale = true)
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
                    if(useScale) this.cg[_nextId].transform.DOScale(0.95f, this.duration).SetEase(Ease.OutBack);
                    this.cg[_nextId].interactable = true;
                    this.cg[_nextId].blocksRaycasts = true;
                }
                else
                {
                    this.cg[i].DOFade(0f, 0f);
                    //if (useScale) this.cg[_nextId].transform.DOScale(0f, 0f);
                    this.cg[i].interactable = false;
                    this.cg[i].blocksRaycasts = false;
                }
            }
        }
    }
}


public static class SetUI
{
    public static void Set(CanvasGroup _cg=null, bool _status=false, float _duration=0f, Action _onComplete = null)
    {
        if (_cg != null)
        {
            _cg.DOFade(_status? 1f : 0f, _duration).OnComplete(()=> { 
                if (_onComplete != null) 
                    _onComplete.Invoke(); 
            });
            _cg.interactable = _status;
            _cg.blocksRaycasts = _status;
        }
    }

    public static void SetGroup(CanvasGroup[] _cgs = null, int _showId=-1, float _duration = 0f)
    {
        for (int i = 0; i < _cgs.Length; i++) {
            if (_cgs[i] != null) {

                if (i == _showId)
                {
                    _cgs[_showId].DOFade(1f, _duration);
                    _cgs[_showId].interactable = true;
                    _cgs[_showId].blocksRaycasts = true;
                }
                else
                {
                    _cgs[i].DOFade(0f, _duration);
                    _cgs[i].interactable = false;
                    _cgs[i].blocksRaycasts = false;
                }
            }
        }
    }

    public static void SetMove(CanvasGroup _cg = null, bool _status = false, Vector2 _targetPos= default, float _duration = 0f, Action _onComplete = null)
    {
        if (_cg != null)
        {
            _cg.DOFade(_status ? 1f : 0f, _duration);
            _cg.transform.DOLocalMove(_targetPos, _duration).OnComplete(() => {
                if (_onComplete != null)
                    _onComplete.Invoke();
            });
            _cg.interactable = _status;
            _cg.blocksRaycasts = _status;
        }
    }

    public static Sprite ConvertTextureToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

}
