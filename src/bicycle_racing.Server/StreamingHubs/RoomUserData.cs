using bicycle_racing.Shared.Interfaces.StreamingHubs;
using UnityEngine;

namespace bicycle_racing.Server.StreamingHubs
{
    public class RoomUserData
    {
        public JoinedUser JoinedUser;
        public Vector3 pos;
        public int checkCount = 0;
        public bool isReady = false; 
    }
}
