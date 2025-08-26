using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR;

/// <summary>
/// 专门为Meta XR (Oculus) VR环境设计的头部注视射线检测脚本
/// 解决VR中射线偏移问题，确保准确的头部跟踪射线检测
/// </summary>
public class MetaXRGazeRaycast : MonoBehaviour
{
    [Header("VR设置")]
    [Tooltip("VR相机 (通常是CenterEyeAnchor)")]
    public Camera vrCamera;
    
    [Tooltip("射线最大检测距离")]
    public float maxRayDistance = 100f;
    
    [Tooltip("是否显示调试信息")]
    public bool showDebugInfo = true;
    
    [Header("GazeObject集成")]
    [Tooltip("是否自动查找GazeObject")]
    public bool autoFindGazeObject = true;
    
    private GazeObject gazeObject;
    private GameObject currentTarget;
    private GraphicRaycaster[] graphicRaycasters;
    private EventSystem eventSystem;
    
    // VR特有变量
    private Transform headTransform;
    private bool isVRActive = false;
    
    void Start()
    {
        InitializeVRComponents();
        InitializeGazeSystem();
    }
    
    void InitializeVRComponents()
    {
        // 检测VR状态
        isVRActive = XRSettings.enabled;
        
        if (isVRActive)
        {
            Debug.Log("VR模式已激活");
        }
        else
        {
            Debug.LogWarning("VR模式未激活，使用常规相机模式");
        }
        
        // 自动查找VR相机
        if (vrCamera == null)
        {
            // 优先查找VR相机标识
            Camera[] cameras = FindObjectsOfType<Camera>();
            foreach (Camera cam in cameras)
            {
                // 查找常见的VR相机名称
                if (cam.name.Contains("CenterEyeAnchor") || 
                    cam.name.Contains("CenterEye") ||
                    cam.name.Contains("VRCamera") ||
                    cam.name.Contains("XROrigin"))
                {
                    vrCamera = cam;
                    Debug.Log($"自动找到VR相机: {cam.name}");
                    break;
                }
            }
            
            // 如果还没找到，使用主相机
            if (vrCamera == null)
            {
                vrCamera = Camera.main;
                Debug.Log("使用主相机作为VR相机");
            }
        }
        
        if (vrCamera != null)
        {
            headTransform = vrCamera.transform;
            Debug.Log($"头部跟踪Transform设置为: {headTransform.name}");
        }
    }
    
    void InitializeGazeSystem()
    {
        // 获取EventSystem
        eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            Debug.LogError("场景中没有EventSystem！");
        }
        
        // 获取所有GraphicRaycaster
        graphicRaycasters = FindObjectsOfType<GraphicRaycaster>();
        Debug.Log($"找到 {graphicRaycasters.Length} 个GraphicRaycaster");
        
