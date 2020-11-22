using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testTextInput : MonoBehaviour
{
   
    float nextTime;
    Text inputText;

    // Start is called before the first frame update
    void Start()
    {
        nextTime = Time.time + 20f;
        inputText = gameObject.GetComponent<Text>();
        inputText.text = "Hello dude";
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Time.time.ToString() + ", " + nextTime.ToString());
        if (Time.time > nextTime)
        {
        //    Debug.Log("interval");
            inputText.text = "Yeah";
            nextTime += 20;
        }

    }
}
