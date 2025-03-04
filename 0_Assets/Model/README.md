# Model Folder

## Description
The `Model` folder contains the following key files for YOLO-based object detection:

1. **`best.onnx`**  
   - This is the pre-trained YOLO model file, fine-tuned for detecting specific animal classes in the game environment.

2. **`classes_Animal_new.txt`**  
   - A text file listing the animal categories detected by the model.

## Usage Instructions
1. Drag and drop the `best.onnx` model file and the `classes_Animal_new.txt` class file into the designated script in Unity to enable object detection functionality.
2. You can use these files directly or train and fine-tune your own model to replace them for customized detection.

<div align="center">
  <img src="https://raw.githubusercontent.com/Nano274/YOLO-Object-Detection-and-Gameplay-Design-Practice-in-Unity/master//Pictures/Setting1.png" alt="Photo" width="405" height="230">
</div>

## Customization
- To fine-tune the model for new classes:
  1. Prepare a dataset and use a YOLO training framework (such as YOLOv5 or YOLOv8).
  2. Export the trained model as an ONNX file.
  3. Update the `classes_Animal_new.txt` file to reflect the new categories.
- Replace the old `best.onnx` and `classes_Animal_new.txt` files with your updated versions.

---

