using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quick_destroyer : MonoBehaviour
{
    [SerializeField]
    private float time_to_kill;
    private float timer = 0;

    void Update()
    {
        timer += Time.deltaTime;
        if ( timer >= time_to_kill)
        {
            Debug.Log("destroy");
            Destroy(gameObject);
        }
        
    }
}
