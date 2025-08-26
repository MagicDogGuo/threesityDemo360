using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 简单的相机射线检测脚本，配合GazeObject.cs使用
/// </summary>
public class CameraRaycastGaze : MonoBehaviour
{
    [Header("射线设置")]
    public Camera gazeCamera;
    public float maxRayDistance = 100f;
    
    private GazeObject gazeObject;
    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;
    private GameObject currentTarget;
    
    void Start()
    {
        // 如果没有指定相机，使用主相机
        if (gazeCamera == null)
            gazeCamera = Camera.main;
            
        // 获取GazeObject组件
        gazeObject = FindObjectOfType<GazeObject>();
        if (gazeObject == null)
        {
            Debug.LogError("场景中没有找到GazeObject组件！请添加GazeObject到场景中。");
            return;
        }
        
        // 获取EventSystem和GraphicRaycaster
        eventSystem = EventSystem.current;
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
            
            // 如果是World Space Canvas，自动为按钮添加Collider
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                Debug.Log("检测到World Space Canvas，为按钮添加Collider...");
                AutoAddCollidersToButtons(canvas);
            }
        }
    }
    
    void AutoAddCollidersToButtons(Canvas canvas)
    {
        Button[] buttons = canvas.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            if (button.GetComponent<Collider>() == null)
            {
                BoxCollider collider = button.gameObject.AddComponent<BoxCollider>();
                RectTransform rect = button.GetComponent<RectTransform>();
                collider.size = new Vector3(rect.rect.width, rect.rect.height, 1f);
                Debug.Log($"为按钮 {button.name} 添加了BoxCollider");
            }
        }
    }
    
    void Update()
    {
        if (gazeObject == null) return;
        
        PerformRaycast();
    }
    
    void PerformRaycast()
    {
        // 从相机中心射出射线
        GameObject hitObject = null;
        Button hitButton = null;
        
        // 对于World Space Canvas，直接使用Physics Raycast
        Ray ray = gazeCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        
        // 首先尝试UI射线检测（适用于Screen Space Canvas）
        if (graphicRaycaster != null && eventSystem != null)
        {
            Canvas canvas = graphicRaycaster.GetComponent<Canvas>();
            
            // 检查Canvas的渲染模式
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay || canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                Debug.Log("========使用UI Raycast (Screen Space)");
                PointerEventData pointerData = new PointerEventData(eventSystem)
                {
                    position = new Vector2(Screen.width / 2, Screen.height / 2)
                };
                
                var results = new System.Collections.Generic.List<RaycastResult>();
                graphicRaycaster.Raycast(pointerData, results);
                Debug.Log("========UI Raycast results: " + results.Count);
                
                if (results.Count > 0)
                {
                    hitObject = results[0].gameObject;
                    hitButton = hitObject.GetComponent<Button>();
                    Debug.Log("========UI hitObject: " + hitObject.name);
                    
                    if (hitButton == null)
                        hitButton = hitObject.GetComponentInParent<Button>();
                }
            }
        }
        
        // 如果没有命中UI或者是World Space Canvas，尝试Physics射线检测
        if (hitObject == null)
        {
            Debug.Log("========使用Physics Raycast");
            if (Physics.Raycast(ray, out hit, maxRayDistance))
            {
                hitObject = hit.collider.gameObject;
                hitButton = hitObject.GetComponent<Button>();
                Debug.Log("========Physics hitObject: " + hitObject.name);
                
                if (hitButton == null)
                    hitButton = hitObject.GetComponentInParent<Button>();
                    
                if (hitButton != null)
                    Debug.Log("========找到Button: " + hitButton.name);
            }
        }
        
        // 调用GazeObject的接口方法
        if (hitButton != null && hitButton.interactable)
        {
            if (currentTarget != hitObject)
            {
                // 如果之前有目标，先退出
                if (currentTarget != null)
                    gazeObject.OnGazeExit(gazeCamera, currentTarget);
                
                // 开始新的注视
                currentTarget = hitObject;
                gazeObject.OnGazeStart(gazeCamera, hitObject, Vector3.zero, true);
            }
            
            // 持续注视
            gazeObject.OnGazeStay(gazeCamera, hitObject, Vector3.zero, true);
        }
        else
        {
            // 没有命中按钮，退出注视
            if (currentTarget != null)
            {
                gazeObject.OnGazeExit(gazeCamera, currentTarget);
                currentTarget = null;
            }
        }
    }
    
    // 在Scene视图中显示射线（调试用）
    void OnDrawGizmos()
    {
        if (gazeCamera != null)
        {
            Gizmos.color = Color.red;
            Vector3 rayStart = gazeCamera.transform.position;
            Vector3 rayDirection = gazeCamera.transform.forward;
            Gizmos.DrawRay(rayStart, rayDirection * maxRayDistance);
        }
    }
}
