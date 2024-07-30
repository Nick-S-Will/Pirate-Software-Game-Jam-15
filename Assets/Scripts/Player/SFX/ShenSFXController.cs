using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ShadowAlchemy.Player
{

public class ShenSFXController : MonoBehaviour
{

    #region Variables

    [Tooltip("GameObject typed Shen; Shen prefab")]
    public GameObject shen;

    //audio sources
    [Tooltip("GameObject type; use MoveSFX GO")] public GameObject movingAudioSourceGO;

    //manipulateSFX plays on OnManipulated() event in Shen prefab > Shadow Manipulation in inspector
    [Tooltip("AudioSource type; use ManipulationSFX GO")]public AudioSource manipulationAudioSource; 
    [Tooltip("AudioSource type; use DeathSFX GO")]public AudioSource deathAudioSource;

    
    //specifically needed for MoveSFX to work/debug it
    [Header("Moving Audio")]
    [Tooltip("ShadowReversal type; use Shen prefab(it will grab what it needs)")]public ShadowTraversal shenRef;
    [SerializeField]private Vector2 shadowRefInput => shenRef.MoveInput;  

    [Tooltip("See ShadowTraversal.MoveInput visually; for debugging purposes")]public Vector2 viewableMoveInput; //change to a debug related name later
    #endregion


#region Update(), for movement
    public void Update()
    {
        viewableMoveInput = shadowRefInput.normalized;

        if(viewableMoveInput == new Vector2(0,0)){
            movingAudioSourceGO.SetActive(false); //deactivate that shit
        }

        //meaning, there's movement!
        else{
            if(!movingAudioSourceGO.activeInHierarchy){ //if it's not already active, then turn it on!
                movingAudioSourceGO.SetActive(true);
            }
        }
    }
    #endregion

#region playSFX methods

    //runs on the lightDeath.Invoke(); in the light death handler
    public void playDeathSFX(bool soundActivated){
        deathAudioSource.Play();
    }

    #endregion

}
}
