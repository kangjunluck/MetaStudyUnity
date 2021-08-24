using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudFloater : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private List<Sprite> sprites;

    private float timer = 0.5f;
    private float distance = 11f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnFloatingCloud(distance);
            timer = 1f;
        }
    }
    public void SpawnFloatingCloud(float dist) 
    {
        float angle = Random.Range(0f,360f);
        Vector3 spawnPos = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0f)*distance ;
        Vector3 direction = new Vector3(Random.Range(-1f,1f), Random.Range(-1f,1f),0f);
        float floatingSpeed = Random.Range(2f,6f);
        
        var cloud = Instantiate(prefab, spawnPos, Quaternion.identity).GetComponent<FloatingCloud>();
        cloud.SetFloatingCloud(sprites[Random.Range(0,sprites.Count)], direction, floatingSpeed, Random.Range(0.5f,1f));
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var cloud = collision.GetComponent<FloatingCloud>();
        if (cloud != null)
        {
            Destroy(cloud.gameObject);
        }
    }
}


