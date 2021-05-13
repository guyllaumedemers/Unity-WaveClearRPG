using UnityEngine;
using UnityEngine.EventSystems;

namespace Characters.Player
{
    [ExecuteInEditMode]
    public class CameraController : MonoBehaviour {
        public float playerCameraDistance;
        public Transform cameraTarget;
        public float zoomSpeed = 35f;
        
        private Camera _playerCamera;

        public float _cameraHeight = 10f;
        public float _cameraRotateSpeed = 3f;
        public float _cameraHeightChangeSpeed = 12f;
        private float _cameraRadius = 12f;
        private float _cameraAngle = 4.7f;
        

        void Start() {
            _playerCamera = GetComponent<Camera>();
        }
        
        void Update() {

            float cameraX = cameraTarget.position.x + (_cameraRadius * Mathf.Cos(_cameraAngle));
            float cameraY = cameraTarget.position.y + _cameraHeight;
            float cameraZ = cameraTarget.position.z + (_cameraRadius * Mathf.Sin(_cameraAngle));

            if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                _playerCamera.fieldOfView -= scroll * zoomSpeed;
                _playerCamera.fieldOfView = Mathf.Clamp(_playerCamera.fieldOfView, 20, 100);
            }

            transform.position = new Vector3(cameraX, cameraY, cameraZ);

            if (Input.GetKey(KeyCode.A))
            {
                _cameraAngle = _cameraAngle - _cameraRotateSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                _cameraAngle = _cameraAngle + _cameraRotateSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.W))
            {
                _cameraHeight = _cameraHeight - _cameraHeightChangeSpeed * Time.deltaTime;
                _cameraHeight = Mathf.Clamp(_cameraHeight, 2.5f, 20);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                _cameraHeight = _cameraHeight + _cameraHeightChangeSpeed * Time.deltaTime;
                _cameraHeight = Mathf.Clamp(_cameraHeight, 2.5f, 20);
            }

            transform.LookAt(cameraTarget.position);
        }
    }
}
