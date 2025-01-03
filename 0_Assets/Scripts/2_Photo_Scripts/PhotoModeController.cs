using UnityEngine;
using UnityEngine.UI;
using Unity.Sentis;
using System.Collections;
using System.IO;

public class PhotoModeController : MonoBehaviour
{
    public RunYOLOv8 runYOLOv8;//链接YOLOv8的脚本
    public Analysis analysis; //链接Analyze YOLOv8的脚本
    public PhotoDisplayManager photoDisplayManager; // 照片管理脚本

    public Camera mainCamera;              // 主相机
    public Camera photoCamera;             // 照片模式相机
    public GameObject controller;          // 玩家控制器
    public GameObject photoTakenMessage;   // 动态提示UI
    public GameObject analysisTakenMessage;   // 动态提示UI
    public GameObject analysingMessage;   // 动态提示UI

    public RawImage wImage;
    public RawImage aImage;
    public RawImage sImage;
    public RawImage dImage;
    public RawImage pImage;
    public RawImage vImage;
    public RawImage cImage;
    public Button vButton;
    public Button cButton;

    public RawImage rawImage;              // 照片模式下的 RawImage
    public Button analysingButton;
    public Button[] nomalModeButtons;
    public Button[] photoModeButtons;      // 照片模式下的其他按钮
    //public Button[] displayingButtons;      // 照片模式下的其他按钮

    public RawImage[] nomalModeImages;
    public RawImage[] photoModeImages;
    //public RawImage[] displayingImage;

    private bool isVisible = true;       // 当前是否显示 UI

    private bool isPhotoMode = false;      // 是否为照片模式
    private bool isPhotoDisplayed = false; // 是否在展示照片

    private bool isAnalysisMode = false;    // 是否在分析现场
    private string currentPhotoPath;       // 当前显示的照片路径

    private string photoDirectory = "CapturedPhotos";
    private string analyzedDirectory = "AnalyzedPhotos";

    void Start()
    {
        photoTakenMessage.SetActive(false); // 默认隐藏拍照提示
        analysisTakenMessage.SetActive(false); // 默认隐藏分析提示
        analysingMessage.SetActive(false); // 默认隐藏分析提示

        //TogglePhotoModeUI(false); // 初始状态下隐藏照片模式 UI

        /*if (displayingButtons != null && displayingButtons.Length > 0)
        {
            foreach (Button btn in displayingButtons)
            {
                btn.gameObject.SetActive(true);//显示esc按钮
            }
            foreach (RawImage img in displayingImage)
            {
                img.gameObject.SetActive(true);//显示WASD+ESC按钮
            }
        }*/

        // 确保保存照片的文件夹存在
        if (!Directory.Exists(photoDirectory))
        {
            Directory.CreateDirectory(photoDirectory);
        }
        if (!Directory.Exists(analyzedDirectory))
        {
            Directory.CreateDirectory(analyzedDirectory);
        }
    }

