using Displayable;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace ShadowAlchemy.Player.UI
{
    public class ShadowManipulationDisplay : Display<ShadowManipulation>
    {
        [SerializeField] private GameObject graphicsParent;
        [SerializeField] private TMP_Text targetText;

        private void Awake()
        {
            if (graphicsParent == null) Debug.LogError($"{nameof(graphicsParent)} not assigned");
            if (targetText == null) Debug.LogError($"{nameof(targetText)} not assigned");

            SetListeners(null, displayObject);
            UpdateText();
        }

        private void SetListeners(ShadowManipulation oldObject, ShadowManipulation newObject)
        {
            if (oldObject)
            {
                oldObject.OnTargetChanged.RemoveListener(UpdateText);
                oldObject.OnTargetCanBeManipulated.RemoveListener(UpdateText);
                oldObject.OnManipulated.RemoveListener(UpdateText);
            }

            if (newObject)
            {
                newObject.OnTargetChanged.AddListener(UpdateText);
                newObject.OnTargetCanBeManipulated.AddListener(UpdateText);
                newObject.OnManipulated.AddListener(UpdateText);
            }
        }

        public override void SetObject(ShadowManipulation newObject)
        {
            SetListeners(displayObject, newObject);
            base.SetObject(newObject);
        }

        public override void UpdateText()
        {
            var target = displayObject.Target;
            targetText.text = target ? target.ActionName : "";

            graphicsParent.SetActive(target && target.CanBeManipulated);
        }
    }
}