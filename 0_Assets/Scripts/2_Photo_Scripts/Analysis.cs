using System.Collections;
using System.Collections.Generic;
using Unity.Sentis;
using UnityEngine;
using UnityEngine.UI;
using Unity.Sentis.Layers;
using System.IO;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.Video;
using Lays = Unity.Sentis.Layers;
using FF = Unity.Sentis.Functional;

public class Analysis : MonoBehaviour
{
    // ��ǥ���v�����O��
    // ��yolov8n.sentis���ե��������
    public ModelAsset asset;
    // classes.txt����
    public TextAsset labelsAsset;
    private string[] labels;
    // !!!���饹�����������Ҫ��
    private const int numClasses = 80;
    // ���Ĥʥƥ󥽥루YOLO��I���ã�
    Tensor<float> centersToCorners;

    //����ǥ�g���O����
    private Worker engine;
    //camera�O��
    private bool isDetectionActive = false; // �����Ƿ����ü��ı�־
    public Transform objectBoxParent; // ��������
    public Camera analysisCamera; // Unity �����е� Camera
    public RenderTexture renderTexture; // ���ڽ��������ʵʱ����� RenderTexture

    // ��ʾ�v�B�O��
    // �Х���ǥ��󥰥ܥå������ץ饤�ȡ��ܥå����Υƥ�������
    public Sprite borderSprite;
    public Texture2D borderTexture;
    // ��٥�ե����
    public Font font;

    // ʹ�ä���Хå�����ɣ�GPU/CPU��
    const BackendType backend = BackendType.GPUCompute;

    //��ǥ���������� ��ǥ���������ߤ�(YOLO����������640x640)
    private const int imageWidth = 640;
    private const int imageHeight = 640;

    // NMS��IoU���v���륻�åƥ��󥰡��ʳ�������
    [SerializeField, Range(0, 1)] float iouThreshold = 0.5f;
    [SerializeField, Range(0, 1)] float scoreThreshold = 0.5f;
    // ���Ф餯�äϤʤ�
    // int maxOutputBoxes = 64;

    // �Х���ǥ��󥰥ܥå����ש`��
    List<GameObject> boxPool = new();
    // ��٥륫��`�Ǖ��������५��`������
    private Dictionary<string, Color> labelColorMap = new Dictionary<string, Color>();
    private System.Random random = new System.Random();
    // �Х���ǥ��󥰥ܥå����ǩ`��������
    public struct BoundingBox
    {
        public float centerX;
        public float centerY;
        public float width;
        public float height;
        public string label;
        public float score;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //��ǥ��`��
        LoadModel();

        //��٥��i���z��
        labels = labelsAsset.text.Split('\n');

        //���ץ饤���O��
        if (borderSprite == null)
        {
            borderSprite = Sprite.Create(borderTexture, new Rect(0, 0, borderTexture.width, borderTexture.height), new Vector2(borderTexture.width / 2, borderTexture.height / 2));
        }
    }

