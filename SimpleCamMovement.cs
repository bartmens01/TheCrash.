using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCamMovement : MonoBehaviour
{
    Vector3 pos;
   public AudioSource PickUp;
    // Start is called before the first frame update
    void Start()
    {

    }

    public float speedH = 4.0f;
    public float speedV = 4.0f;
    public float speedG = 0.5f;

    public float yaw = 0.0f;
    public float pitch = 0.0f;

    void Update()
    { 
        //Camera controller
        Screen.lockCursor = true;
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        if(Input.GetKey("w"))
        {
            transform.Translate(Vector3.forward * speedG);
        }
        if (Input.GetKey("s"))
        {
            transform.Translate(Vector3.back * speedG);
        }
         pitch = Mathf.Clamp(pitch, -90f, 60f);

        // Raycast for picking up Planks
        pos = transform.position + new Vector3(0, 0, 0);

        RaycastHit hit;
        Debug.DrawRay(pos, transform.TransformDirection(Vector3.forward));
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(pos, transform.TransformDirection(Vector3.forward), out hit, 2f))
            {


                if (hit.collider.tag == "Plank")
                {
                    Debug.Log("Collected");
                    GameObject.FindGameObjectWithTag("Player").GetComponent<SimpleCharacterControl>().Plank ++;
                    Destroy(hit.collider.gameObject);
                    PickUp.Play();
                }
            }
        }
    }
}
