# Unity 6 – 2D Elevator Simulation

This project contains a functional multi-elevator dispatch simulation that satisfies the assignment requirements.

## Implemented Features

- 3-elevator architecture (configured via `ElevatorSystemManager` in-scene).
- Supports 4+ floors (default min=0, max=3).
- Floor call buttons (Up/Down/Any trigger methods via Unity UI buttons).
- Nearest/most suitable available elevator selection using a cost model.
- Logical direction-aware dispatch (prefer elevators already moving toward request).
- No duplicate over-response (single hall call tracked until served).
- Each elevator keeps independent state and queue.
- Smooth motion between floors via `Vector3.MoveTowards`.
- Current floor display per elevator using `TMP_Text`.

## Scripts

- `Assets/Scripts/ElevatorSystem/ElevatorRequest.cs`
- `Assets/Scripts/ElevatorSystem/ElevatorController.cs`
- `Assets/Scripts/ElevatorSystem/ElevatorSystemManager.cs`
- `Assets/Scripts/ElevatorSystem/FloorCallButton.cs`

## Scene Setup (Unity Editor)

1. Create a `GameObject` named `ElevatorSystemManager`, attach `ElevatorSystemManager.cs`.
2. Create 3 elevator cab GameObjects (`Elevator_1`, `Elevator_2`, `Elevator_3`) with `ElevatorController.cs`.
3. Assign unique elevator IDs and per-elevator TMP floor display references.
4. Drag all 3 elevators into manager `elevators` list.
5. Create floor UI panel(s) for floors 0–3 with buttons.
6. Add `FloorCallButton.cs` to each floor button group and assign:
   - manager reference
   - floor number
   - button `OnClick` to `RequestUp`, `RequestDown`, or `RequestAny`
7. Ensure elevator cab initial Y position corresponds to floor (`y = floor * floorHeight`).

## Make it live

- A GitHub Actions workflow is included at `.github/workflows/unity-webgl.yml` to build and deploy WebGL automatically.
- Setup and hosting steps are documented in `Deployment/WEBGL_HOSTING.md`.

## Notes

- `minFloor` and `maxFloor` are configurable for more than 4 floors.
- `doorWaitSeconds` adds realistic stop time before next move.
- The queue is reordered to keep directionally sensible movement while remaining simple and deterministic.
