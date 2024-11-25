using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading;
//using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using NUnit.Framework;
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
    private float y_rot=90;

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

    public List<GameObject> customers;
    public List<GameObject> jump_points;
    public GameObject quest_item;


    public GameObject win_screen;
    public GameObject lose_screen;
    public GameObject pause_screen;
    private bool paused = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        go_pos=transform.position;
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
            if (Input.GetButtonDown("Pause")) { paused = !paused; pause_screen.SetActive(paused); }
            //new move

            transform.position = Vector3.Lerp(transform.position, go_pos, .5f);
            if (Input.GetKeyDown(KeyCode.Keypad1)|| Input.GetKeyDown(KeyCode.Alpha1)) { go_pos = jump_points[1].transform.position; }
            if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) { go_pos = jump_points[2].transform.position; }
            if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) { go_pos = jump_points[3].transform.position; }
            if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4)) { go_pos = jump_points[4].transform.position; }

            if (Input.GetButtonDown("Jump")|| Input.GetKeyDown(KeyCode.Keypad0)|| Input.GetKeyDown(KeyCode.Alpha0)) { go_pos = jump_points[0].transform.position;  y_rot*=0; } //reset rotation
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
            y_rot += Time.deltaTime * 90 * Input.GetAxis("Horizontal"); //pivot alt

            //X rot
            if (Mathf.Abs(cursor_now.x) > cursor_max.x * 3 / 4)
            {
                y_rot += Time.deltaTime * 90 * (cursor_now.x / cursor_max.x); //pivot
                if (Input.GetAxisRaw("Mouse X") == 0&& Mathf.Abs(cursor_now.x) > cursor_max.x)
                { cursor_now.x -= Mathf.Sign(cursor_now.x) * Time.deltaTime * 1.5f; }
            }
            //yRot
            if (Mathf.Abs(cursor_now.y) > cursor_max.y * 100 / 4&& Mathf.Abs(cursor_now.y) > cursor_max.y * 4 / 4) //disabled
            {
                x_rot -= Time.deltaTime * 90 * (cursor_now.y / cursor_max.y); //pivot
                if (Input.GetAxisRaw("Mouse Y") == 0&& Mathf.Abs(cursor_now.y) > cursor_max.y * 4 / 4)
                { 
                    cursor_now.y = Vector2.Lerp(cursor_now,new Vector2(0,0), .02f).y;
                    if (cursor_now.y> cursor_max.y * 4 / 5)
                    { cursor_now.y = Vector2.Lerp(new Vector2(0, x_rot), new Vector2(0, 0), .0f).y; }
                    
                }
            }
        }
        else
        { rb.linearVelocity *= 0; rb.angularVelocity *= 0; }


        //Camera
        if (paused) { Cursor.lockState = CursorLockMode.Locked;}
        else { Cursor.lockState = CursorLockMode.None;}
        Cursor.visible = false;
        cam.transform.position = cam_target.transform.position + transform.right * .5f * (Mathf.Sin(Mathf.Clamp(shake_now, 0, 100000)));
        cam.transform.rotation = Quaternion.Euler(x_rot, y_rot, 0);
        //cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation,Quaternion.Euler(x_rot, y_rot, 0),.1f);

        shake_now -= Mathf.PI * 20 * Time.deltaTime;  //cam shake


        //QUEST ITEM 
        if (quest_item != null)
        //{ quest_item.transform.position = GameObject.Find("to_do").transform.position; }
        { quest_item.transform.rotation *= Quaternion.Euler(0, 90 * Time.deltaTime, 0); }

        //cursor
        GameObject raytarg = GameObject.Find("ray_target");
        cursor_now += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Time.deltaTime * 3;

        cursor_now.x = Mathf.Clamp(cursor_now.x, -cursor_max.x * 2, cursor_max.x * 2);
        cursor_now.y = Mathf.Clamp(cursor_now.y, -cursor_max.y*1, cursor_max.y*1);
        raypos = cam_target.transform.position + transform.right * cursor_now.x + transform.up * cursor_now.y;

        Debug.DrawRay(raytarg.transform.position, -raytarg.transform.transform.forward * 10, Color.yellow);
        raytarg.transform.position = raypos + transform.forward * 1f;
        raytarg.transform.LookAt(cam.transform.position);

        switch (equip_now)
        {
            case equip_type.item:
                equipment.transform.position = cursor.transform.position;
                break;
        }
        //interactions
        RaycastHit hit;
       

        if (Physics.Raycast(raytarg.transform.transform.position, -raytarg.transform.forward, out hit, 10)&& hit.transform.tag == "interact"&&stun_now<=0)
        {
            cursor.transform.position = (hit.point + hit.transform.position*3)/4;
            cursor.transform.localScale=Vector3.one/6;
            cursor.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.yellow);
            if (Input.GetButtonDown("Fire1")&&stun_now<=0)
            {
                StartCoroutine(hit.transform.gameObject.GetComponent<interaction_code>().interact());
            }
        }
        else
        {
            cursor.transform.localScale = Vector3.one/40;
            cursor.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white);

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

    void FixedUpdate()
    {
        
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
        GameObject.Find("HealthFill").GetComponent<Image>().color=new Color(1-health_now/health_max, health_now / health_max,.2f);
//death
        if (health_now <= 0)
        {
            x_rot=90;// = Quaternion.Euler(0, 90, 0);
            lose_screen.SetActive(true);
            yield return new WaitForSeconds(1);
            while (!Input.GetButton("Fire1")) { yield return new WaitForEndOfFrame(); stun_now = .1f; }


            GameObject.Find("spawner").GetComponent<spawner_code>().spawn_timer = 5;
            GameObject.Find("spawner").GetComponent<spawner_code>().intensity = 0;
            transform.position = start_pos;
            y_rot = 0;
            x_rot = 0;
            speed_now *= 0;
            grav_now *= 0;
            additional_force *= 0;
            health_now = health_max;
            //reset healthbar
            healthbar.value = health_now;
            GameObject.Find("HealthFill").GetComponent<Image>().color = new Color(1 - health_now / health_max, -1 + health_now / health_max, .2f);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }
        yield return null;
    }

    public IEnumerator customerShuffle()
    {
        //if (customers.Count>8) { customers= customers.OrderBy(x => Random.value).ToList(); }
        var t = 0;
        foreach (GameObject i in customers)
            {

            if (t > 8) { Destroy(i); }  
            i.GetComponent<interaction_code>().order = t; 
            t++; 
            
            }


        //win
        if (customers.Count <= 0) 
        
        {
            win_screen.SetActive(true);
            yield return new WaitForSeconds(1);

            while (!Input.GetButton("Fire1")) { yield return new WaitForEndOfFrame(); stun_now = .1f; }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        yield return null;

    }

}
