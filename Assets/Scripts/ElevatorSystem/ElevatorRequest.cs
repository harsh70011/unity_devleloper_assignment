using System;

namespace ElevatorSim
{
    [Serializable]
    public struct ElevatorRequest
    {
        public int Floor;
        public Direction Direction;

        public ElevatorRequest(int floor, Direction direction)
        {
            Floor = floor;
            Direction = direction;
        }
    }

    public enum Direction
    {
        Idle = 0,
        Up = 1,
        Down = -1
    }
}
