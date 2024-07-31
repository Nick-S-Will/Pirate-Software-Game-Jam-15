using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShadowAlchemy.NPC
{
    public class NPC_TextAppear : MonoBehaviour
    {
        //note: this code should be on every npc/GO with a text bubble option

        #region Variables
        public GameObject shen;
        private Vector3 shenPosition;

        public GameObject textBubbleCanvas;
        public float minimumDistance; //will depend on situation
        public UnityEvent ShenInRadius;
        private bool isDistanceInRange = false;
        #endregion

        #region Start()
        public void Start()
        {
            textBubbleCanvas.SetActive(false);
        }
        #endregion

        #region Update()
        public void Update()
        {
            if (CanRun())
            {
                shenPosition = shen.transform.position; //get the world location of shen
                OnShenInRadius();

            }

        }
        #endregion

        #region On Shen In Radius()
        //If Shen is close to npc (based on minimum distance), show text bubble. Else, disappear
        private void OnShenInRadius()
        {
            isDistanceInRange = (Vector3.Distance(this.transform.position, shenPosition) <= minimumDistance);
            textBubbleCanvas.SetActive(isDistanceInRange ? true : false);
            ShenInRadius.Invoke();
        }
        #endregion

        #region Debug
        //checks if it has all it needs - optimization purposes
        private bool CanRun()
        {
            if (shen == null)
            {
                Debug.Log("No Shen reference; NPC_TextAppear will not work");
                return false;
            }
            if (textBubbleCanvas == null)
            {
                Debug.Log("No Text Bubble Canvas reference; NPC_TextAppear will not work");
                return false;
            }
            if (minimumDistance <= 0)
            {
                Debug.Log("Minimum distance to compare must be >0.01");
                return false;
            }
            //all good then :)
            return true;


        }
        #endregion

    }
}