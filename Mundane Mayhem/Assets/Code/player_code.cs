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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
//move
        if (Input.GetAxis("Vertical") != 0)
        { 
            if (Mathf.Abs(speed_now) < speed_max)
            { speed_now += Time.deltaTime * speed_max * Input.GetAxis("Vertical") * 5; }   //accelerate
        }

        else if (Mathf.Abs(speed_now)>1)
        {speed_now-=Time.deltaTime*speed_max*12*Mathf.Sign(speed_now);} //decel
        else
        { speed_now = 0; }

        //   rb.AddForce(gameObject.transform.forward * speed_now);
        rb.linearVelocity = (gameObject.transform.forward * speed_now) + additional_force +grav_now*transform.up;
        rb.angularVelocity *= 0;
        additional_force -= new Vector3(Mathf.Clamp(additional_force.x, -1, 1), Mathf.Clamp(additional_force.y, -1, 1), Mathf.Clamp(additional_force.z, -1, 1)) * Time.deltaTime*10;

//gravity
        if (Physics.Raycast(transform.position, -transform.up, 1.2f)) {grav_now *= 0; }
        else
        {grav_now -= Time.deltaTime * 10;}



        //transform.forward
        //~Rotate
        if (Input.GetAxisRaw("Horizontal")!=0)
        {
            //y_rot += Input.GetAxis("Horizontal") * 90;
            y_rot += Time.deltaTime * Input.GetAxis("Horizontal") * 90;
            
        }

        transform.rotation = Quaternion.Euler(0, y_rot, 0);


//Camera
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        cam.transform.position = cam_target.transform.position;
        cam.transform.rotation = Quaternion.Euler(x_rot, y_rot, 0);
//cursor
        cursor_now += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"))*Time.deltaTime*5;
        cursor_now.x = Mathf.Clamp(cursor_now.x, -cursor_max.x, cursor_max.x);
        cursor_now.y = Mathf.Clamp(cursor_now.y, -cursor_max.y, cursor_max.y);
        raypos = cam_target.transform.position + transform.right * cursor_now.x + transform.up * cursor_now.y;

        Debug.DrawRay(raypos, transform.TransformDirection(-cursor.transform.forward) * 10, Color.yellow);
        RaycastHit hit;
        if (Physics.Raycast(raypos, -cursor.transform.forward, out hit, 10)&& hit.transform.tag == "interact")
        { 
                //hit.collider.gameObject.SetActive(false);
                cursor.transform.position = hit.point;
                //cursor.transform.position = hit.transform.position;
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