    void Update()
    {
        HandleContinuousKey(KeyCode.W, wImage);
        HandleContinuousKey(KeyCode.A, aImage);
        HandleContinuousKey(KeyCode.S, sImage);
        HandleContinuousKey(KeyCode.D, dImage);

        // 监听 ESC 键
        if (Input.GetKeyDown(KeyCode.Escape) &&!isPhotoMode)
        {
            ToggleNormalModeUI();
        }


        if (Input.GetMouseButtonDown(1)) // 按 V 键拍照
        {
            controller.SetActive(false);
            currentPhotoPath = SavePhoto(photoCamera);
        }

        if (isPhotoMode && Input.GetKeyDown(KeyCode.V)&& !isAnalysisMode) // 按 V 键拍照
        {
            StartCoroutine(FlashImage(vImage)); 
            StartCoroutine(FlashButton(vButton));
            currentPhotoPath = SavePhoto(photoCamera);
            StartCoroutine(ShowPhotoTakenMessage(photoTakenMessage));

            // 加载拍摄的照片
            Texture2D photo = LoadPhoto(currentPhotoPath);
            photoDisplayManager.DisplayPhoto(photo);
            isPhotoDisplayed = true; // 更新状态
        }

        if (isPhotoMode && Input.GetKeyDown(KeyCode.C) && !isAnalysisMode) // 按 C 键分析照片
        {
            StartCoroutine(FlashImage(cImage));
            StartCoroutine(FlashButton(cButton));
            if (!string.IsNullOrEmpty(currentPhotoPath) && File.Exists(currentPhotoPath))
            {
                if (runYOLOv8 == null)
                {
                    Debug.LogError("runYOLOv8 is not assigned in the inspector.");
                    return;
                }
                string analyzedPhotoPath = runYOLOv8.Link(currentPhotoPath); // 给图像的路径

                StartCoroutine(ShowPhotoTakenMessage(analysisTakenMessage));

                Texture2D analyzedPhoto = LoadPhoto(analyzedPhotoPath);    // 将新图像展示出来
                photoDisplayManager.DisplayPhoto(analyzedPhoto);
                isPhotoDisplayed = true; // 更新状态
            }
            else
            {
                Debug.LogWarning("No photo to analyze.");
            }
        }

        if (!isPhotoMode && Input.GetKeyDown(KeyCode.P)) // 按 P 键进入分析模式
        {
            StartCoroutine(FlashImage(pImage));
            if (!isAnalysisMode)
            {
                analysingButton.gameObject.SetActive(true);
                analysingMessage.SetActive(true);
                analysis.Link(); // 进入分析模式
                Debug.Log("Analysis mode started.");
                isAnalysisMode = true; // 更新状态
            }
            else
            {
                analysingButton.gameObject.SetActive(false);
                analysingMessage.SetActive(false);
                analysis.StopDetection(); // 退出分析模式
                Debug.Log("Analysis mode stopped.");
                isAnalysisMode = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.O) && !isAnalysisMode) // 按 O 键切换模式
        {
            HandlePhotoModeToggle();
        }
    }

    private void HandlePhotoModeToggle()
    {
        if (isPhotoDisplayed) // 如果正在展示照片
        {
            photoDisplayManager.ClearAllPhotos(); // 清除照片展示
            isPhotoDisplayed = false; // 更新状态
            Debug.Log("Cleared displayed photo. Returning to photo mode.");
        }
        else if (!isPhotoMode) // 如果不是照片模式
        {
            EnterPhotoMode();
        }
        else // 如果是照片模式且没有照片展示
        {
            ExitPhotoMode();
        }
    }

    private void EnterPhotoMode()
    {
        isPhotoMode = true;
        mainCamera.enabled = false;
        photoCamera.enabled = true;
        controller.SetActive(false);
        //Control
        TogglePhotoModeUI(true);
        Debug.Log("Entered photo mode.");
    }

    private void ExitPhotoMode()
    {
        isPhotoMode = false;
        mainCamera.enabled = true;
        photoCamera.enabled = false;
        controller.SetActive(true);
        TogglePhotoModeUI(false);
        photoDisplayManager.ClearAllPhotos();
        currentPhotoPath = null; // 清除当前照片路径
        Debug.Log("Exited photo mode.");
    }

    private IEnumerator ShowPhotoTakenMessage(GameObject message)
    {
        if (message != null)
        {
            message.SetActive(true);
            yield return new WaitForSeconds(1); // 显示 1 秒
            message.SetActive(false);
        }
    }

