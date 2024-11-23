using System;
using UnityEngine;

public class interaction : MonoBehaviour
{
    public enum act_type { dialogue, equip, hold, quest };
    public act_type act_now;

    public string[] my_words = new string[] { "queer", "aren't you?" };

    private Vector3 original_size;
    public bool freeze_player = false;
    public string quest_item = "";
    //public GameObject dialogue_object;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        original_size= transform.localScale;
        //dialogue_object = GameObject.Find("Dialogue");
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = (transform.localScale*9+original_size)/10;
    }

    public void interact()
    {
        transform.localScale *= 1.25f;

        switch (act_now)
        {
            case act_type.dialogue:
                GameObject.Find("Dialogue").GetComponent<dialogue_code>().global_words=my_words;
                GameObject.Find("Dialogue").GetComponent<dialogue_code>().freeze_player = freeze_player;

                //dialogue_object.GetComponent<dialogue_code>().global_words=my_words;
                //StartCoroutine(dialogue_object.GetComponent<dialogue_code>().speak());
                StartCoroutine(GameObject.Find("Dialogue").GetComponent<dialogue_code>().speak());
                break;
            case act_type.quest:
                GameObject.Find("Dialogue").GetComponent<dialogue_code>().global_words = my_words;
                GameObject.Find("Dialogue").GetComponent<dialogue_code>().freeze_player = freeze_player;

                StartCoroutine(GameObject.Find("Dialogue").GetComponent<dialogue_code>().speak());
                //check what player is holding with substring    
                break;
        }

    }
}
