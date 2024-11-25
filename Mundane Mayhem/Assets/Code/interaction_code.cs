using System;
using System.Collections;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class interaction_code : MonoBehaviour
{
    [Header("NPC Variables")]

    public GameObject quest_item;
    public AudioClip voice;
    public string[] my_words = new string[] { "you look so incredibly bored.", "aren't you?" };
    public string[] my_response = new string[] { "correct", "incorrect" };

    

    public enum act_type { dialogue, equip, hold, quest, travel, gun, display };
    [Header("Other Properties")]
    public act_type act_now;
    private Vector3 start_pos;
    private Vector3 original_size;
    public bool freeze_player = false;
    private GameObject player;
    private bool original = true;
    



    public enum item_type { none, soda,ciggy, soda_wonster, soda_notpee,chip_chipchip,chip_blue, candy, slushee, soda_ahhhh,soda_beer,candy_red,candy_green,coffee,burger,necronomicon,idol_of_the_old_god,pandoras_box};
    public item_type item_now;
    public int order;
    private float charlie = 10f;
    

    



    //public GameObject dialogue_object;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        original_size = transform.localScale;
        player = GameObject.Find("Player");
        start_pos= transform.position;

        switch (act_now)
        {
            case act_type.quest:
                item_now = quest_item.GetComponent<interaction_code>().item_now;
                transform.GetChild(1).gameObject.transform.position -= Vector3.up*2;
                transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = (true);   //get normal
                transform.GetChild(1).gameObject.GetComponent<Renderer>().enabled = (false); //hide mad

                player.GetComponent<player_code>().customers.Add(gameObject);
                StartCoroutine(player.GetComponent<player_code>().customerShuffle());
                break;
        }
        //dialogue_object = GameObject.Find("Dialogue");
    }

    // Update is called once per frame
    void Update()
    {
        if (charlie>1) { transform.localScale = (transform.localScale * 9 + original_size) / 10; }
        switch (act_now)
        {
            case act_type.equip:
                if (!original&& gameObject.GetComponent<Collider>().enabled) { charlie -= Time.deltaTime; } 
                if (charlie<1) { transform.localScale=Vector3.Lerp(new Vector3(0, 0, 0),original_size,charlie); }
                if (charlie<=0) { Destroy(gameObject); }
                break;
            case act_type.quest:
                if (order >= 0)
                { transform.position = Vector3.Lerp(transform.position, new Vector3(order * 2, 1, 1.5f), .05f); }
                else { transform.position = Vector3.Lerp(transform.position, start_pos, .1f); }
                break;

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
                    freeze_player = false;


                    if (player.GetComponent<player_code>().equipment.GetComponent<interaction_code>().item_now == item_now) //task complete
                    {
                        
                        //satisfy
                        codeObj.global_words = new string[] { my_response[0] };
                        player.GetComponent<player_code>().customers.Remove(gameObject);
                        StartCoroutine(player.GetComponent<player_code>().customerShuffle()); //shuffle players
                        order = -1000;
                    } 
                    else
                    {
                        codeObj.global_words = new string[] { my_response[1] }; //fail task
                        StartCoroutine(player.GetComponent<player_code>().hurt());
                        transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = (false); //hide normal expression
                        transform.GetChild(1).gameObject.GetComponent<Renderer>().enabled = (true);  //get mad expression
                    }

                    //clear hand
                    Destroy(player.GetComponent<player_code>().equipment);
                    player.GetComponent<player_code>().equipment = null;
                    player.GetComponent<player_code>().equip_now = player_code.equip_type.none;
                }
                else
                    {
                        freeze_player = true;
                        codeObj.global_words = my_words; 
                        while (player.GetComponent<player_code>().stun_now>0)
                        {
                        yield return new WaitForEndOfFrame();
                        }
    //assign item to to-do list
                        if (quest_item!=null)
                        {if (player.GetComponent<player_code>().quest_item!=null) { Destroy(player.GetComponent<player_code>().quest_item); player.GetComponent<player_code>().quest_item = null; }
                        GameObject to_do_item = Instantiate(quest_item);
                        to_do_item.gameObject.GetComponent<Collider>().enabled = false;
                        to_do_item.layer = 6;
                        to_do_item.GetComponent<interaction_code>().act_now = interaction_code.act_type.display;
                        to_do_item.transform.GetChild(0).gameObject.layer = 6;
                        player.GetComponent<player_code>().quest_item = to_do_item;
                        to_do_item.transform.parent=GameObject.Find("to_do").transform;
                        to_do_item.transform.position=GameObject.Find("to_do").transform.position;

                    }
                }
                codeObj.freeze_player = freeze_player;
                codeObj.talker = gameObject;
                StartCoroutine(codeObj.speak());
                yield return new WaitForSeconds(1.5f);
                transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = (true);   //get normal expression
                transform.GetChild(1).gameObject.GetComponent<Renderer>().enabled = (false); //hide mad expression
                break;
            case act_type.travel:
                player.GetComponent<player_code>().go_pos = transform.position;
                break;
        }
        transform.localScale *= 1.25f;

        yield return null;

    }
}
