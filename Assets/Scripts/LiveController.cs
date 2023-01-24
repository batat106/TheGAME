using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveController : MonoBehaviour
{
    public GameObject LivePoint;
    private List<GameObject> live = new List<GameObject>();
    public PlayerController playerController;
    
    void Start()
    {
        for (int x = 0; x < playerController.live; x++)
        {
            CreateLivePoint(x);
        }
    }

    void CreateLivePoint(int x)
    {
        GameObject live_point = Instantiate(LivePoint, transform);
        live_point.transform.position = new Vector3(transform.position.x + x * 1.5f, transform.position.y);
        live.Add(live_point);
    }
    
    void Update()
    {
        if (live.Count > playerController.live)
        {
            for (int i = live.Count - 1; i >= playerController.live; i--)
            {
                Destroy(live[i]);
                live.RemoveAt(i);
            }
        }
        else if(live.Count < playerController.live)
        {
            for (int i = live.Count; i < playerController.live; i++)
            {
                CreateLivePoint(i);
            }
        }
    }
}
