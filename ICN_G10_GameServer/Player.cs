using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ICN_G10_GameServer
{
    // For all player data and logic
    class Player
    {
        public int id;
        public string username;

        public Vector3 position;
        public Quaternion rotation;

        public Player(int _id, string _username, Vector3 _position)
        {
            id = _id;
            username = _username;
            position = _position;
            rotation = Quaternion.Identity;
        }
    }
}
