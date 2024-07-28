using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPC_TextAppear : MonoBehaviour
{

    //variable
    public GameObject textBubble;

    public UnityEvent ShenInRadius;



    public void Start(){
        textBubble.gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void Update()
    {
        //&& this.Collider2D.isTrigger //tired, update tomorrow
        if (textBubble == null) {
            Debug.Log("There is no text bubble set to check");
        }

        else{
            // if collider is triggered
            // textBubble.gameObject.SetActive(true);
            Debug.Log("Meaning this is triggered");

        }


    }

//if you come close, it'll activate and words will pop up. else, will dissappear/not active in scene
    private void OnShenInRadius(){}

}
