using TMPro;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Vector2 startPosition;
    public Vector2 endPosition;
    public RectTransform rectTransform;
    public TextMeshProUGUI word;
    public AudioSource objectEffect;
    public bool allowMove = false;

    public void setAudioEffect(bool status)
    {
        if (this.objectEffect != null)
        {
            if (status)
            {
                this.objectEffect.loop = true;
                this.objectEffect.Play();
            }
            else
            {
                this.objectEffect.Stop();
            }
        }
    }
}
