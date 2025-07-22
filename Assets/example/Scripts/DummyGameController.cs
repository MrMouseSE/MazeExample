using example.Scripts.MazeGeneratorScripts;
using example.Scripts.MazeRulesDescriptions;
using UnityEngine;

namespace example.Scripts
{
    public class DummyGameController : MonoBehaviour
    {
        private MazeGeneratorController _mazeGeneratorController;
        
        private void Start()
        {
            _mazeGeneratorController = new MazeGeneratorController();
        }

        public void GenerateMaze(int currentDifficulty, MazeRoomsPlacementRules mazeGeneratorRules)
        {
            _mazeGeneratorController.SetCurrentMazeRules(mazeGeneratorRules.MazeDifficultyRules[currentDifficulty]);
            _mazeGeneratorController.GenerateMaze();
        }

        public Vector3 GetMazeCenterPosition()
        {
            var rooms = _mazeGeneratorController.GetMazeRooms();
            Vector3 roomCenter = Vector3.zero;
            foreach (var room in rooms)
            {
                roomCenter += room.RoomTransform.position;
            }
            roomCenter /= rooms.Count;
            return roomCenter;
        }
    }
}
