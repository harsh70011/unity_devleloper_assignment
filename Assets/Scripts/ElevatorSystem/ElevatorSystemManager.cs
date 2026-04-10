using System.Collections.Generic;
using UnityEngine;

namespace ElevatorSim
{
    public class ElevatorSystemManager : MonoBehaviour
    {
        [SerializeField] private List<ElevatorController> elevators = new();
        [SerializeField] private int minFloor = 0;
        [SerializeField] private int maxFloor = 3;

        private readonly HashSet<HallCall> pendingHallCalls = new();

        public void RequestFromFloor(int floor, Direction direction)
        {
            if (floor < minFloor || floor > maxFloor)
            {
                Debug.LogWarning($"Ignoring floor request {floor}, outside valid range [{minFloor}, {maxFloor}].");
                return;
            }

            ElevatorController chosen = FindBestElevator(floor, direction);
            if (chosen == null)
            {
                Debug.LogError("No valid elevators configured in ElevatorSystemManager.");
                return;
            }

            HallCall hallCall = new(floor, direction);
            if (!pendingHallCalls.Add(hallCall))
            {
                return;
            }

            chosen.EnqueueFloor(floor);
            chosen.FloorArrived -= OnElevatorArrived;
            chosen.FloorArrived += OnElevatorArrived;
        }

        private ElevatorController FindBestElevator(int requestedFloor, Direction requestedDirection)
        {
            ElevatorController bestElevator = null;
            int bestCost = int.MaxValue;

            foreach (ElevatorController elevator in elevators)
            {
                if (elevator == null)
                {
                    continue;
                }

                if (elevator.HasQueuedFloor(requestedFloor))
                {
                    return elevator;
                }

                int cost = elevator.EstimatedCostForRequest(requestedFloor, requestedDirection);
                if (cost < bestCost)
                {
                    bestCost = cost;
                    bestElevator = elevator;
                }
            }

            return bestElevator;
        }

        private void OnElevatorArrived(ElevatorController elevator, int floor)
        {
            pendingHallCalls.Remove(new HallCall(floor, Direction.Up));
            pendingHallCalls.Remove(new HallCall(floor, Direction.Down));
            pendingHallCalls.Remove(new HallCall(floor, Direction.Idle));
        }

        private readonly struct HallCall
        {
            public int Floor { get; }
            public Direction Direction { get; }

            public HallCall(int floor, Direction direction)
            {
                Floor = floor;
                Direction = direction;
            }
        }
    }
}
