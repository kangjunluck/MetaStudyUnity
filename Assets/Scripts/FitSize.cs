using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitSize : MonoBehaviour
{
    void fitCameraWidth() {
    SpriteRenderer sr = (SpriteRenderer)GetComponent ("Renderer");
    if (sr == null)
        return;

    // Set filterMode
    sr.sprite.texture.filterMode = FilterMode.Point;

    // Get stuff
    double width = sr.sprite.bounds.size.x;
    Debug.Log ("width: " + width);
    double worldScreenHeight = Camera.main.orthographicSize * 2.0;
    double worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

    // Resize
    transform.localScale = new Vector2 (1, 1) * (float)(worldScreenWidth / width);
}
}
