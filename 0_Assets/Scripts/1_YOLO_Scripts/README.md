# YOLO Model Module

## Features
1. **Model Integration**
   - Loads and executes a YOLOv8 model (`.sentis`) using the Unity Sentis framework.
   - Handles object detection with bounding boxes and class labels.

2. **Photo and Real-Time Analysis**
   - Analyzes captured photos or camera frames dynamically.
   - Displays bounding boxes with labels and confidence scores.

3. **Bounding Box Management**
   - Supports rendering and managing bounding boxes dynamically.
   - Uses a pool system to optimize rendering performance.

4. **Flexible Backend**
   - Supports GPU or CPU backends for model inference.

---

## Key Components

### 1. **RunYOLOv8**
This script is responsible for running the YOLO model on captured images or textures.

#### Key Features:
- **Model Loading:**
  - Loads the YOLOv8 model (`asset`) and class labels (`labelsAsset`).
- **Prediction:**
  - Processes input images and outputs bounding boxes, labels, and confidence scores.
- **Bounding Box Rendering:**
  - Dynamically draws bounding boxes with labels on detected objects.
- **Photo Analysis:**
  - Captures and analyzes saved photos, saving the results to the `AnalyzedPhotos` directory.

#### Functions:
- **`Link(string photoPath)`**: Analyzes a saved photo and renders the result.
- **`Predict(Texture2D inputImage)`**: Runs YOLO inference on the given image.
- **`DrawBox(BoundingBox box, int id, float fontSize)`**: Renders a bounding box and its label.

---

### 2. **Analysis**
This script performs real-time object detection using a camera feed.

#### Key Features:
- **Model Initialization:**
  - Loads the YOLO model and prepares it for real-time inference.
- **Real-Time Detection:**
  - Processes frames captured by a `Camera` component and displays bounding boxes on objects.
- **Toggle Detection:**
  - Easily start or stop the detection process.

#### Functions:
- **`Link()`**: Starts real-time object detection.
- **`StopDetection()`**: Stops real-time object detection.
- **`ExecuteML()`**: Processes each frame and renders detection results.

---

## Usage

### Setup Instructions
1. **Scene Configuration:**
   - Add the `RunYOLOv8` and `Analysis` scripts to relevant GameObjects in your Unity scene.
   - Assign the `Camera`, `RenderTexture`, and UI components (like `RawImage`) in the Inspector.

2. **Model and Labels:**
   - Place the YOLOv8 model file (e.g., `yolov8n.sentis`) and class label file (e.g., `classes.txt`) in the Unity project.
   - Link these files to the `asset` and `labelsAsset` fields in the Inspector.

3. **UI Setup:**
   - Design a UI layout with a `RawImage` for displaying results.
   - Use a `Sprite` and `Font` for rendering bounding boxes and labels.

4. **Directories:**
   - Create `AnalyzedPhotos` in the project directory for saving analyzed photo results.

---

### Controls
| Key       | Action                                |
|-----------|--------------------------------------|
| **Link**  | Analyze a saved photo using YOLO     |
| **ExecuteML** | Perform real-time object detection |

---

## Notes
- Ensure the YOLO model file matches the expected input dimensions (e.g., 640x640 for YOLOv8).
- For optimal performance, use a GPU backend if available.
- Adjust thresholds (`iouThreshold` and `scoreThreshold`) in the Inspector to fine-tune detection sensitivity.

---

## Example Workflow
1. Load the YOLO model (`yolov8n.sentis`) and class labels (`classes.txt`).
2. Start real-time detection using the `Analysis` script or analyze photos with `RunYOLOv8`.
3. View bounding boxes and labels rendered dynamically in the Unity scene.
4. Save analyzed photos for later review.

For further assistance or customization, feel free to open an issue in the repository.

