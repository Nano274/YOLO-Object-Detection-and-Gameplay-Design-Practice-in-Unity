using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // 引入 URP 支持
using UnityEngine.UI; // 引入 UnityEngine.UI，包含 Slider 和其他 UI 组件

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;       // 引用暂停菜单的 Canvas
    public Slider volumeSlider;          // 引用音乐控制滑条
    public Volume globalVolume;          // 引用全局 Volume
    private Bloom bloom;                 // 泛光模块
    private ChromaticAberration chromaticAberration; // 色差模块
    private DepthOfField depthOfField;   // 景深模块
    private bool isPaused = false;       // 游戏是否暂停的标志
    public PhotoModeController photoModeController; // 引用 PhotoModeController

    void Start()
    {
        // 初始化隐藏暂停菜单
        pauseMenuUI.SetActive(false);

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
    }

    void Update()
    {
        // 监听 ESC 键
        if (Input.GetKeyDown(KeyCode.Escape)&& photoModeController.IsPhotoMode())
        {
            if (isPaused)
            {
                ResumeGame(); // 恢复游戏
            }
            else
            {
                PauseGame(); // 暂停游戏
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);        // 隐藏菜单
        Time.timeScale = 1f;                // 恢复时间流动
        Cursor.lockState = CursorLockMode.Locked; // 锁定鼠标
        Cursor.visible = false;             // 隐藏鼠标
        isPaused = false;

        // 恢复默认效果
        StopAllCoroutines();
        StartCoroutine(AnimateBloomIntensity(8f, 1f));          // 泛光强度从 8 恢复到 1
        StartCoroutine(AnimateChromaticAberration(1f, 0f));     // 色差强度从 1 恢复到 0
        StartCoroutine(AnimateDepthOfField(0.1f, 10f));         // 焦距从 0.1 恢复到 10
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);         // 显示菜单
        Time.timeScale = 0f;                 // 暂停时间流动
        Cursor.lockState = CursorLockMode.None; // 解锁鼠标
        Cursor.visible = true;               // 显示鼠标
        isPaused = true;

        // 动态设置效果
        StopAllCoroutines();
        StartCoroutine(AnimateBloomIntensity(1f, 8f));          // 泛光强度从 1 增加到 8
        StartCoroutine(AnimateChromaticAberration(0f, 1f));     // 色差强度从 0 增加到 1
        StartCoroutine(AnimateDepthOfField(10f, 0.1f));         // 焦距从 10 减小到 0.1
    }

    public void QuitGame()
    {
        // 返回主菜单或者退出游戏
        // SceneManager.LoadScene("MainMenu");
        Application.Quit(); // 退出游戏（仅在打包后有效）
    }

    public void SetVolume(float volume)
    {
        // 调整音乐音量
        AudioListener.volume = volume;
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
