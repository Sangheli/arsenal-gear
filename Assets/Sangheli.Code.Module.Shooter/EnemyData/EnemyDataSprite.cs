using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyDataSprite
{
    public Sprite spriteView;
    public Sprite spriteBorder;
    public List<Sprite> multiSprite;
    public bool useMultiSprite;
    
    public Sprite Get() => useMultiSprite ? multiSprite.PickRandomSingle() : spriteView;
}