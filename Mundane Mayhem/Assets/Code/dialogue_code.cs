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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //StartCoroutine(speak());
        my_text = gameObject.GetComponent<TextMeshProUGUI>();
        //my_text = gameObject.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator speak()
    {
        int line_max = global_words.Length;
        int line_now = 0;
        visible_characters = 0;
        yield return new WaitForSeconds(.2f);

        while (line_now<line_max)
        {
            //print(global_words[line_now]);
            while (visible_characters < global_words[line_now].Length) //click clack type
            {
                visible_characters += 1;
                yield return new WaitForSeconds(.05f);
                to_type = (global_words[line_now].Substring(0, Mathf.Clamp(visible_characters,0, global_words[line_now].Length)));
                my_text.text = to_type;

                if (freeze_player)
                { GameObject.Find("Player").GetComponent<player_code>().stun_now = .1f; }
            }
           
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

        yield return null;
    }
}