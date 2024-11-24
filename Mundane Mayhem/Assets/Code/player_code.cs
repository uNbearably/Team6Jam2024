using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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

    public Vector3 go_pos;
    public Vector3 start_pos;

    public enum equip_type {none, item, gun};
    public equip_type equip_now;
    public GameObject equipment;

    public GameObject[] customers;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start_pos=transform.position;
        //rb = gameObject.GetComponent<Rigidbody>();
        //cam_target = GameObject.Find("cam_target");
        //cursor = GameObject.Find("cursor");
        //cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        stun_now -= Time.deltaTime;
        if (stun_now <= 0)
        {
            //new move

            transform.position = Vector3.Lerp(transform.position, go_pos, .5f);
            if (Input.GetButtonDown("Jump")) { go_pos = start_pos; transform.rotation = Quaternion.Euler(0,0,0); } //reset rotation
            //transform.position = 5*Vector3Int.FloorToInt(go_pos/5);
            //
            //old move

            //if (Input.GetAxis("Vertical") != 0)
            //{
            //  if (Mathf.Abs(speed_now) < speed_max)
            //{ speed_now += Time.deltaTime * speed_max * Input.GetAxis("Vertical") * 5; }   //accelerate
            //}

            //          else if (Mathf.Abs(speed_now) > 1)
            //        { speed_now -= Time.deltaTime * speed_max * 12 * Mathf.Sign(speed_now); } //decel
            //      else
            //    { speed_now = 0; }

            //   rb.AddForce(gameObject.transform.forward * speed_now);
            //    rb.linearVelocity = (gameObject.transform.forward * speed_now) + gameObject.transform.right * Input.GetAxis("Horizontal") * 2 + additional_force + grav_now * transform.up;
            //    rb.angularVelocity *= 0;
            //   additional_force -= new Vector3(Mathf.Clamp(additional_force.x, -1, 1), Mathf.Clamp(additional_force.y, -1, 1), Mathf.Clamp(additional_force.z, -1, 1)) * Time.deltaTime * 10;

            //gravity
            if (Physics.Raycast(transform.position, -transform.up, 1.2f)) { grav_now *= 0; }
            else
            { grav_now -= Time.deltaTime * 10; }

            transform.rotation = Quaternion.Euler(0, y_rot, 0);

            //Look Around
            x_rot = Mathf.Clamp(x_rot, -20, 20);

            if (Mathf.Abs(cursor_now.x) > cursor_max.x * 3 / 4)
            {
                y_rot += Time.deltaTime * 90 * (cursor_now.x / cursor_max.x); //pivot
                //y_rot += 45 * Mathf.Sign(cursor_now.x); //pivot
                if (Input.GetAxisRaw("Mouse X") == 0)
                { cursor_now.x -= Mathf.Sign(cursor_now.x) * Time.deltaTime * 1.5f; }
                //{ cursor_now.x *= 3/4; }
            }

            if (Mathf.Abs(cursor_now.y) > cursor_max.y * 5 / 4) //temporarily disabled
            {
                x_rot += Time.deltaTime * 45 * (cursor_now.y / cursor_max.y); //pivot
                if (Input.GetAxisRaw("Mouse Y") == 0)
                { cursor_now.y -= Mathf.Sign(cursor_now.y * Time.deltaTime * .75f); }
            }
        }
        else
        { rb.linearVelocity *= 0; rb.angularVelocity *= 0; }
        //Camera
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        cam.transform.position = cam_target.transform.position + transform.right * .5f * (Mathf.Sin(Mathf.Clamp(shake_now, 0, 100000)));
        cam.transform.rotation = Quaternion.Euler(x_rot, y_rot, 0);
        //cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation,Quaternion.Euler(x_rot, y_rot, 0),.1f);

        shake_now -= Mathf.PI * 20 * Time.deltaTime;  //cam shake


        //cursor
        GameObject raytarg = GameObject.Find("ray_target");
        cursor_now += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Time.deltaTime * 2;

        cursor_now.x = Mathf.Clamp(cursor_now.x, -cursor_max.x * 2, cursor_max.x * 2);
        cursor_now.y = Mathf.Clamp(cursor_now.y, -cursor_max.y, cursor_max.y);
        raypos = cam_target.transform.position + transform.right * cursor_now.x + transform.up * cursor_now.y;

        Debug.DrawRay(raytarg.transform.position, -raytarg.transform.transform.forward * 10, Color.yellow);
        raytarg.transform.position = raypos + transform.forward * 1f;
        raytarg.transform.LookAt(cam.transform.position);

        switch (equip_now)
        {
            case equip_type.item:
                equipment.transform.position = cursor.transform.position;
                equipment.GetComponent<Collider>().enabled = false;
                break;
        }
        //interactions
        RaycastHit hit;
       

        if (Physics.Raycast(raytarg.transform.transform.position, -raytarg.transform.forward, out hit, 10)&& hit.transform.tag == "interact"&&stun_now<=0)
        {
            cursor.transform.position = (hit.point*3 + hit.transform.position)/4;
            if (Input.GetButtonDown("Fire1")&&stun_now<=0)
            {
                StartCoroutine(hit.transform.gameObject.GetComponent<interaction_code>().interact());
            }
        }
        else
        {
            cursor.transform.position = raypos + transform.forward * 1f;
        //throw
            if (equip_now==equip_type.item&&Input.GetButtonDown("Fire1"))
            {
                equip_now = equip_type.none;
                equipment.GetComponent<Rigidbody>().linearVelocity = -raytarg.transform.forward*5;
                equipment.GetComponent<Rigidbody>().angularVelocity = -raytarg.transform.right*5;
                equipment.GetComponent<Rigidbody>().useGravity = true;
                equipment.GetComponent<Collider>().enabled = true;
                equipment.GetComponent<Collider>().isTrigger = false;

                equipment = null;
            }
        } //place cursor   



    }
    private void OnTriggerEnter(Collider other)
    {
        //hurt
        if (other.tag == "hurt")
        {
            StartCoroutine(hurt());
        }
    }

    public IEnumerator hurt()
    {
        Slider healthbar = GameObject.Find("healthbar").GetComponent<Slider>();
        //additional_force -= (4 * (other.transform.position - gameObject.transform.position));
        stun_now = .2f;
        shake_now = 4 * Mathf.PI;
        //health_now -= other.GetComponent<value_holder>().value;
        health_now -= 1;
        healthbar.value=health_now;
        GameObject.Find("HealthFill").GetComponent<Image>().color=new Color(1-health_now/health_max, -1 + health_now / health_max,.2f);
        if (health_now <= 0)
        {
            transform.position = start_pos;
            transform.rotation = Quaternion.Euler(0,0,0);
            speed_now *= 0;
            grav_now *= 0;
            additional_force *= 0;
            health_now = health_max;
        }
        yield return null;
    }

    public IEnumerator customerShuffle()
    {
        var t = 0;
        foreach (GameObject i in customers)
            { i.GetComponent<interaction_code>().order = t; t++; }


        yield return null;

    }

}
