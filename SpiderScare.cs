using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderScare : MonoBehaviour
{
    public int speed;
    public bool Go;
    public int timer = 100;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Go == true)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
            
        }
        if (timer == 0)
        {
            Destroy(gameObject);
        }
    }
    private void FixedUpdate()
    {
        //Sets the timer to destroy the spider.
        if (Go == true)
        {
            timer--;
        }
    }
}