    void LoadModel()
    {
        // ��ǥ���`��
        Model model1 = ModelLoader.Load(asset);

        //YOLO��I���åƥ󥽥�
        centersToCorners = new Tensor<float>(new TensorShape(4, 4),
        new float[]
        {
                    1,      0,      1,      0,
                    0,      1,      0,      1,
                    -0.5f,  0,      0.5f,   0,
                    0,      -0.5f,  0,      0.5f
        });

        //Ӌ�㥰�������
        FunctionalGraph graph = new();

        //�ƥ󥽥���������������顸�Х�ƥ��󥰥֥å������ˡ����������������ߥ�������ID��
        //�����ˉ������NMS�����Х�ƥ��󥰥֥å�����ID���x�k��
        FunctionalTensor input = graph.AddInputs(model1)[0];
        FunctionalTensor output = Functional.Forward(model1, input)[0];
        FunctionalTensor boxCoords = output[0, 0..4, ..].Transpose(0, 1);        //shape=(8400,4)
        FunctionalTensor allScores = output[0, 4.., ..];                         //shape=(80,8400)
        FunctionalTensor scores = Functional.ReduceMax(allScores, 0);        //shape=(8400)
        FunctionalTensor classIDs = Functional.ArgMax(allScores, 0);                          //shape=(8400) 
        FunctionalTensor boxCorners = Functional.MatMul(boxCoords, Functional.Constant(centersToCorners));
        FunctionalTensor indices = Functional.NMS(boxCorners, scores, iouThreshold, scoreThreshold);           //shape=(N)
        FunctionalTensor indices2 = indices.Unsqueeze(-1).BroadcastTo(new int[] { 4 });//shape=(N,4)
        FunctionalTensor coords = Functional.Gather(boxCoords, 0, indices2);                  //shape=(N,4)
        FunctionalTensor labelIDs = Functional.Gather(classIDs, 0, indices);                  //shape=(N)
        FunctionalTensor selectedScores = Functional.Gather(scores, 0, indices);                  // shape=(N)
        //Ӌ�㥰��դ��¤��ʥ�ǥ�Ȥ�������
        Model runtimeModel = graph.Compile(coords, labelIDs, selectedScores);

        // �g�Х��󥸥�����
        engine = new(runtimeModel, BackendType.GPUCompute);
    }

    //!!!!!�����ϥ�����ץȤΌg�����
    public void Link()
    {
        Debug.Log("Starting real-time object detection...");
        isDetectionActive = true; // ���ü��
    }

    public void StopDetection()
    {
        ClearAnnotations();
        Debug.Log("Stopping real-time object detection...");
        isDetectionActive = false; // ֹͣ���
    }

    // Update is called once per frame
    void Update()
    {
        if (isDetectionActive) // ֻ�м������ʱ��ִ�� check
        {
            ExecuteML();
        }
    }

    public void ExecuteML()
    {
        ClearAnnotations();

        Texture2D inputTexture = CaptureCameraFrame();

        //�����ƥ󥽥�ˉ�Q
        Tensor<float> input = TextureConverter.ToTensor(inputTexture, imageWidth, imageHeight, 3);

        //�ƥ󥽥���ǥ������
        engine.Schedule(input);
        //��ǥ��g��(ͨ�� PeekOutput ������ȡģ�͵������������)
        //���� "output_0" �� "output_1" ��ģ����������ƣ���������
        var output = engine.PeekOutput("output_0") as Tensor<float>;
        var labelIDs = engine.PeekOutput("output_1") as Tensor<int>;
        var selectedScores = engine.PeekOutput("output_2") as Tensor<float>;

        //CPU�˥ǩ`���򥳥ԩ`(�ΤȤʤ������ΥХå�����Sentis��GPU����ƥ󥽥���i���z��ޤʤ�����)
        //һ���浹�ʲ��֤���˼��
        Tensor<float> cpuOutput = output.ReadbackAndClone();
        Tensor<int> cpulabelIDs = labelIDs.ReadbackAndClone();
        Tensor<float> cpuScores = selectedScores.ReadbackAndClone();

        //�ƥ󥽥����
        input?.Dispose();
        output?.Dispose();
        labelIDs.Dispose();
        selectedScores.Dispose();

        //����Υ�������ȡ��
        float displayWidth = 2560;
        float displayHeight = 1440;
        //����Υ����`���Ӌ��
        float scaleX = displayWidth / imageWidth;
        float scaleY = displayHeight / imageHeight;
        //�ʳ����줿�Х�ƥ��󥰥֥å���������ȡ��
        int boxesFound = cpuOutput.shape[0];

        //���Х���ǥ��󥰥ܥå����軭�������ϤޤǸ��ƤǤ��룩
        for (int n = 0; n < Mathf.Min(boxesFound, 200); n++)
        {
            var box = new BoundingBox
            {
                centerX = cpuOutput[n, 0] * scaleX - displayWidth / 2,
                centerY = cpuOutput[n, 1] * scaleY - displayHeight / 2,
                width = cpuOutput[n, 2] * scaleX,
                height = cpuOutput[n, 3] * scaleY,
                label = labels[cpulabelIDs[n]],
                score = (float)cpuScores[n],
            };
            DrawBox(box, n, displayHeight * 0.05f);
        }
        //���
        cpuOutput?.Dispose();
        cpulabelIDs?.Dispose();
        cpuScores?.Dispose();
    }

