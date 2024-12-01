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
    public AudioSource voice;
    public GameObject talker;
    private GameObject dialoguebox;
    public bool zoom_cam = false;
    public static bool talking = false;
    private bool silent = false;

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
        talking = true;
        Camera cam = Camera.main;
        dialoguebox.transform.localPosition=Vector3.zero;
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
                if (voice!=null&&!silent) 
                { 
                    //GetComponent<AudioSource>().clip=voice; 
                    GetComponent<AudioSource>().Play(); 
                    print(voice);
                    //voice.Play();
                }
                if (talker!=null&& !silent) { talker.transform.localScale = new Vector3(1-wobble, 1+wobble, 1-wobble); }

                yield return new WaitForSeconds(.0125f);
                to_type = (global_words[line_now].Substring(0, Mathf.Clamp(visible_characters,0, global_words[line_now].Length)));
                my_text.text = to_type;

                //pauses in speech
                if (visible_characters>0&&visible_characters< global_words[line_now].Length-1)
                {string thischunk = global_words[line_now].Substring(visible_characters-1, 1);
                    print(thischunk);
                    switch (thischunk)
                    {
                        case "(":
                            silent = true; print(silent);  break;
                        case ")":
                            silent = false; print(silent); break;
                        //case " ":
                        //    yield return new WaitForSeconds(.01f);
                        //    break;
                        case ".":
                            yield return new WaitForSeconds(.01f);
                            break;
                        case "?":
                            yield return new WaitForSeconds(.02f);
                            break;
                    }
                }
                if (freeze_player)
                { GameObject.Find("Player").GetComponent<player_code>().stun_now = .3f; }
            }
            yield return new WaitForSeconds(.025f);

            while (!Input.GetButton("Fire1")) 
            { 
                if (freeze_player)
                { GameObject.Find("Player").GetComponent<player_code>().stun_now = .2f; }
                yield return new WaitForFixedUpdate();
                
            }
            visible_characters = 0;
            line_now += 1;
            if (zoom_cam) { cam.fieldOfView -= 15; }
        }
        my_text.text = "";
        talker = null;
        voice = null;
        talking = false;
        cam.fieldOfView = 60;
        //lower dialouge box
        while (!talking) { yield return new WaitForSeconds(.01f); dialoguebox.transform.position -= Vector3.up*.1f; }
        
        yield return null;
        
    }
}
