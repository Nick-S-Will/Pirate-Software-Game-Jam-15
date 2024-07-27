using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShadowAlchemy.Player{
public class FaceRotation : MonoBehaviour
{

    #region Variables
    [Header("Shen")] [Tooltip("Shen GameObject, not the graphics")] public GameObject shen;
    
    //What the current input is; from ShadowTraversal script
    private Vector2 shadowRefInput => ShadowTraversal.moveInput;  //readonly

    public UnityEvent FaceMoving; 
    
    [Header("Debug")]
    
    //Position-based (on Shen GO)
    [Tooltip("Based on Shen GO from last Update()")] [SerializeField] private Vector3 lastPosition;
    [Tooltip("Based on Shen GO")] [SerializeField] private Vector3 currentPosition;

    [Tooltip("Show last and current position of Shen GO in console to check inspector numbers")] public bool showPositionInConsole;
    [Tooltip("Shows direction of player movement; i.e. x = -1 is left")]public Vector2 shadowInputDirection; //viewing shadowRefInput in realtime
    #endregion

    #region Start()

    public void Start()
    {
        if (shen==null) {Debug.Log("No Shen Game object");}
        else{
             UpdateDebugValues();
             lastPosition = shen.transform.localPosition;
        }
      
    }
    #endregion

    #region Update()
    public void Update()
    {
        currentPosition = shen.transform.localPosition;

        if(showPositionInConsole){
            Debug.Log(lastPosition+" :lastPosition");
            Debug.Log(currentPosition+" :currentPosition");
        }

        if(lastPosition != currentPosition) OnFaceMoving(); 

        UpdateDebugValues();
        lastPosition = currentPosition;

    }
    #endregion


    #region On Face Moving()
    //event
    public void OnFaceMoving(){

        //https://stackoverflow.com/questions/36781086/how-to-convert-vector3-to-quaternion - function

        switch(Mathf.Abs(shadowRefInput.x)){
            case 0:
            case 1:
                //Debug.Log("Straight");


                if(shadowRefInput.x == 1) {this.transform.localRotation = Quaternion.Euler(new Vector3(0.0f,0.0f,270.0f));} //going right
                else if(shadowRefInput.x == -1) {this.transform.localRotation = Quaternion.Euler(new Vector3(0.0f,0.0f,90.0f));} //going left
                else if(shadowRefInput.y == 1) {this.transform.localRotation = Quaternion.Euler(new Vector3(0.0f,0.0f,0.0f));} //going up
                else if(shadowRefInput.y == -1) {this.transform.localRotation = Quaternion.Euler(new Vector3(0.0f,0.0f,180.0f));} //going down
                
                break;

            case 0.707107f: 
                //Debug.Log("Diagonal");

                if(shadowRefInput.x == 0.707107f && shadowRefInput.y == 0.707107f) {this.transform.localRotation = Quaternion.Euler(new Vector3(0.0f,0.0f,315.0f));} //right-up
                else if(shadowRefInput.x == -0.707107f && shadowRefInput.y == -0.707107f) {this.transform.localRotation = Quaternion.Euler(new Vector3(0.0f,0.0f,135.0f));} //left-down
                else if(shadowRefInput.x == -0.707107f && shadowRefInput.y == 0.707107f) {this.transform.localRotation = Quaternion.Euler(new Vector3(0.0f,0.0f,45.0f));} //left-up
                else if(shadowRefInput.x == 0.707107f && shadowRefInput.y == -0.707107f) {this.transform.localRotation = Quaternion.Euler(new Vector3(0.0f,0.0f,225.0f));} //right-down
                
                break;

            default:
                Debug.Log("Invalid input \nInput x: "+shadowRefInput.x+" || Input y: "+shadowRefInput.y);
                break;
        }

        FaceMoving.Invoke();
    }
    #endregion


    #region Debug
    //Updates the values
    private void UpdateDebugValues(){
         //for debugging/visualization purposes
        shadowInputDirection = shadowRefInput;  
    }
    #endregion
}
}
