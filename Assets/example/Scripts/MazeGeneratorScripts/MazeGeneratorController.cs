using System.Collections.Generic;
using example.Scripts.MazeRulesDescriptions;
using example.Scripts.RoomScripts;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace example.Scripts.MazeGeneratorScripts
{
    public class MazeGeneratorController
    {
        private MazePlacementDifficultyRule _currentMazePlacementRules;
        private List<RoomContainer> _mazeRooms;
        
        public void SetCurrentMazeRules(MazePlacementDifficultyRule currentMazePlacementRules)
        {
            _currentMazePlacementRules = currentMazePlacementRules;
        }

        public void GenerateMaze()
        {
            DestroyPreviousGeneration();
            _mazeRooms = new List<RoomContainer>();
            int currentGenerationStep = Random.Range(_currentMazePlacementRules.MainPathMinMaxSteps.x, 
                _currentMazePlacementRules.MainPathMinMaxSteps.y);
            GenerateMainPath(currentGenerationStep);
            MazeRoomStaticFactory.GenerateAdditionalRoomItem(_mazeRooms);
        }

        public List<RoomContainer> GetMazeRooms()
        {
            return _mazeRooms;
        }

        private void DestroyPreviousGeneration()
        {
            if (_mazeRooms == null) return;
            foreach (var mazeRoom in _mazeRooms) Object.Destroy(mazeRoom.RoomGameObject);
            _mazeRooms.Clear();
        }

        private void GenerateMainPath(int pathSteps)
        {
            MazeRoomStaticFactory.SetCurrentDifficulty(_currentMazePlacementRules);
            GenerateStartRoom();
            for (int currentPathStep = 1; currentPathStep < pathSteps + 1; currentPathStep++)
            {
                var stepForGeneration = currentPathStep == pathSteps ? -1 : currentPathStep;
                RoomContainer newRoom = MazeRoomStaticFactory.CreateRoom(stepForGeneration, _mazeRooms);
                AddRoomToList(newRoom);
            }
        }

        private void GenerateStartRoom()
        {
            RoomContainer newRoom = MazeRoomStaticFactory.CreateRoom();
            AddRoomToList(newRoom);
        }

        private void AddRoomToList(RoomContainer room)
        {
            _mazeRooms.Add(room);
        }
    }
}
