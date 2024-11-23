using UnityEngine;
//using UnityEngine.UIElements;

public class player_code : MonoBehaviour
{
   


    public float speed_max = 10;
    public float speed_now = 0;
    public Camera cam;
    public Rigidbody rigd;
    public GameObject cam_target;
    public GameObject cursor;
    private float x_rot=0;
    private float y_rot=0;

    public float health_max = 5;
    private float health_now = 5;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
//move
        if (Input.GetAxis("Vertical")!=0&&Mathf.Abs(speed_now)<speed_max)
        {
            speed_now+=Time.deltaTime*speed_max*Input.GetAxis("Vertical")*10;
            //Apply a force to this Rigidbody in direction of this GameObjects up axis
           
        }
        else// if (speed_now>0)
        {
            speed_now-=Time.deltaTime*speed_max*speed_max*20*Mathf.Sign(speed_now);
            speed_now=Mathf.Clamp(speed_now,0,100);
        }
        rigd.AddForce(gameObject.transform.forward * speed_now);




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
    


    }
    private void OnTriggerEnter(Collider other)
    {
//hurt
        rigd.AddForce(2*(other.transform.position-gameObject.transform.position));
        health_now -= other.GetComponent<value_holder>().value;
        if (health_now<=0)
        {
            transform.position = new Vector3(0, 1, 0);
            speed_now *=0;
            health_now = health_max;
        }
    }
}
