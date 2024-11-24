using System;
using System.Collections;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class interaction_code : MonoBehaviour
{
    public enum act_type { dialogue, equip, hold, quest, travel, gun};
    public act_type act_now;

    public string[] my_words = new string[] { "queer", "aren't you?" };
    public string[] my_response = new string[] { "correct", "incorrect" };

    private Vector3 original_size;
    public bool freeze_player = false;
    private GameObject player;
    private bool original = true;

    public enum item_type { none, soda,ciggy};
    public item_type item_now;
    //public GameObject dialogue_object;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        original_size= transform.localScale;
        player = GameObject.Find("Player");
        //dialogue_object = GameObject.Find("Dialogue");
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = (transform.localScale * 9 + original_size) / 10;
        switch (act_now)
        {
            case act_type.travel:
                if (Vector3.Distance(GameObject.Find("Player").transform.position,transform.position)<3)
                { GetComponent<Collider>().enabled=false; GetComponent<MeshRenderer>().enabled = false; }
                else
                { GetComponent<Collider>().enabled=true; GetComponent<MeshRenderer>().enabled = true; }

                break;
        }
    }

    public IEnumerator interact()
    {
        dialogue_code codeObj = GameObject.Find("Dialogue").GetComponent<dialogue_code>();


        switch (act_now)
        {
            case act_type.equip:
                yield return new WaitForSeconds(.2f);
                GameObject dupe = Instantiate(gameObject, new Vector3(0, 0, 0), Quaternion.identity);
                dupe.GetComponent<interaction_code>().original = false;
                dupe.GetComponent<Collider>().enabled= false;
                player.GetComponent<player_code>().equipment = dupe;
                player.GetComponent<player_code>().equip_now = player_code.equip_type.item;
                if (!original) { Destroy(gameObject); }
                break;
            case act_type.dialogue:
                codeObj.global_words=my_words;
                codeObj.freeze_player = freeze_player;

                //dialogue_object.GetComponent<dialogue_code>().global_words=my_words;
                //StartCoroutine(dialogue_object.GetComponent<dialogue_code>().speak());
                StartCoroutine(codeObj.speak());
                break;
            case act_type.quest:
                if (player.GetComponent<player_code>().equipment!=null)//quest check
                {
                    if (player.GetComponent<player_code>().equipment.GetComponent<interaction_code>().item_now == item_now) //task complete
                    { 
                        transform.position += Vector3.up;
                        codeObj.global_words = new string[] { my_response[0] };
                    } 
                    else
                    { codeObj.global_words = new string[] { my_response[1] }; }
                }
                else
                    {codeObj.global_words = my_words;}
                codeObj.freeze_player = freeze_player;

                StartCoroutine(codeObj.speak());
                //check what player is holding with substring    
                break;
            case act_type.travel:
                GameObject.Find("Player").GetComponent<player_code>().go_pos = transform.position;
                break;
        }
        transform.localScale *= 1.25f;

        yield return null;

    }
}
