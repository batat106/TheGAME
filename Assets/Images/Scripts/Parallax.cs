using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    // Start is called before the first frame update
    private float lenght, start_pos;
    public GameObject main_camera;
    public float parallax_distance;
    void Start()
    {
        start_pos = transform.position.x;
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float temp = main_camera.transform.position.x * (1 - parallax_distance);
        float dist = main_camera.transform.position.x * parallax_distance;
        
        transform.position = new Vector3(start_pos + dist, transform.position.y);
        
        if (temp > start_pos + lenght)
        {
            start_pos += lenght;
        } else if (temp < start_pos - lenght)
        {
            start_pos -= lenght;
        }
    }
}
