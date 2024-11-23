using UnityEngine;
//using UnityEngine.UIElements;

public class player_code : MonoBehaviour
{
    public float speed_max = 10;
    public float speed_now = 0;
    public Camera cam;
    public Rigidbody rb;
    public GameObject cam_target;
    public GameObject cursor;
    private float x_rot=0;
    private float y_rot=0;

    public Vector3 additional_force;

    public float health_max = 5;
    private float health_now = 5;


    public Vector2 cursor_max;
    public Vector2 cursor_now;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
//move
        if (Input.GetAxisRaw("Vertical") != 0)
        { if (Mathf.Abs(speed_now) < speed_max)
            { speed_now += Time.deltaTime * speed_max * Input.GetAxis("Vertical") * 5; }   //accelerate
           
        }
        else if (Mathf.Abs(speed_now)>1)
        {
            speed_now-=Time.deltaTime*speed_max*speed_max*15*Mathf.Sign(speed_now);
            speed_now=Mathf.Clamp(speed_now,0,100);
        }
        else
        { speed_now = 0; }
     //   rb.AddForce(gameObject.transform.forward * speed_now);
        rb.linearVelocity=(gameObject.transform.forward * speed_now)+additional_force;
        additional_force -= new Vector3(Mathf.Clamp(additional_force.x, -1, 1), Mathf.Clamp(additional_force.y, -1, 1), Mathf.Clamp(additional_force.z, -1, 1)) * Time.deltaTime;




        //transform.forward
        //~Rotate
        if (Input.GetAxisRaw("Horizontal")!=0)
        {
            //y_rot += Input.GetAxis("Horizontal") * 90;
            y_rot += Time.deltaTime * Input.GetAxis("Horizontal") * 90;
            
        }

        transform.rotation = Quaternion.Euler(0, y_rot, 0);


//Camera
        Cursor.lockState = CursorLockMode.Locked;
        cam.transform.position = cam_target.transform.position;
        cam.transform.rotation = Quaternion.Euler(x_rot, y_rot, 0);
//cursor
        //RaycastHit hit;

        //Debug.DrawRay(transform.position, transform.TransformDirection(-transform.up) * GroundCheckDist, Color.yellow);
        //if (Physics.Raycast(transform.position, -transform.up, GroundCheckDist))
        //if (Physics.Raycast(transform.position, -transform.up, out hit, GroundCheckDist))
        //{ if (hit.transform.tag == "someTag") { hit.collider.gameObject.SetActive(false); } }



    }
    private void OnTriggerEnter(Collider other)
    {
//hurt
        additional_force+=(2*(other.transform.position-gameObject.transform.position));
        health_now -= other.GetComponent<value_holder>().value;
        if (health_now<=0)
        {
            transform.position = new Vector3(0, 1, 0);
            speed_now *=0;
            additional_force *= 0;
            health_now = health_max;
        }
    }
}
