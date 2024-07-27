using UnityEngine;

namespace ShadowAlchemy.Player
{
    public class LightDeath : MonoBehaviour
    {
        [SerializeField] private ShadowTraversal player;
        [Tooltip("Optional reset transform. Will use " + nameof(player) + "'s position and rotation at " + nameof(Start) + "() otherwise")]
        [SerializeField] private Transform startTransform;

        private Vector3 startPosition;
        private Quaternion startRotation;

        private void Awake()
        {
            if (player == null) Debug.LogError($"{nameof(player)} not assigned");

            player.OnShadowExited.AddListener(ResetPlayerToStart);
        }

        private void Start()
        {
            startPosition = player.transform.position;
            startRotation = player.transform.rotation;
        }

        private void ResetPlayerToStart()
        {
            var position = startTransform ? startTransform.position : startPosition;
            var rotation = startTransform ? startTransform.rotation : startRotation;
            player.transform.SetLocalPositionAndRotation(position, rotation);
        }
    }
}