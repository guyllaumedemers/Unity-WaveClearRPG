using UnityEngine;

namespace Characters.Player
{
    public class PlayerWorldInteraction : MonoBehaviour
    {

        public LayerMask interactLayer;

        private UnityEngine.AI.NavMeshAgent _playerAgent;

        void Start()
        {
            _playerAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }

        void Update()
        {

            if (GameController.Instance._canInteract && !global::Player.Instance._isDisplay)
                if (Input.GetMouseButtonDown(0))
                    GetInteraction();

            if (_playerAgent.remainingDistance > 0.05f)
            {
                if (!global::Player.Instance.Animator.GetBool("isMoving"))
                    global::Player.Instance.Animator.SetBool("isMoving", true);
            }
            else if (global::Player.Instance.Animator.GetBool("isMoving"))
            {
                global::Player.Instance.Animator.SetBool("isMoving", false);
            }

            Debug.DrawRay(transform.position, transform.forward * 2f, Color.red);
        }

        void GetInteraction()
        {
            Ray interactionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.SphereCast(interactionRay, 1f, out var interactionInfo, Mathf.Infinity, interactLayer))
            {
                _playerAgent.updateRotation = true;
                _playerAgent.destination = interactionInfo.point;
            }
        }
    }
}
