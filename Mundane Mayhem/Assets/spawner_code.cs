using UnityEngine;

public class spawner_code : MonoBehaviour
{
    public GameObject[] customers;
    public float spawn_timer = 5;
    public float intensity = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        spawn_timer -= Time.time;
        spawn_timer += Time.time;
        if (spawn_timer <=0) 
        { 
            spawn_timer=Random.Range(5,30 - intensity / 60);
            int customernumber = (int) Mathf.Floor(Random.Range(0, customers.Length+.9f));
            Instantiate(customers[customernumber],transform.position,Quaternion.Euler(0,0,0));
            print(customers[customernumber]);
        }
    }
}
