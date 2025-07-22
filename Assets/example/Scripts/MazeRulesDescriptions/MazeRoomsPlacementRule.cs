using example.Scripts.AdditionalItemsScripts;
using example.Scripts.RoomScripts;

namespace example.Scripts.MazeRulesDescriptions
{
    [System.Serializable]
    public class MazeRoomsPlacementRule
    {
        public RoomTypes RoomType;
        public RoomContainer RoomPrefab;
        public int MaximumCount;
        public AdditionalItemContainer[] AdditionalItems;
    }
}