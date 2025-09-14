using UnityEngine;
using UnityEngine.UI; // để dùng CanvasScaler

public class JoystickManager : MonoBehaviour
{
    [Header("Joystick UI Object")]
    public GameObject joystickUI; // kéo thả Joystick UI vào đây

    [Header("Canvas Scaler (UI Root)")]
    public CanvasScaler canvasScaler; // kéo CanvasScaler của Canvas vào đây

    [Header("Map Settings")]
    public SpriteRenderer backgroundSprite; // gán Background SpriteRenderer vào đây
    public Camera mainCamera;               // gán Main Camera vào đây

    void Awake()
    {
        // Ép buộc xoay ngang trên Android
#if UNITY_ANDROID && !UNITY_EDITOR
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
#endif
    }

    void Start()
    {
        // --- Joystick UI ---
        if (joystickUI != null)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            joystickUI.SetActive(true);
#elif UNITY_WEBGL
            joystickUI.SetActive(Application.isMobilePlatform);
#else
            joystickUI.SetActive(false);
#endif
        }

        // --- Canvas Scaler ---
        if (canvasScaler != null)
        {
            if (Application.isMobilePlatform)
            {
                // Điện thoại → scale theo màn hình thật
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
            }
            else
            {
                // PC giữ nguyên 1920x1080
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);
            }
        }

        // --- Camera Fit Map ---
        if (mainCamera != null && backgroundSprite != null)
        {
            FitCameraToMap();
        }
    }

    void Update()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // Nếu xoay ngang mà UI chưa fit thì refresh Canvas
        if (Screen.orientation == ScreenOrientation.LandscapeLeft)
        {
            Canvas.ForceUpdateCanvases();
        }
#endif
    }

    void FitCameraToMap()
    {
        Bounds bounds = backgroundSprite.bounds;
        float mapWidth = bounds.size.x;
        float mapHeight = bounds.size.y;

        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = mapWidth / mapHeight;

        if (screenRatio >= targetRatio)
        {
            // màn hình rộng hơn map → fit theo chiều cao
            mainCamera.orthographicSize = mapHeight / 2f;
        }
        else
        {
            // màn hình hẹp hơn map → fit theo chiều rộng
            float differenceInSize = targetRatio / screenRatio;
            mainCamera.orthographicSize = mapHeight / 2f * differenceInSize;
        }

        // Căn camera giữa background
        Vector3 mapCenter = bounds.center;
        mainCamera.transform.position = new Vector3(mapCenter.x, mapCenter.y, -10f);
    }
}
