using UnityEngine;

namespace ElevatorSim
{
    public class FloorCallButton : MonoBehaviour
    {
        [SerializeField] private ElevatorSystemManager elevatorSystem;
        [SerializeField] private int floorNumber;

        public void RequestUp()
        {
            elevatorSystem.RequestFromFloor(floorNumber, Direction.Up);
        }

        public void RequestDown()
        {
            elevatorSystem.RequestFromFloor(floorNumber, Direction.Down);
        }

        public void RequestAny()
        {
            elevatorSystem.RequestFromFloor(floorNumber, Direction.Idle);
        }
    }
}
