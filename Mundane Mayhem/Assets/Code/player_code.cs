using UnityEngine;
//using UnityEngine.UIElements;

public class player_code : MonoBehaviour
{
    public float speed_max = 7;
    public float speed_now = 0;
    public float grav_now = 0;


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
    private Vector3 raypos;

    public float stun_now;
    public float shake_now;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //rb = gameObject.GetComponent<Rigidbody>();
        //cam_target = GameObject.Find("cam_target");
        //cursor = GameObject.Find("cursor");
        //cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        stun_now -= Time.deltaTime;
        if (stun_now <= 0)
        {
//move

            if (Input.GetAxis("Vertical") != 0)
            {
                if (Mathf.Abs(speed_now) < speed_max)
                { speed_now += Time.deltaTime * speed_max * Input.GetAxis("Vertical") * 5; }   //accelerate
            }

            else if (Mathf.Abs(speed_now) > 1)
            { speed_now -= Time.deltaTime * speed_max * 12 * Mathf.Sign(speed_now); } //decel
            else
            { speed_now = 0; }

            //   rb.AddForce(gameObject.transform.forward * speed_now);
            rb.linearVelocity = (gameObject.transform.forward * speed_now) + gameObject.transform.right * Input.GetAxis("Horizontal") * 2 + additional_force + grav_now * transform.up;
            rb.angularVelocity *= 0;
            additional_force -= new Vector3(Mathf.Clamp(additional_force.x, -1, 1), Mathf.Clamp(additional_force.y, -1, 1), Mathf.Clamp(additional_force.z, -1, 1)) * Time.deltaTime * 10;

//gravity
            if (Physics.Raycast(transform.position, -transform.up, 1.2f)) { grav_now *= 0; }
            else
            { grav_now -= Time.deltaTime * 10; }

            transform.rotation = Quaternion.Euler(0, y_rot, 0);

            if (Mathf.Abs(cursor_now.x) > cursor_max.x * 2 / 3)
            {
                //cursor_now += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Time.deltaTime * 5; //double rotation at edges
                y_rot += Time.deltaTime * 90 * (cursor_now.x / cursor_max.x); //pivot
                                                                              //cursor_now.x -= Mathf.Sign(cursor_now.x) * Time.deltaTime * 2;
            }
        }
//Camera
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        cam.transform.position = cam_target.transform.position+transform.right*(Mathf.Sin(Mathf.Clamp(shake_now,0,1000000)));
        cam.transform.rotation = Quaternion.Euler(x_rot, y_rot, 0);
        shake_now -= Mathf.PI*20*Time.deltaTime;  //cam shake
        
        
//cursor
        cursor_now += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"))*Time.deltaTime*5;
        
        cursor_now.x = Mathf.Clamp(cursor_now.x, -cursor_max.x, cursor_max.x);
        cursor_now.y = Mathf.Clamp(cursor_now.y, -cursor_max.y, cursor_max.y);
        raypos = cam_target.transform.position + transform.right * cursor_now.x + transform.up * cursor_now.y;

        Debug.DrawRay(raypos, transform.TransformDirection(-cursor.transform.forward) * 10, Color.yellow);
    //interactions
        RaycastHit hit;
        if (Physics.Raycast(raypos, -cursor.transform.forward, out hit, 10)&& hit.transform.tag == "interact")
        { 
                cursor.transform.position = (hit.point+ hit.transform.position)/2;
                if (Input.GetButtonDown("Fire1")&&stun_now<=0)
                {
                   hit.transform.gameObject.GetComponent<interaction>().interact();
                }
        }
        else
        { 
            cursor.transform.position = raypos + transform.forward * 1f; 
            cursor.transform.LookAt(cam.transform.position);

        } //place cursor   





    }
    private void OnTriggerEnter(Collider other)
    {
//hurt
        additional_force-=(4*(other.transform.position-gameObject.transform.position));
        stun_now = .2f;
        shake_now = 4*Mathf.PI;
        health_now -= other.GetComponent<value_holder>().value;
        if (health_now<=0)
        {
            transform.position = new Vector3(0, 1, 0);
            speed_now *=0;
            grav_now *= 0;
            additional_force *= 0;
            health_now = health_max;
        }
    }
}