    // 切换照片模式 UI 的显示和隐藏
    public void TogglePhotoModeUI(bool show)
    {
        if (rawImage != null)
        {
            rawImage.gameObject.SetActive(show);
        }

        // 隐藏初始按钮
        if (nomalModeButtons != null && nomalModeButtons.Length > 0 &&
            nomalModeImages != null && nomalModeImages.Length > 0)
        {
            for (int i = 0; i < nomalModeButtons.Length; i++)
            {
                Button btn1 = nomalModeButtons[i];
                btn1.gameObject.SetActive(!show);
            }
            for (int i = 0; i < nomalModeImages.Length; i++)
            {
                RawImage img1 = nomalModeImages[i];
                img1.gameObject.SetActive(!show); // RawImage 使用 gameObject.SetActive
            }
        }
        if (photoModeButtons != null && photoModeButtons.Length > 0 &&
            photoModeImages != null && photoModeImages.Length > 0)
        {
            for (int i = 0; i < photoModeButtons.Length; i++)
            {
                Button btn1 = photoModeButtons[i];
                btn1.gameObject.SetActive(show);
            }
            for (int i = 0; i < photoModeImages.Length; i++)
            {
                RawImage img1 = photoModeImages[i];
                img1.gameObject.SetActive(show); // RawImage 使用 gameObject.SetActive
            }
        }

    }

    private string SavePhoto(Camera camera)
    {
        RenderTexture renderTexture = new RenderTexture(1920, 1080, 24);
        camera.targetTexture = renderTexture;
        Texture2D screenshot = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
        screenshot.Apply();
        camera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        string photoPath = Path.Combine(photoDirectory, $"Photo_{System.DateTime.Now:yyyyMMdd_HHmmss}.png");
        File.WriteAllBytes(photoPath, screenshot.EncodeToPNG());
        Debug.Log($"Photo saved at: {photoPath}");
        return photoPath;
    }

    private Texture2D LoadPhoto(string photoPath)
    {
        if (!File.Exists(photoPath))
        {
            Debug.LogError("Photo file does not exist: " + photoPath);
            return null;
        }

        byte[] fileData = File.ReadAllBytes(photoPath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        return texture;
    }

    private void HandleContinuousKey(KeyCode key, RawImage image)
    {
        if (Input.GetKey(key))
        {
            image.gameObject.SetActive(true);
        }
        else
        {
            image.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 短暂闪烁图像
    /// </summary>
    private IEnumerator FlashImage(RawImage image)
    {
        image.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f); // 显示 0.1 秒
        image.gameObject.SetActive(false);
    }

    /// <summary>
    /// 短暂闪烁图像
    /// </summary>
    private IEnumerator FlashButton(Button btn)
    {
        btn.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f); // 显示 0.1 秒
        btn.gameObject.SetActive(false);
    }

    public bool IsPhotoMode()
    {
        return !isPhotoMode;
    }

    private void ToggleNormalModeUI()
    {
        // 切换可见性
        isVisible = !isVisible;

        if (!isVisible){
            if (nomalModeButtons != null && nomalModeButtons.Length > 0 &&
           nomalModeImages != null && nomalModeImages.Length > 0)
            {
                for (int i = 0; i < nomalModeButtons.Length; i++)
                {
                    Button btn1 = nomalModeButtons[i];
                    btn1.gameObject.SetActive(false);
                }
                for (int i = 0; i < nomalModeImages.Length; i++)
                {
                    RawImage img1 = nomalModeImages[i];
                    img1.gameObject.SetActive(false); // RawImage 使用 gameObject.SetActive
                }
            }
        }else if (isVisible)
        {
            if (nomalModeButtons != null && nomalModeButtons.Length > 0 &&
           nomalModeImages != null && nomalModeImages.Length > 0)
            {
                for (int i = 0; i < nomalModeButtons.Length; i++)
                {
                    Button btn1 = nomalModeButtons[i];
                    btn1.gameObject.SetActive(true);
                }
                for (int i = 0; i < nomalModeImages.Length; i++)
                {
                    RawImage img1 = nomalModeImages[i];
                    img1.gameObject.SetActive(true); // RawImage 使用 gameObject.SetActive
                }
            }
        }

    }
}
