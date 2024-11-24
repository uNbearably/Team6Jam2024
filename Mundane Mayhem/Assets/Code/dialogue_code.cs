using System;
using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class dialogue_code : MonoBehaviour
{
    public string[] global_words = new string[] { "queer", "aren't you?" };
    public TextMeshProUGUI my_text;
    public bool freeze_player = false;
    private int visible_characters = 0;
    private string to_type = "";
    public AudioClip voice;
    public GameObject talker;
    private GameObject dialoguebox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //StartCoroutine(speak());
        my_text = gameObject.GetComponent<TextMeshProUGUI>();
        dialoguebox = GameObject.Find("DialogueBox");
        //my_text = gameObject.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator speak()
    {
        dialoguebox.transform.position=Vector3.zero;
        int line_max = global_words.Length;
        int line_now = 0;
        visible_characters = 0;
        yield return new WaitForSeconds(.02f);

        while (line_now<line_max)
        {
            //print(global_words[line_now]);
            while (visible_characters < global_words[line_now].Length) //click clack type
            {
                visible_characters += 1;
                float wobble = UnityEngine.Random.Range(0, .2f);
                if (voice!=null) 
                { 
                    GetComponent<AudioSource>().clip=voice; 
                    GetComponent<AudioSource>().Play(); 
                }
                if (talker!=null) { talker.transform.localScale = new Vector3(1-wobble, 1+wobble, 1-wobble); }

                yield return new WaitForSeconds(.0125f);
                to_type = (global_words[line_now].Substring(0, Mathf.Clamp(visible_characters,0, global_words[line_now].Length)));
                my_text.text = to_type;

                //pauses in speech
                if (visible_characters>1&&visible_characters< global_words[line_now].Length-1)
                {string thischunk = global_words[line_now].Substring(visible_characters-1, 2);

                    switch (thischunk)
                    {
                        case "  ":
                            yield return new WaitForSeconds(.01f);
                            break;
                        case ". ":
                            yield return new WaitForSeconds(.02f);
                            break;
                        case "? ":
                            yield return new WaitForSeconds(.04f);
                            break;
                    }
}
                if (freeze_player)
                { GameObject.Find("Player").GetComponent<player_code>().stun_now = .1f; }
            }
            yield return new WaitForSeconds(.05f);

            while (!Input.GetButton("Fire1")) 
            { 
                if (freeze_player)
                { GameObject.Find("Player").GetComponent<player_code>().stun_now = .1f; }
                yield return new WaitForFixedUpdate();
                
            }
            visible_characters = 0;
            line_now += 1;
        }
        my_text.text = "";
        talker = null;
        voice = null;
        while (voice == null) { yield return new WaitForSeconds(.01f); dialoguebox.transform.position -= Vector3.up*.01f; }
        yield return null;
    }
}
