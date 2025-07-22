using example.Scripts.MazeRulesDescriptions;
using UnityEngine;
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
            Camera.transform.position = cameraPosition;
        }
    }
}
