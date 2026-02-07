# Plane Detection AR Template

An AR Foundation sample focused on reliable plane detection, responsive placement controls, and lightweight diagnostics. The project targets Unity **6000.2.6f2** (Unity 6) and ships with XR Interaction Toolkit, ARCore XR Plug-in, and AR Foundation 6.2, making it a solid starting point for Android or iOS AR prototypes.

## Feature Highlights
- **Plane management UI** – `ARModeController` lets you toggle detection, hide planes, remove tracked planes, or delete spawned content while guarding other placement scripts through a shared `IsPlacementAllowed` flag.
- **Multiple placement modes** – Use tap-to-place planes, drag-to-place gestures, or feature-point placement (`cloudpointToPlacePrefab`) for content that should cling to point clouds rather than surfaces.
- **Image tracking pipeline** – `ImageTrackingManager` instantiates prefabs for each reference image at runtime and keeps them aligned while the target stays in view.
- **Menu and debug overlays** – `ARTemplateMenuManager` and `ARDebugMenu` hook into XR Interaction Toolkit Starter Assets so you can spawn prefabs from a modal menu, toggle plane visuals, and surface debug sliders without writing extra UI glue.
- **Performance HUD** – `PerformanceMonitor` streams memory and GPU timing data into a TextMeshPro label so you can benchmark directly on device.
- **Curated demo scenes** – The `Assets/Scenes` folder includes sample experiences (ScenePrime, ImageTrackingScene, carScene, BallShootingGameScene, Scene_Dragon, SceneTest) that showcase different interaction styles.

## Project Layout
| Folder | Purpose |
| --- | --- |
| `Assets/MobileARTemplateAssets` | Core scripts, prefabs, materials, and UI assets used across the demo scenes. |
| `Assets/Scenes` | Ready-to-open Unity scenes covering plane placement, image tracking, vehicle placement, and mini-game prototypes. |
| `Assets/Pipelines`, `Settings`, `TextMesh Pro`, `XR`, `XRI` | Supporting render pipeline assets, volume profiles, fonts, and XR Interaction Toolkit configurations. |
| `Build` | Auto-generated Burst debug data from previous builds (safe to delete if storage is a concern). |

## Requirements
- Unity Hub with **Unity 6000.2.6f2** plus Android (or iOS) Build Support, OpenJDK, and SDK/NDK tools.
- ARCore- or ARKit-capable device running Android 10+ or iOS 15+.
- USB debugging enabled (Android) or a valid Apple provisioning profile (iOS).
- Optional: TextMeshPro Essentials imported (already configured in this project) for the diagnostics overlay.

## Quick Start
1. Clone or download the repository into a local folder.
2. Open Unity Hub → **Open** → select `Plane Detection/Plane Detection.sln` or the folder root.
3. When prompted, install Unity 6000.2.6f2; allow the Editor to update the project.
4. In **Build Settings**, switch the platform to **Android** (or **iOS**) and click **Apply**.
5. Open `Assets/Scenes/ScenePrime.unity` to explore the standard plane-detection flow. Use Play Mode with a webcam/AR simulation or deploy to device for accurate tracking.
6. Connect a device, press **Build & Run**, and test the placement modes, menu toggles, and debug overlays directly on hardware.

## Building for Device
- **Android**: 
	- Enable **ARM64** architecture with IL2CPP and strip unused managed code for smaller builds.
	- Under **Project Settings → XR Plug-in Management**, enable **ARCore** for Android and ensure required permissions (camera) are checked.
	- If you use feature-point placement, keep depth and point-cloud subsystems enabled in **AR Session Origin**.
- **iOS**:
	- Enable **ARKit** plus **Requires ARKit support** in **XR Plug-in Management**.
	- In Xcode, set the provisioning profile and add a usage description for camera access in `Info.plist`.

## Key Scripts & How to Extend
| Script | Description | Extension Tips |
| --- | --- | --- |
| `Scripts/ARModeController.cs` | Centralizes UI toggles for plane detection, plane deletion, object removal, and placement gating. | Ensure your placed prefabs use the `PlacedObjects` layer so the removal raycasts work. Add more modes by mirroring the existing toggle pattern. |
| `Scripts/TapToPlaceObject.cs` | Minimal tap-to-place behavior that raycasts against plane polygons and instantiates a prefab at the hit pose. | Swap the prefab at runtime or pool objects instead of instantiating per tap for performance. |
| `Scripts/PlaceDragInScene.cs` | Supports dragging new content out of `ARRaycastManager.raycastPrefab`, useful for painting anchors along surfaces. | Adjust the `SetIsPlacingToFalseWithDelay` coroutine to control gesture responsiveness. |
| `Scripts/cloudpointToPlacePrefab.cs` | Places or repositions a prefab on feature points with Enhanced Touch + mouse fallback. | Pair with depth-based visualization by switching `TrackableType.FeaturePoint` to `TrackableType.FeaturePoint | TrackableType.Depth`. |
| `Scripts/ImageTrackingManager.cs` | Keeps a prefab dictionary synced with the tracked-image library and updates pose each frame. | Populate `prefabsToSpawn` in the Inspector in the same order as the XR Reference Image Library. Add pooling to avoid Instantiate/Destroy when tracking toggles. |
| `Scripts/PerformanceMonitor.cs` | Streams memory and GPU timings into a TextMeshProUGUI label via ProfilerRecorder. | Expose more metrics (CPU time, draw calls) by adding additional recorders in `OnEnable`. |

## Working with Scenes
- **ScenePrime** – default showcase scene combining plane toggles, menu controls, and placement scripts.
- **ImageTrackingScene** – configured with a reference image library to test `ImageTrackingManager` and prefab spawning.
- **PointCloudScene** – use to place a flying saucer prefab to interact with point cloud feature points.
- **carScene** – uses the car placement scripts to anchor a vehicle prefab on detected planes.
- **BallShootingGameScene**, **Scene_Dragon**, **SceneTest** – experimental playgrounds for physics interactions and alternative assets; ideal for extending the template.

## Performance Overlay
1. Drop the **PerformanceMonitor** prefab (or add the script to an empty GameObject) inside your scene Canvas.
2. Assign a TextMeshProUGUI element to `statsText`.
3. Press Play or deploy; memory usage (MB) and GPU frame time (ms) will stream in real time.
4. Uncomment the draw-call line inside `Update()` if you want render-thread statistics too.

## Troubleshooting
- **No planes appear in Play Mode** – Verify your XR Origin includes an `ARPlaneManager` with the plane prefab assigned and that plane detection is toggled on in `ARModeController`.
- **Layer-based deletion fails** – Confirm you created `ARPlanes` and `PlacedObjects` layers and assigned them to plane prefabs and spawned objects respectively.
- **Image targets never track** – Check that the active XR Reference Image Library is linked to `ARTrackedImageManager` and that the physical print size matches the library metadata.
- **Build errors about missing TMP assets** – Reimport TextMeshPro Essentials via `Window → TextMeshPro → Import TMP Essential Resources`.

## License
Distributed under the [MIT License](LICENSE). Review the license text before shipping commercial builds or redistributing modified assets.
