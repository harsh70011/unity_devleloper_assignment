using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ElevatorSim
{
    public class ElevatorController : MonoBehaviour
    {
        [Header("Identity")]
        [SerializeField] private int elevatorId;

        [Header("Movement")]
        [SerializeField] private float floorHeight = 2.75f;
        [SerializeField] private float speed = 2.5f;
        [SerializeField] private float arrivalThreshold = 0.05f;
        [SerializeField] private float doorWaitSeconds = 1f;

        [Header("UI")]
        [SerializeField] private TMP_Text floorDisplay;

        public int ElevatorId => elevatorId;
        public int CurrentFloor { get; private set; }
        public Direction CurrentDirection { get; private set; } = Direction.Idle;
        public bool IsMoving { get; private set; }

        private readonly List<int> requestQueue = new();
        private readonly HashSet<int> queuedFloors = new();
        private int targetFloor;
        private float waitTimer;

        public event Action<ElevatorController, int> FloorArrived;

        private void Start()
        {
            CurrentFloor = GetNearestFloorFromPosition();
            targetFloor = CurrentFloor;
            transform.position = GetFloorPosition(CurrentFloor);
            UpdateFloorDisplay();
        }

        private void Update()
        {
            if (waitTimer > 0f)
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    waitTimer = 0f;
                    TrySelectNextTarget();
                }

                return;
            }

            if (!IsMoving)
            {
                TrySelectNextTarget();
                return;
            }

            MoveTowardsTarget();
        }

        public void EnqueueFloor(int floor)
        {
            if (floor == targetFloor && IsMoving)
            {
                return;
            }

            if (floor == CurrentFloor && !IsMoving && waitTimer <= 0f)
            {
                waitTimer = doorWaitSeconds;
                FloorArrived?.Invoke(this, CurrentFloor);
                return;
            }

            if (queuedFloors.Add(floor))
            {
                requestQueue.Add(floor);
                ReorderQueue();
            }
        }

        public bool HasQueuedFloor(int floor)
        {
            return floor == targetFloor || queuedFloors.Contains(floor);
        }

        public int EstimatedCostForRequest(int requestedFloor, Direction requestedDirection)
        {
            if (!IsMoving && waitTimer <= 0f)
            {
                return Mathf.Abs(CurrentFloor - requestedFloor);
            }

            int baseCost = Mathf.Abs(CurrentFloor - requestedFloor);

            if (CurrentDirection == requestedDirection && IsFloorAhead(requestedFloor))
            {
                return baseCost;
            }

            if (CurrentDirection != Direction.Idle)
            {
                baseCost += 2;
            }

            baseCost += requestQueue.Count;
            return baseCost;
        }

        private void TrySelectNextTarget()
        {
            if (requestQueue.Count == 0)
            {
                IsMoving = false;
                CurrentDirection = Direction.Idle;
                targetFloor = CurrentFloor;
                return;
            }

            targetFloor = requestQueue[0];
            requestQueue.RemoveAt(0);
            queuedFloors.Remove(targetFloor);

            if (targetFloor == CurrentFloor)
            {
                waitTimer = doorWaitSeconds;
                FloorArrived?.Invoke(this, CurrentFloor);
                return;
            }

            IsMoving = true;
            CurrentDirection = targetFloor > CurrentFloor ? Direction.Up : Direction.Down;
        }

        private void MoveTowardsTarget()
        {
            Vector3 destination = GetFloorPosition(targetFloor);
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, destination) <= arrivalThreshold)
            {
                transform.position = destination;
                CurrentFloor = targetFloor;
                IsMoving = false;
                CurrentDirection = Direction.Idle;
                waitTimer = doorWaitSeconds;
                UpdateFloorDisplay();
                FloorArrived?.Invoke(this, CurrentFloor);
            }
        }

        private bool IsFloorAhead(int floor)
        {
            return CurrentDirection switch
            {
                Direction.Up => floor >= CurrentFloor,
                Direction.Down => floor <= CurrentFloor,
                _ => false
            };
        }

        private void ReorderQueue()
        {
            if (requestQueue.Count <= 1)
            {
                return;
            }

            requestQueue.Sort((a, b) =>
            {
                int da = Mathf.Abs(CurrentFloor - a);
                int db = Mathf.Abs(CurrentFloor - b);
                return da.CompareTo(db);
            });

            if (CurrentDirection == Direction.Up)
            {
                requestQueue.Sort((a, b) =>
                {
                    bool aAhead = a >= CurrentFloor;
                    bool bAhead = b >= CurrentFloor;
                    if (aAhead != bAhead)
                    {
                        return aAhead ? -1 : 1;
                    }

                    return a.CompareTo(b);
                });
            }
            else if (CurrentDirection == Direction.Down)
            {
                requestQueue.Sort((a, b) =>
                {
                    bool aAhead = a <= CurrentFloor;
                    bool bAhead = b <= CurrentFloor;
                    if (aAhead != bAhead)
                    {
                        return aAhead ? -1 : 1;
                    }

                    return b.CompareTo(a);
                });
            }
        }

        private Vector3 GetFloorPosition(int floor)
        {
            Vector3 pos = transform.position;
            pos.y = floor * floorHeight;
            return pos;
        }

        private int GetNearestFloorFromPosition()
        {
            return Mathf.RoundToInt(transform.position.y / floorHeight);
        }

        private void UpdateFloorDisplay()
        {
            if (floorDisplay == null)
            {
                return;
            }

            floorDisplay.text = $"E{elevatorId} : {CurrentFloor}";
        }
    }
}
