using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof (CameraSetup))]
public class CameraSetupDrawer : PropertyDrawer
{
    public float spacing = 3.0f;
    public int count = 7;

    float height;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * count + (count * spacing);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        
        SerializedProperty camera = property.FindPropertyRelative("camera");
        SerializedProperty tag = property.FindPropertyRelative("tag");
        SerializedProperty clearFlags = property.FindPropertyRelative("clearFlags");
        SerializedProperty backgroundColor = property.FindPropertyRelative("backgroundColor");
        SerializedProperty cullingMask = property.FindPropertyRelative("cullingMask");
        SerializedProperty depth = property.FindPropertyRelative("depth");
        
        GUIContent cameraContent = new GUIContent("Camera");
        GUIContent tagMaskContent = new GUIContent("Tag");
        GUIContent clearFlagsContent = new GUIContent("Clear Flags");
        GUIContent backgroundColorContent = new GUIContent("Background");
        GUIContent cullingMaskContent = new GUIContent("Culling Mask");
        GUIContent depthContent = new GUIContent("Depth");


        // Calculate the height of each element
        height = (position.height - (count * spacing)) / count;

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.LabelField(positionRect(position, 0), label, EditorStyles.boldLabel);
        EditorGUI.PropertyField(positionRect(position, 1), camera, cameraContent);
        tag.stringValue = EditorGUI.TagField(positionRect(position, 2), tagMaskContent, tag.stringValue);
        EditorGUI.PropertyField(positionRect(position, 3), clearFlags, clearFlagsContent);
        EditorGUI.PropertyField(positionRect(position, 4), backgroundColor, backgroundColorContent);
        EditorGUI.PropertyField(positionRect(position, 5), cullingMask, cullingMaskContent);
        EditorGUI.PropertyField(positionRect(position, 6), depth, depthContent);
        
        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public Rect positionRect(Rect position, int i)
    {
        return new Rect(position.x, position.y + (height + spacing) * i, position.width, height);
    }
}

#endif

[System.Serializable]
public class CameraSetup
{
    public Camera camera;   
    public string tag = "Untagged";
    public CameraClearFlags clearFlags;
    public Color backgroundColor;
    public LayerMask cullingMask;
    public int depth;
    
    public void Setup(float size)
    {
        if (camera == null)
            return;
        camera.tag = tag;
        camera.clearFlags = clearFlags;
        camera.backgroundColor = backgroundColor;
        camera.cullingMask = cullingMask;
        camera.depth = depth;
        camera.orthographic = true;
        camera.orthographicSize = size;
    }
}


//[ExecuteInEditMode]
public class TripletCameras : MonoBehaviour
{
    [SerializeField] private CameraSetup _mainCamera;
    [SerializeField] private CameraSetup _otherCamera;
    [SerializeField] private CameraSetup _alphaMaskCamera;
    [SerializeField] private int _alphaMaskDownResFactor;

    [Header("General Setup")]
    [SerializeField] private float size;
    
    void Start()
    {
        Setup();
    }

    private void OnValidate()
    {
        Setup();
    }

    void Setup()
    {
        _mainCamera.Setup(size);
        _otherCamera.Setup(size);
        _alphaMaskCamera.Setup(size);

        SetLightUniverseTex();
        SetAlphaMaskRT();
    }

    private void SetLightUniverseTex()
    {
        if (_otherCamera.camera == null)
            return;

        if (_otherCamera.camera.targetTexture != null) {
            RenderTexture temp = _otherCamera.camera.targetTexture;
            _otherCamera.camera.targetTexture = null;
            DestroyImmediate(temp);
        }

        int width = Screen.width > 0 ? Screen.width : 720;
        int height = Screen.height > 0 ? Screen.height : 1280;
        RenderTexture renderTexture = new RenderTexture(width, height, 24);

        renderTexture.name = "LightUniverseTex";
        renderTexture.filterMode = FilterMode.Bilinear;
        _otherCamera.camera.targetTexture = renderTexture;

        Shader.SetGlobalTexture("_LightUniverseTex", _otherCamera.camera.targetTexture);
    }

    private void SetAlphaMaskRT()
    {
        if (_mainCamera.camera == null || _alphaMaskCamera.camera == null)
            return;

        if (_alphaMaskCamera.camera.targetTexture != null)
        {
            RenderTexture temp = _alphaMaskCamera.camera.targetTexture;
            _alphaMaskCamera.camera.targetTexture = null;
            DestroyImmediate(temp);
        }

        RenderTexture renderTexture = new RenderTexture(
                _mainCamera.camera.pixelWidth >> _alphaMaskDownResFactor,
                _mainCamera.camera.pixelHeight >> _alphaMaskDownResFactor,
                8);

        renderTexture.name = "AlphaMaskTex";
        renderTexture.antiAliasing = 4;
        renderTexture.depth = 0;
        renderTexture.filterMode = FilterMode.Bilinear;
        _alphaMaskCamera.camera.targetTexture = renderTexture;

        Shader.SetGlobalTexture("_AlphaMaskTex", _alphaMaskCamera.camera.targetTexture);
    }
}
