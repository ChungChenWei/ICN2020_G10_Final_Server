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

        private float moveSpeed = 5f / Constants.TICKS_PER_SEC;
        private bool[] inputs;

        public Player(int _id, string _username, Vector3 _position)
        {
            id = _id;
            username = _username;
            position = _position;
            rotation = Quaternion.Identity;

            inputs = new bool[4];
        }

        public void Update()
        {
            Vector2 _inputDirection = Vector2.Zero;
            if (inputs[0])
            {
                _inputDirection.Y += 1;
            }
            if (inputs[1])
            {
                _inputDirection.Y -= 1;
            }
            if (inputs[2])
            {
                _inputDirection.X += 1;
            }
            if (inputs[3])
            {
                _inputDirection.X -= 1;
            }

            if(_inputDirection.Length() > 0)
            {
                Move(_inputDirection);
            }
        }

        private void Move(Vector2 _inputDirection)
        {
            //// Direction Player is facing
            //Vector3 _forward = Vector3.Transform(new Vector3(0, 0, 1), rotation);
            //Vector3 _right = Vector3.Normalize(Vector3.Cross(_forward, new Vector3(0, 1, 0)));

            //Vector3 _moveDirection = _right * _inputDirection.X + _forward * _inputDirection.Y;
            Vector3 _moveDirection = new Vector3(-_inputDirection.X, _inputDirection.Y, 0);

            position += _moveDirection * moveSpeed;

            ServerSend.PlayerPosition(this);
            // We Send rotation to other people
            ServerSend.PlayerRotation(this);
        }

        public void SetInput(bool[] _inputs, Quaternion _rotation)
        {
            inputs = _inputs;
            rotation = _rotation;
        }
    }
}
