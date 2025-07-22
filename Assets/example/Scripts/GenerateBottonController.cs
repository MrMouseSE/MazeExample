using example.Scripts.MazeRulesDescriptions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace example.Scripts
{
    public class GenerateBottonController : MonoBehaviour
    {
        public DummyGameController GameController;
        public Button GenerateButton;
        public Slider DifficultySlider;
        public MazeRoomsPlacementRules MazeGeneratorRules;
        public Camera Camera;
        public float ZoomInDistance = 7f;
        public float CameraZoomSpeed = 1.5f;
        public AnimationCurve CameraZoomBehaviour;

        private Vector3 _cameraPosition;
        private Vector3 _cameraCurrentPosition;
        private Vector3 _cameraPreviousPosition;
        private float _currentZoomProgressValue;
        
        

        private void Start()
        {
            GenerateButton.onClick.AddListener(GenerateMaze);
            DifficultySlider.minValue = 0;
            DifficultySlider.maxValue = MazeGeneratorRules.MazeDifficultyRules.Length - 1;
        }

        private void GenerateMaze()
        {
            GameController.GenerateMaze((int)DifficultySlider.value, MazeGeneratorRules);
            var cameraPosition = GameController.GetMazeCenterPosition();
            cameraPosition.z = -20 - ((int)DifficultySlider.value * 7);
            _cameraPosition = cameraPosition;
            Camera.transform.position = _cameraPosition;
        }
        
        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _cameraPreviousPosition = Camera.transform.position;
                Vector3 newCameraPosition = GameController.GetCurrentMouseTouchRoomPosition(Camera, Mouse.current.position.ReadValue());
                _cameraCurrentPosition = newCameraPosition.sqrMagnitude > 1000 ? _cameraPosition : new Vector3(newCameraPosition.x, newCameraPosition.y, -ZoomInDistance);
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                _cameraPreviousPosition = _cameraPosition;
                _cameraCurrentPosition = Camera.transform.position;
            }

            float offsetValue = Mouse.current.leftButton.isPressed ? Time.deltaTime : -Time.deltaTime;
            _currentZoomProgressValue = Mathf.Clamp01(_currentZoomProgressValue + offsetValue * CameraZoomSpeed);

            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            var progressValue = CameraZoomBehaviour.Evaluate(_currentZoomProgressValue);
            Camera.transform.position = Vector3.Lerp(_cameraPreviousPosition,_cameraCurrentPosition,progressValue);
        }
    }
}
