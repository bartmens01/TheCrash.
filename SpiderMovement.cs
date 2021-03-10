using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderMovement : MonoBehaviour
{
    private Animator anim;
    public Transform Player;
    public int MoveSpeed = 4;
    public int MaxDist = 10;
    public int MinDist = 5;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        anim.SetBool("isWalking", true);
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Player);

        if (Vector3.Distance(transform.position, Player.position) >= MinDist)
        {

            transform.position += transform.forward * MoveSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, Player.position) <= MaxDist)
            {
                anim.SetBool("isWalking", false);
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("idle"))
                    anim.SetBool("idle", true);
            }

        }
    }
}
