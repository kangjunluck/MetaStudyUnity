using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingCloud : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector3 direction ;
    private float floatingSpeed;

    // Start is called before the first frame update

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetFloatingCloud(Sprite sprite, Vector3 direction, float floatingSpeed, float size)
    {
        this.direction = direction;
        this.floatingSpeed = floatingSpeed;

        spriteRenderer.sprite = sprite;

        transform.localScale = new Vector3(size,size,size);
        spriteRenderer.sortingOrder = (int)Mathf.Lerp(1,100, size);
    }

    private void Update() {
        transform.position += direction * floatingSpeed * Time.deltaTime;
    }
}
