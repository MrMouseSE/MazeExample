using System;
using System.Collections.Generic;
using System.Linq;
using example.Scripts.MazeRulesDescriptions;
using example.Scripts.RoomScripts;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace example.Scripts.MazeGeneratorScripts
{
    public static class MazeRoomStaticFactory
    {
        private static MazePlacementDifficultyRule _mazeRoomsPlacementRule;
        private static RoomTypes[] _availableRoomTypes = { RoomTypes.Corridor, RoomTypes.Fight, RoomTypes.Rest, RoomTypes.Quest };

        private static int[] _connectionIndexesMap = new[] { 0, 1, 2, 3, 1, 0, 3, 2 };
        private static List<ConnectionDirection> _randomDirectionIndexesList = 
            Enum.GetValues(typeof(ConnectionDirection)).Cast<ConnectionDirection>().ToList();
        private static readonly int[] _oppositeDerectionsIndexes = new[] { 1, 0, 3, 2 };

        public static void SetCurrentDifficulty(MazePlacementDifficultyRule mazeRoomsPlacementRule)
        {
            _mazeRoomsPlacementRule = mazeRoomsPlacementRule;
            
        }
        
        public static void GenerateAdditionalRoomItem(List<RoomContainer> mazeRooms)
        {
            foreach (var mazeRoom in mazeRooms)
            {
                int additionalItemRandomCount = Random.Range(0, mazeRoom.RoomAdditionalItemsPlaces.Length)+1;
                for (var index = 0; index < additionalItemRandomCount; index++)
                {
                    var roomRule = _mazeRoomsPlacementRule.RoomsPlacementRules.Find(x => x.RoomType == mazeRoom.RoomType);
                    if (roomRule.AdditionalItems == null || roomRule.AdditionalItems.Length == 0) continue;
                    var roomAdditionalItemsPlace = mazeRoom.RoomAdditionalItemsPlaces[index];
                    Object.Instantiate(roomRule.AdditionalItems[Random.Range(0, roomRule.AdditionalItems.Length)], roomAdditionalItemsPlace);
                }
            }
        }

        public static RoomContainer CreateRoom()
        {
            var roomContainer = InstantiateRoom(RoomTypes.Start);
            ConnectionDirection nextConnectionDirection = GetRandomDirection();
            roomContainer.AddConnectionDirection((int)nextConnectionDirection);
            return roomContainer;
        }
        
        public static RoomContainer CreateRoom(int currentRoomPlacementStep, List<RoomContainer> mazeRooms)
        {
            
            RoomContainer previousRoomContainer = mazeRooms[^1];
            ConnectionDirection previousConnectionDirection = previousRoomContainer.GetLastConnectionDirection();
            RoomTypes newRoomType = GetRandomRoom(mazeRooms, currentRoomPlacementStep == -1);
            RoomContainer roomContainer = InstantiateRoom(newRoomType);
            ConnectionDirection nextConnectionDirection = GetNextConnectionDirection(previousConnectionDirection, mazeRooms, roomContainer);
            PlaceNewRoom(nextConnectionDirection, mazeRooms[^1], roomContainer);
            
            return roomContainer;
        }
        
        private static ConnectionDirection GetRandomDirection()
        {
            return (ConnectionDirection)Random.Range(0, _randomDirectionIndexesList.Count);
        }
        
        private static int GetOppositeDirection(ConnectionDirection direction)
        {
            return _oppositeDerectionsIndexes[(int)direction];
        }

        private static ConnectionDirection GetNextConnectionDirection(ConnectionDirection previousConnectionDirection, List<RoomContainer> mazeRooms, RoomContainer room)
        {
            var unavailableConnectionDirectionIndex = _oppositeDerectionsIndexes[(int)previousConnectionDirection];
            List<ConnectionDirection> newConnectionDirections =
                _randomDirectionIndexesList.FindAll(x => x != (ConnectionDirection)unavailableConnectionDirectionIndex);

            for (int i = 0; i < newConnectionDirections.Count; i++)
            {
                ConnectionDirection nextConnectionDirection = newConnectionDirections[Random.Range(0, newConnectionDirections.Count)];
                Vector3 newRoomPosition = GetRoomPosition(nextConnectionDirection, mazeRooms[^1], room);
                if (CheckNextPositionAvailable(mazeRooms, newRoomPosition))
                {
                    return nextConnectionDirection;
                }
                newConnectionDirections.Remove(nextConnectionDirection);
            }

            throw new NotImplementedException("Not available connection direction");
        }

        private static RoomTypes GetRandomRoom(List<RoomContainer> mazeRooms, bool isThisTheEndOfPath)
        {
            if(isThisTheEndOfPath) return RoomTypes.End;
            List<RoomTypes> currentAvailableRoomTypes = new List<RoomTypes>();
            foreach (var availableRoomType in _availableRoomTypes)
            {
                List<RoomContainer> roomsOfType = mazeRooms.FindAll(x => x.RoomType == availableRoomType);
                int maximumRoomOfType = _mazeRoomsPlacementRule.RoomsPlacementRules
                    .Find(x => x.RoomType == availableRoomType).MaximumCount;
                if (roomsOfType.Count != maximumRoomOfType) currentAvailableRoomTypes.Add(availableRoomType);
            }
            RoomTypes newRoomType = currentAvailableRoomTypes[Random.Range(0, currentAvailableRoomTypes.Count)];
            
            return newRoomType;
        }

        private static RoomContainer InstantiateRoom(RoomTypes start)
        {
            RoomContainer roomContainer = _mazeRoomsPlacementRule.RoomsPlacementRules.Find(x=>x.RoomType == start).RoomPrefab;
            RoomContainer newInstantiatedRoom = Object.Instantiate(roomContainer);
            newInstantiatedRoom.InitializeRoom();
            return newInstantiatedRoom;
        }

        private static void PlaceNewRoom(ConnectionDirection nextConnectionDirection, 
            RoomContainer previousRoom, RoomContainer newRoom)
        {
            Vector3 newRoomPosition = GetRoomPosition(previousRoom.GetLastConnectionDirection(), previousRoom, newRoom);
            newRoom.AddConnectionDirection(GetOppositeDirection(previousRoom.GetLastConnectionDirection()));
            if (newRoom.RoomType != RoomTypes.End) newRoom.AddConnectionDirection((int)nextConnectionDirection);
            newRoom.RoomTransform.position = newRoomPosition;
        }

        private static bool CheckNextPositionAvailable(List<RoomContainer> mazeRooms, Vector3 nextPosition)
        {
            foreach (var mazeRoom in mazeRooms)
            {
                //Todo: fix this for any concave bounds intersect
                if (mazeRoom.IsPointInRoomBound(nextPosition)) return false;
            }
            return true;
        }

        private static Vector3 GetRoomPosition(ConnectionDirection roomConnectionDirection, RoomContainer previousRoom, RoomContainer newRoom)
        {
            Vector3 connectionPosition = previousRoom.ConnectPoints[_connectionIndexesMap[(int)roomConnectionDirection]].position;
            Vector3 newRoomOffset = newRoom.ConnectPoints[_connectionIndexesMap[(int)roomConnectionDirection+4]].localPosition;
            return connectionPosition - newRoomOffset;
        }
    }
}
