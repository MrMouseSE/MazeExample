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

        public Vector3 GetCurrentMouseTouchRoomPosition(Camera currentCamera, Vector2 mousePosition)
        {
            var rooms = _mazeGeneratorController.GetMazeRooms();
            if (rooms == null || rooms.Count == 0) return Vector3.positiveInfinity;
            foreach (var room in rooms)
            {
                foreach (var roomCollider in room.RoomColliders)
                {
                    Vector2 point = currentCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, roomCollider.transform.position.z - currentCamera.transform.position.z));
                    if(roomCollider.OverlapPoint(point)) return room.RoomTransform.position;
                }
            }
            return Vector3.positiveInfinity;
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
