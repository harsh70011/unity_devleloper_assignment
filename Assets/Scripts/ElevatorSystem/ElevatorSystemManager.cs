using System.Collections.Generic;
using UnityEngine;

namespace ElevatorSim
{
    public class ElevatorSystemManager : MonoBehaviour
    {
        [SerializeField] private List<ElevatorController> elevators = new();
        [SerializeField] private int minFloor = 0;
        [SerializeField] private int maxFloor = 3;

        private readonly HashSet<int> pendingHallCalls = new();

        public void RequestFromFloor(int floor, Direction direction)
        {
            if (floor < minFloor || floor > maxFloor)
            {
                Debug.LogWarning($"Ignoring floor request {floor}, outside valid range [{minFloor}, {maxFloor}].");
                return;
            }

            if (elevators.Count == 0)
            {
                Debug.LogError("No elevators configured in ElevatorSystemManager.");
                return;
            }

            if (!pendingHallCalls.Add(floor))
            {
                return;
            }

            ElevatorController chosen = FindBestElevator(floor, direction);
            chosen.EnqueueFloor(floor);
            chosen.FloorArrived -= OnElevatorArrived;
            chosen.FloorArrived += OnElevatorArrived;
        }

        private ElevatorController FindBestElevator(int requestedFloor, Direction requestedDirection)
        {
            ElevatorController bestElevator = elevators[0];
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
            pendingHallCalls.Remove(floor);
        }
    }
}
