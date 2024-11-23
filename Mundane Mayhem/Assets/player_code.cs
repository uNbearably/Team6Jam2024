using UnityEngine;

public class player_code : MonoBehaviour
{
    public float speed_max = 10;
    public float speed_now = 0;
    public Camera cam ;//= GameObject.FindGameObjectWithTag("MainCamera");
    public Rigidbody rigd ;//= gameObject.GetComponent<Rigidbody>();
    private float x_rot=0;
    private float y_rot=0;



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
            speed_now+=Time.deltaTime*speed_max*Input.GetAxis("Vertical")*2;
            //Apply a force to this Rigidbody in direction of this GameObjects up axis
           
        }
        else if (speed_now>0)
        {
            speed_now-=Time.deltaTime*speed_max*speed_max*3*Mathf.Sign(speed_now);
            speed_now=Mathf.Clamp(speed_now,0,100);
        }
        rigd.AddForce(gameObject.transform.forward * speed_now);
        
        y_rot+=Time.deltaTime*Input.GetAxis("Horizontal")*90;
       


        //transform.forward
     //~Rotate
        transform.rotation = Quaternion.Euler(0, y_rot, 0);   
        Cursor.lockState = CursorLockMode.Locked;

     //Camera
        cam.transform.position = gameObject.transform.position;
        cam.transform.rotation = Quaternion.Euler(x_rot, y_rot, 0);



    }
}
