using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // 引入 URP 支持
using UnityEngine.UI; // 引入 UnityEngine.UI，包含 Slider 和其他 UI 组件
using TMPro;

public class GameStartManager : MonoBehaviour
{
    public PhotoModeController photoModeController;
    public Button startButton;   // 开始按钮
    public TMP_Text startText;
    public Volume globalVolume; // 全局模糊效果
    private Bloom bloom;                 // 泛光模块
    private ChromaticAberration chromaticAberration; // 色差模块
    private DepthOfField depthOfField;   // 景深模块
    private bool isStart = true; // 是否在展示照片
    //private bool isInputBlocked = true; // 是否屏蔽输入

    void Start()
    {
        // 暂停游戏时间
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None; // 解锁鼠标
        Cursor.visible = true;               // 显示鼠标

        // 获取 Bloom（泛光）模块
        if (globalVolume.profile.TryGet(out Bloom b))
        {
            bloom = b;
            bloom.intensity.overrideState = true; // 启用强度控制
            //bloom.intensity.value = 1f;          // 默认强度
        }

        // 获取 Chromatic Aberration（色差）模块
        if (globalVolume.profile.TryGet(out ChromaticAberration ca))
        {
            chromaticAberration = ca;
            chromaticAberration.intensity.overrideState = true; // 启用强度控制
            //chromaticAberration.intensity.value = 0f;           // 默认强度
        }

        // 获取 Depth of Field（景深）模块
        if (globalVolume.profile.TryGet(out DepthOfField dof))
        {
            depthOfField = dof;
            depthOfField.focusDistance.overrideState = true; // 启用焦距控制
            //depthOfField.focusDistance.value = 10f;          // 默认焦距
        }

        // 监听开始按钮点击事件
        startButton.onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        // 恢复游戏时间
        Time.timeScale = 1;
        //isInputBlocked = false;
        photoModeController.TogglePhotoModeUI(!isStart);
        Cursor.lockState = CursorLockMode.Locked; // 锁定鼠标
        Cursor.visible = false;             // 隐藏鼠标

        StopAllCoroutines();
        StartCoroutine(AnimateBloomIntensity(8f, 1f));          // 泛光强度从 8 恢复到 1
        StartCoroutine(AnimateChromaticAberration(1f, 0f));     // 色差强度从 1 恢复到 0
        StartCoroutine(AnimateDepthOfField(0.1f, 10f));         // 焦距从 0.1 恢复到 10

        // 隐藏按钮
        startButton.gameObject.SetActive(false);
        startText.gameObject.SetActive(false);
    }

    void Update()
    {

    }

    // 动态调整泛光强度
    private System.Collections.IEnumerator AnimateBloomIntensity(float from, float to)
    {
        float duration = 1f; // 动画时长
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (bloom != null)
                bloom.intensity.value = Mathf.Lerp(from, to, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        if (bloom != null)
            bloom.intensity.value = to;
    }

    // 动态调整色差强度
    private System.Collections.IEnumerator AnimateChromaticAberration(float from, float to)
    {
        float duration = 1f; // 动画时长
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (chromaticAberration != null)
                chromaticAberration.intensity.value = Mathf.Lerp(from, to, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        if (chromaticAberration != null)
            chromaticAberration.intensity.value = to;
    }

    // 动态调整景深焦距
    private System.Collections.IEnumerator AnimateDepthOfField(float from, float to)
    {
        float duration = 1f; // 动画时长
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (depthOfField != null)
                depthOfField.focusDistance.value = Mathf.Lerp(from, to, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        if (depthOfField != null)
            depthOfField.focusDistance.value = to;
    }
}