        // 获取GazeObject
        if (autoFindGazeObject)
        {
            gazeObject = FindObjectOfType<GazeObject>();
            if (gazeObject == null)
            {
                Debug.LogWarning("场景中没有找到GazeObject组件！");
            }
            else
            {
                Debug.Log("成功连接到GazeObject");
            }
        }
    }
    
    void Update()
    {
        if (headTransform == null) return;
        
        PerformVRGazeRaycast();
    }
    
    void PerformVRGazeRaycast()
    {
        // 使用VR头部的实际位置和方向创建射线
        Vector3 rayOrigin = headTransform.position;
        Vector3 rayDirection = headTransform.forward;
        
        Ray gazeRay = new Ray(rayOrigin, rayDirection);
        
        if (showDebugInfo)
        {
            Debug.Log("=== Meta XR 注视射线信息 ===");
            Debug.Log($"VR激活状态: {isVRActive}");
            Debug.Log($"头部位置: {rayOrigin}");
            Debug.Log($"头部方向: {rayDirection}");
            Debug.Log($"头部旋转: {headTransform.rotation.eulerAngles}");
        }
        
        // 检测UI和3D物件
        GameObject hitObject = DetectUIAndObjects(gazeRay);
        
        // 处理注视状态变化
        HandleGazeTarget(hitObject);
    }
    
    GameObject DetectUIAndObjects(Ray gazeRay)
    {
        GameObject hitObject = null;
        
        // 1. 首先检测UI (World Space Canvas)
        hitObject = DetectWorldSpaceUI(gazeRay);
        
        // 2. 如果没有命中UI，检测3D物件
        if (hitObject == null)
        {
            hitObject = Detect3DObjects(gazeRay);
        }
        
        // 3. 最后检测Screen Space UI (如果VR中有的话)
        if (hitObject == null)
        {
            hitObject = DetectScreenSpaceUI();
        }
        
        return hitObject;
    }
    
    GameObject DetectWorldSpaceUI(Ray gazeRay)
    {
        RaycastHit hit;
        if (Physics.Raycast(gazeRay, out hit, maxRayDistance))
        {
            // 检查是否命中UI元素
            if (hit.collider.GetComponent<Button>() != null ||
                hit.collider.GetComponentInParent<Button>() != null)
            {
                if (showDebugInfo)
                {
                    Debug.Log($"命中World Space UI: {hit.collider.gameObject.name} (距离: {hit.distance:F2}m)");
                }
                return hit.collider.gameObject;
            }
        }
        return null;
    }
    
    GameObject Detect3DObjects(Ray gazeRay)
    {
        RaycastHit[] hits = Physics.RaycastAll(gazeRay, maxRayDistance);
        
        if (hits.Length > 0)
        {
            System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
            
            if (showDebugInfo)
            {
                Debug.Log($"=== 3D物件检测结果 (共{hits.Length}个) ===");
                for (int i = 0; i < hits.Length; i++)
                {
                    var hit = hits[i];
                    Debug.Log($"3D物件[{i}]: {hit.collider.gameObject.name} (距离: {hit.distance:F2}m)");
                }
            }
            
            // 返回最近的有Button组件的物件，或者第一个物件
            foreach (var hit in hits)
            {
                if (hit.collider.GetComponent<Button>() != null ||
                    hit.collider.GetComponentInParent<Button>() != null)
                {
                    return hit.collider.gameObject;
                }
            }
            
            return hits[0].collider.gameObject;
        }
        
        return null;
    }
    
    GameObject DetectScreenSpaceUI()
    {
        // 对于VR，Screen Space UI通常不常用，但还是提供支持
        if (eventSystem == null || graphicRaycasters.Length == 0) return null;
        
        // 使用屏幕中心点
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = screenCenter
        };
        
        foreach (var raycaster in graphicRaycasters)
        {
            if (raycaster == null) continue;
            
            Canvas canvas = raycaster.GetComponent<Canvas>();
            if (canvas != null && (canvas.renderMode == RenderMode.ScreenSpaceOverlay || 
                                 canvas.renderMode == RenderMode.ScreenSpaceCamera))
            {
                var results = new System.Collections.Generic.List<RaycastResult>();
                raycaster.Raycast(pointerData, results);
                
                if (results.Count > 0)
                {
                    if (showDebugInfo)
                    {
                        Debug.Log($"命中Screen Space UI: {results[0].gameObject.name}");
                    }
                    return results[0].gameObject;
                }
            }
        }
        
        return null;
    }
    
    void HandleGazeTarget(GameObject hitObject)
    {
        if (gazeObject == null) return;
        
        // 检查是否有Button组件
        Button hitButton = null;
        if (hitObject != null)
        {
            hitButton = hitObject.GetComponent<Button>();
            if (hitButton == null)
                hitButton = hitObject.GetComponentInParent<Button>();
        }
        
        // 只有当命中按钮时才处理注视事件
        if (hitButton != null && hitButton.interactable)
        {
            if (currentTarget != hitObject)
            {
                // 切换到新目标
                if (currentTarget != null)
                {
                    gazeObject.OnGazeExit(vrCamera, currentTarget);
                }
                
                currentTarget = hitObject;
                gazeObject.OnGazeStart(vrCamera, hitObject, Vector3.zero, true);
                
                if (showDebugInfo)
                {
                    Debug.Log($"开始注视按钮: {hitObject.name}");
                }
            }
            
            // 持续注视
            gazeObject.OnGazeStay(vrCamera, hitObject, Vector3.zero, true);
        }
        else
        {
            // 没有命中按钮，退出注视
            if (currentTarget != null)
            {
                gazeObject.OnGazeExit(vrCamera, currentTarget);
                currentTarget = null;
                
                if (showDebugInfo)
                {
                    Debug.Log("退出注视");
                }
            }
        }
    }
    
    // 在Scene视图中可视化射线
    void OnDrawGizmos()
    {
        if (headTransform != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 rayOrigin = headTransform.position;
            Vector3 rayDirection = headTransform.forward;
            Gizmos.DrawRay(rayOrigin, rayDirection * maxRayDistance);
            
            // 画一个小球表示头部位置
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(rayOrigin, 0.05f);
        }
    }
    
    // 公共方法：手动设置VR相机
    public void SetVRCamera(Camera camera)
    {
        vrCamera = camera;
        if (camera != null)
        {
            headTransform = camera.transform;
            Debug.Log($"手动设置VR相机: {camera.name}");
        }
    }
    
    // 公共方法：获取当前注视的物件
    public GameObject GetCurrentGazeTarget()
    {
        return currentTarget;
    }
}