    public void DrawBox(BoundingBox box, int id, float fontSize)
    {
        // �ש`�뤫��ȡ��
        GameObject panel;
        if (id < boxPool.Count)
        {
            panel = boxPool[id];
            panel.SetActive(true);
        }
        else
        {
            panel = CreateNewBox(GetColorForLabel(box.label)); // ��ȡ�����ɫ
        }

        Image img = panel.GetComponent<Image>(); // ��ȡ Image ���
        Color color1 = GetColorForLabel(box.label);
        color1.a = 0.5f; // ����͸����
        img.color = color1; // ������ɫ
        // Set box position
        panel.transform.localPosition = new Vector3(box.centerX, -box.centerY);
        // Set box size
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(box.width, box.height);
        // Set label text
        var label = panel.GetComponentInChildren<Text>();
        label.color = GetColorForLabel(box.label);
        label.horizontalOverflow = HorizontalWrapMode.Overflow; // ˮƽ�����������
        label.verticalOverflow = VerticalWrapMode.Overflow;     // ��ֱ�����������
        label.text = $"{box.label} {box.score:F2}"; // ��ʽ���ַ�������ʾ��ǩ�����Ŷ�
        label.fontSize = (int)fontSize;
    }

    public GameObject CreateNewBox(Color color)
    {
        // Create the box and set image
        var panel = new GameObject("ObjectBox");
        panel.AddComponent<CanvasRenderer>();
        Image img = panel.AddComponent<Image>();
        img.color = color; // ʹ�ô������ɫ
        img.sprite = borderSprite;
        img.type = Image.Type.Sliced;
        panel.transform.SetParent(objectBoxParent, false);

        // Create the label
        var text = new GameObject("ObjectLabel");
        text.AddComponent<CanvasRenderer>();
        text.transform.SetParent(panel.transform, false);
        Text txt = text.AddComponent<Text>();
        txt.font = font;
        txt.color = color; // ������߽��һ�µ���ɫ
        txt.fontSize = 40;
        txt.horizontalOverflow = HorizontalWrapMode.Overflow;

        RectTransform rt2 = text.GetComponent<RectTransform>();
        rt2.offsetMin = new Vector2(20, rt2.offsetMin.y);
        rt2.offsetMax = new Vector2(0, rt2.offsetMax.y);
        rt2.offsetMin = new Vector2(rt2.offsetMin.x, 0);
        rt2.offsetMax = new Vector2(rt2.offsetMax.x, 30);
        rt2.anchorMin = new Vector2(0, 0);
        rt2.anchorMax = new Vector2(1, 1);

        boxPool.Add(panel);
        return panel;
    }

    private Color GetColorForLabel(string label)
    {
        // ����ֵ����Ѿ��������ǩ����ɫ��ֱ�ӷ���
        if (labelColorMap.ContainsKey(label))
            return labelColorMap[label];

        // ���û�У�Ϊ�ñ�ǩ����һ�������ɫ
        Color randomColor = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
        labelColorMap[label] = randomColor; // ���浽�ֵ���
        return randomColor;
    }

    private Texture2D CaptureCameraFrame()
    {
        RenderTexture currentRT = RenderTexture.active; // ���浱ǰ����� RenderTexture
        RenderTexture.active = renderTexture;

        Texture2D capturedTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        capturedTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        capturedTexture.Apply();

        RenderTexture.active = currentRT; // �ָ�֮ǰ�� RenderTexture
        return capturedTexture;
    }

    //�e�Υ�����ץȤˤ�ʹ�äǤ���
    public void ClearAnnotations()
    {
        foreach (var box in boxPool)
        {
            box.SetActive(false);
        }
    }

    private Color GetLabelColor(int classIndex)
    {
        return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value); // ʹ�������ɫʾ��
    }

}  
