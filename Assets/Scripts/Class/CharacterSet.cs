using System;
using UnityEngine;

[Serializable]
public class CharacterSet
{
    public string name;
    public int playerNumber;
    public Color playerColor;
    public Texture boardTexture;
    public Texture defaultIcon, idlingTexture;
    public Texture[] animationTextures;
}
