using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // ���� URP ֧��
using UnityEngine.UI; // ���� UnityEngine.UI������ Slider ������ UI ���

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;       // ������ͣ�˵��� Canvas
    public Slider volumeSlider;          // �������ֿ��ƻ���
    public Volume globalVolume;          // ����ȫ�� Volume
    private Bloom bloom;                 // ����ģ��
    private ChromaticAberration chromaticAberration; // ɫ��ģ��
    private DepthOfField depthOfField;   // ����ģ��
    private bool isPaused = false;       // ��Ϸ�Ƿ���ͣ�ı�־
    public PhotoModeController photoModeController; // ���� PhotoModeController

    void Start()
    {
        // ��ʼ��������ͣ�˵�
        pauseMenuUI.SetActive(false);

        // ��ȡ Bloom�����⣩ģ��
        if (globalVolume.profile.TryGet(out Bloom b))
        {
            bloom = b;
            bloom.intensity.overrideState = true; // ����ǿ�ȿ���
            //bloom.intensity.value = 1f;          // Ĭ��ǿ��
        }

        // ��ȡ Chromatic Aberration��ɫ�ģ��
        if (globalVolume.profile.TryGet(out ChromaticAberration ca))
        {
            chromaticAberration = ca;
            chromaticAberration.intensity.overrideState = true; // ����ǿ�ȿ���
            //chromaticAberration.intensity.value = 0f;           // Ĭ��ǿ��
        }

        // ��ȡ Depth of Field�����ģ��
        if (globalVolume.profile.TryGet(out DepthOfField dof))
        {
            depthOfField = dof;
            depthOfField.focusDistance.overrideState = true; // ���ý������
            //depthOfField.focusDistance.value = 10f;          // Ĭ�Ͻ���
        }
    }

    void Update()
    {
        // ���� ESC ��
        if (Input.GetKeyDown(KeyCode.Escape)&& photoModeController.IsPhotoMode())
        {
            if (isPaused)
            {
                ResumeGame(); // �ָ���Ϸ
            }
            else
            {
                PauseGame(); // ��ͣ��Ϸ
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);        // ���ز˵�
        Time.timeScale = 1f;                // �ָ�ʱ������
        Cursor.lockState = CursorLockMode.Locked; // �������
        Cursor.visible = false;             // �������
        isPaused = false;

        // �ָ�Ĭ��Ч��
        StopAllCoroutines();
        StartCoroutine(AnimateBloomIntensity(8f, 1f));          // ����ǿ�ȴ� 8 �ָ��� 1
        StartCoroutine(AnimateChromaticAberration(1f, 0f));     // ɫ��ǿ�ȴ� 1 �ָ��� 0
        StartCoroutine(AnimateDepthOfField(0.1f, 10f));         // ����� 0.1 �ָ��� 10
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);         // ��ʾ�˵�
        Time.timeScale = 0f;                 // ��ͣʱ������
        Cursor.lockState = CursorLockMode.None; // �������
        Cursor.visible = true;               // ��ʾ���
        isPaused = true;

        // ��̬����Ч��
        StopAllCoroutines();
        StartCoroutine(AnimateBloomIntensity(1f, 8f));          // ����ǿ�ȴ� 1 ���ӵ� 8
        StartCoroutine(AnimateChromaticAberration(0f, 1f));     // ɫ��ǿ�ȴ� 0 ���ӵ� 1
        StartCoroutine(AnimateDepthOfField(10f, 0.1f));         // ����� 10 ��С�� 0.1
    }

    public void QuitGame()
    {
        // �������˵������˳���Ϸ
        // SceneManager.LoadScene("MainMenu");
        Application.Quit(); // �˳���Ϸ�����ڴ������Ч��
    }

    public void SetVolume(float volume)
    {
        // ������������
        AudioListener.volume = volume;
    }

    // ��̬��������ǿ��
    private System.Collections.IEnumerator AnimateBloomIntensity(float from, float to)
    {
        float duration = 1f; // ����ʱ��
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

    // ��̬����ɫ��ǿ��
    private System.Collections.IEnumerator AnimateChromaticAberration(float from, float to)
    {
        float duration = 1f; // ����ʱ��
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

    // ��̬���������
    private System.Collections.IEnumerator AnimateDepthOfField(float from, float to)
    {
        float duration = 1f; // ����ʱ��
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
