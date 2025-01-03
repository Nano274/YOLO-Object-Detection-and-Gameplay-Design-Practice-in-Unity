# Photo Mode Module

## Features

### 1. Capture and Save Photos
- Players can switch to Photo Mode using the **O key**.
- Press **V key** to capture a screenshot. The captured image is saved locally in the `CapturedPhotos` directory.

### 2. Analyze Captured Photos
- After taking a photo, press **C key** to analyze it using the YOLO object detection system.
- Analysis results are saved in the `AnalyzedPhotos` directory and displayed on the UI.

### 3. Real-Time Analysis Mode
- Players can activate **Real-Time Analysis Mode** using the **P key** to analyze surroundings dynamically.

### 4. Photo Display and Management
- Captured photos are displayed in a dedicated UI container, allowing players to view or clear photos.
- The last captured photo is always accessible for further actions like analysis or replacement.

---

## Key Components

### 1. **PhotoModeController**
Handles the overall logic for entering and exiting Photo Mode, capturing photos, and initiating analysis.

#### Key Parameters:
- **Photo Cameras:** `mainCamera` and `photoCamera` to switch views.
- **Directories:** `CapturedPhotos` and `AnalyzedPhotos` for storing images.
- **UI Feedback:** Buttons and messages for player interaction.

#### Main Functions:
- **EnterPhotoMode()**: Activates Photo Mode by enabling the photo camera and disabling gameplay controls.
- **ExitPhotoMode()**: Returns to normal gameplay by re-enabling the main camera and controls.
- **SavePhoto()**: Captures the current camera view and saves it as a PNG file.
- **HandlePhotoModeToggle()**: Manages the toggling between gameplay and Photo Mode.

---

### 2. **PhotoDisplayManager**
Manages photo display in the UI and handles photo-related actions.

#### Key Parameters:
- **`photoContainer`:** The UI container for displaying photos.
- **`photoPrefab`:** The prefab used to display photos.

#### Main Functions:
- **CaptureAndDisplay():** Captures a screenshot and displays it in the UI.
- **DisplayPhoto():** Updates the last displayed photo with a new texture.
- **ClearAllPhotos():** Removes all photos from the UI container.

---

## Controls

| Key       | Action                                   |
|-----------|-----------------------------------------|
| **O**     | Enter or exit Photo Mode                |
| **V**     | Capture a photo                        |
| **C**     | Analyze the last captured photo         |
| **P**     | Toggle Real-Time Analysis Mode          |
| **ESC**   | Toggle normal mode UI visibility        |

---

## Setup Instructions

1. **Scene Setup:**
   - Add the `PhotoModeController` and `PhotoDisplayManager` scripts to appropriate GameObjects.
   - Assign the `mainCamera`, `photoCamera`, and required UI elements in the Inspector.

2. **Directories:**
   - Ensure `CapturedPhotos` and `AnalyzedPhotos` directories exist in the project root or they will be created automatically.

3. **UI Setup:**
   - Design a UI layout with placeholders for photos and buttons.
   - Use the `photoPrefab` to display captured photos in the `photoContainer`.

4. **Integration with YOLO:**
   - Link the `RunYOLOv8` script to process and analyze photos.

---

## Notes
- Photos are captured at a resolution of **1920x1080** by default.
- Ensure the `photoPrefab` has a `RawImage` component for displaying photos.
- Customize the photo resolution and directory paths in the script if necessary.

---

## Example Workflow
1. Press **O** to enter Photo Mode.
2. Use **V** to capture a photo.
3. Press **C** to analyze the captured photo.
4. Press **P** for Real-Time Analysis Mode or **ESC** to return to gameplay.
