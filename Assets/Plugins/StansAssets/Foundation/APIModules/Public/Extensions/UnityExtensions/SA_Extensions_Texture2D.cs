using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation;

public static class SA_Extensions_Texture2D  {

    public static Texture2D Rotate(this Texture2D texture, float angle) {
        return SA.Foundation.Utility.SA_IconManager.Rotate(texture, angle);
    }


    public static Sprite ToSprite(this Texture2D texture) {
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

}
