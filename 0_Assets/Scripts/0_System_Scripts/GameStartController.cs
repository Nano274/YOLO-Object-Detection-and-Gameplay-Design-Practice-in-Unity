using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // ���� URP ֧��
using UnityEngine.UI; // ���� UnityEngine.UI������ Slider ������ UI ���
using TMPro;

public class GameStartManager : MonoBehaviour
{
    public PhotoModeController photoModeController;
    public Button startButton;   // ��ʼ��ť
    public TMP_Text startText;
    public Volume globalVolume; // ȫ��ģ��Ч��
    private Bloom bloom;                 // ����ģ��
    private ChromaticAberration chromaticAberration; // ɫ��ģ��
    private DepthOfField depthOfField;   // ����ģ��
    private bool isStart = true; // �Ƿ���չʾ��Ƭ
    //private bool isInputBlocked = true; // �Ƿ���������

    void Start()
    {
        // ��ͣ��Ϸʱ��
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None; // �������
        Cursor.visible = true;               // ��ʾ���

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

        // ������ʼ��ť����¼�
        startButton.onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        // �ָ���Ϸʱ��
        Time.timeScale = 1;
        //isInputBlocked = false;
        photoModeController.TogglePhotoModeUI(!isStart);
        Cursor.lockState = CursorLockMode.Locked; // �������
        Cursor.visible = false;             // �������

        StopAllCoroutines();
        StartCoroutine(AnimateBloomIntensity(8f, 1f));          // ����ǿ�ȴ� 8 �ָ��� 1
        StartCoroutine(AnimateChromaticAberration(1f, 0f));     // ɫ��ǿ�ȴ� 1 �ָ��� 0
        StartCoroutine(AnimateDepthOfField(0.1f, 10f));         // ����� 0.1 �ָ��� 10

        // ���ذ�ť
        startButton.gameObject.SetActive(false);
        startText.gameObject.SetActive(false);
    }

    void Update()
    {

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
