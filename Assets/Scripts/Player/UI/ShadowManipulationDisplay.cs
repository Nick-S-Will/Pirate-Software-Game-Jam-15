using Displayable;
using TMPro;
using UnityEngine;

namespace ShadowAlchemy.Player.UI
{
    public class ShadowManipulationDisplay : Display<ShadowManipulation>
    {
        [SerializeField] private GameObject graphicsParent;
        [SerializeField] private TMP_Text targetText;

        private void Awake()
        {
            if (displayObject == null) Debug.LogError($"{nameof(displayObject)} not assigned");
            if (graphicsParent == null) Debug.LogError($"{nameof(graphicsParent)} not assigned");
            if (targetText == null) Debug.LogError($"{nameof(targetText)} not assigned");

            displayObject.OnTargetChanged.AddListener(UpdateText);
            displayObject.OnManipulated.AddListener(UpdateText);

            UpdateText();
        }

        public override void UpdateText()
        {
            var target = displayObject.Target;
            targetText.text = target ? target.ActionName : "";

            graphicsParent.SetActive(target);
        }
    }
}