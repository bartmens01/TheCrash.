using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class SimpleCharacterControl : MonoBehaviour {

    private enum ControlMode
    {
        Tank,
        Direct
    }

    [SerializeField] private float m_moveSpeed = 4;
    [SerializeField] private float m_turnSpeed = 200;
    [SerializeField] private float m_jumpForce = 4;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody m_rigidBody;

    [SerializeField] private ControlMode m_controlMode = ControlMode.Direct;

    private float m_currentV = 0;
    private float m_currentH = 0;
    public float Stamina;
    public bool Isrunning;
    public bool CanRun;
    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 0.33f;
    private readonly float m_backwardsWalkScale = 0.16f;
    private readonly float m_backwardRunScale = 0.66f;
    public Transform Spawn1;
    public GameObject Spider;
    public int Plank;
    private bool m_wasGrounded;
    private Vector3 m_currentDirection = Vector3.zero;
    public AudioSource Scare;
    public AudioSource Scare2;
    public AudioSource Tired;
    public AudioSource Bush;
    public AudioSource PickUp;
    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;
    Vector3 pos;
    private bool m_isGrounded;
    private List<Collider> m_collisions = new List<Collider>();

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for(int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider)) {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if(validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        } else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }

	void Update () {
        m_animator.SetBool("Grounded", m_isGrounded);

        switch(m_controlMode)
        {
            case ControlMode.Direct:
                DirectUpdate();
                break;

            case ControlMode.Tank:
                TankUpdate();
                break;

            default:
                Debug.LogError("Unsupported state");
                break;
        }

        m_wasGrounded = m_isGrounded;
    }

    private void TankUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        bool walk = Input.GetKey(KeyCode.LeftShift);

        if (v < 0) {
            if (walk) { v *= m_backwardsWalkScale; }
            else { v *= m_backwardRunScale; }
        } else if(walk)
        {
            v *= m_walkScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        transform.position += transform.forward * m_currentV * m_moveSpeed * Time.deltaTime;
        transform.Rotate(0, m_currentH * m_turnSpeed * Time.deltaTime, 0);

        m_animator.SetFloat("MoveSpeed", m_currentV);

        JumpingAndLanding();
    }

    private void DirectUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        v *= m_walkScale;
        h *= m_walkScale;
        Transform camera = Camera.main.transform;






        // Stamina system
        


            if (Stamina > 0 && CanRun == true)
            {

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    Isrunning = true;
                    v /= m_walkScale;
                    h /= m_walkScale;
                }
                else
                {
                    Isrunning = false;
                }
            }
            if (Stamina >= 250)
            {
                CanRun = true;
            }
            if (Stamina <= 0)
            {
                Stamina = 0;
                Isrunning = false;
                CanRun = false;
            }
            if (Isrunning == true)
            {
                Stamina--;
            }
            if (Isrunning == false)
            {
                Stamina += 1;
            }
            if (Stamina >= 1000)
            {
                Stamina = 1000;
            }
            if (Stamina == 400)
            {
                Tired.Play();
            }
            if (Stamina > 400)
            {
                Tired.Stop();
            }
        
        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        Vector3 direction = camera.forward * m_currentV + camera.right * m_currentH;

        float directionLength = direction.magnitude;
        direction.y = 0;
        direction = direction.normalized * directionLength;

        if(direction != Vector3.zero)
        {
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);

            transform.rotation = Quaternion.LookRotation(m_currentDirection);
             transform.position += m_currentDirection *  m_moveSpeed * Time.deltaTime;
           
            m_animator.SetFloat("MoveSpeed", direction.magnitude);
        }

        JumpingAndLanding();
    }

    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && m_isGrounded && Input.GetKey(KeyCode.Space))
        {
            m_jumpTimeStamp = Time.time;
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
        }

        if (!m_wasGrounded && m_isGrounded)
        {
            m_animator.SetTrigger("Land");
        }

        if (!m_isGrounded && m_wasGrounded)
        {
            m_animator.SetTrigger("Jump");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // JumpScares/Spiderspawns
        if (other.gameObject.tag == "JumpScare1")
        {
            Debug.Log("ey");
            //Lets the spider move: SpiderScare script
            GameObject.FindGameObjectWithTag("JumpScareSpider").GetComponent<SpiderScare>().Go = true;
            Scare.Play();
        }
        if (other.gameObject.tag == "JumpScare2")
        {
            Debug.Log("ey");
            GameObject.FindGameObjectWithTag("JumpScareSpider2").GetComponent<SpiderScare>().Go = true;
            Scare2.Play();
        }
        if (other.gameObject.tag == "TriggerSpider1")
        {
            Instantiate(Spider, new Vector3(80.53f, 3.86f, 42.9f), Quaternion.identity);
            Debug.Log("KA");
            Destroy(GameObject.FindGameObjectWithTag("TriggerSpider1"));
        }
        if (other.gameObject.tag == "TriggerSpiderD1")
        {
            Destroy(GameObject.FindGameObjectWithTag("Spider"));
            Debug.Log("Del");
        }
        if (other.gameObject.tag == "Death")
        {
            SceneManager.LoadScene("Deathscreen");
            Debug.Log("Dead");
        }
        if (other.gameObject.tag == "Spider")
        {
            SceneManager.LoadScene("Deathscreen");
            Debug.Log("Dead");
        }
        if (other.gameObject.tag == "TriggerSpider2")
        {
            
            
                Instantiate(Spider, new Vector3(46.26f, 4, 95.61f), Quaternion.identity);
                Debug.Log("KA");
            

        }
        if (other.gameObject.tag == "TriggerSpider3")
        {


            Instantiate(Spider, new Vector3(89.62f, 4, 119.31f), Quaternion.identity); 
            Debug.Log("KA");


        }
        if (other.gameObject.tag == "EndTrigger")
        {

            if (Plank == 3)
            {
                SceneManager.LoadScene("Win");
                Debug.Log("Win");
            }

        }
        if (other.gameObject.tag == "Bush")
        {

            Bush.Play();
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Bush")
        {

            Instantiate(Spider, new Vector3(25.27f, 4, 82.97f), Quaternion.identity);
            Destroy(GameObject.FindGameObjectWithTag("Bush"));
        }
    }
    
       
}
