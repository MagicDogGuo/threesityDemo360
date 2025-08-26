using UnityEngine;

/// <summary>
/// 射线检测调试助手，帮助诊断为什么需要很近才能检测到物件
/// </summary>
public class RaycastDebugHelper : MonoBehaviour
{
    [Header("调试设置")]
    public bool enableDebug = true;
    public bool showRayInScene = true;
    public float debugRayLength = 50f;
    
    [Header("相机设置")]
    public Camera debugCamera;
    
    void Start()
    {
        if (debugCamera == null)
            debugCamera = Camera.main;
    }
    
    void Update()
    {
        if (enableDebug && debugCamera != null)
        {
            PerformDebugRaycast();
        }
    }
    
    void PerformDebugRaycast()
    {
        // 从相机中心射出射线
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = debugCamera.ScreenPointToRay(screenCenter);
        
        Debug.Log("=== 射线调试信息 ===");
        Debug.Log($"相机位置: {debugCamera.transform.position}");
        Debug.Log($"相机旋转: {debugCamera.transform.rotation.eulerAngles}");
        Debug.Log($"射线起点: {ray.origin}");
        Debug.Log($"射线方向: {ray.direction}");
        Debug.Log($"屏幕中心点: {screenCenter}");
        
        // 检查相机设置
        Debug.Log($"近剪裁面: {debugCamera.nearClipPlane}");
        Debug.Log($"远剪裁面: {debugCamera.farClipPlane}");
        Debug.Log($"视野角度: {debugCamera.fieldOfView}");
        
        // 进行射线检测
        RaycastHit[] hits = Physics.RaycastAll(ray, debugRayLength);
        
        if (hits.Length > 0)
        {
            Debug.Log($"=== 检测到 {hits.Length} 个物件 ===");
            System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
            
            for (int i = 0; i < hits.Length; i++)
            {
                var hit = hits[i];
                Debug.Log($"物件[{i}]: {hit.collider.gameObject.name}");
                Debug.Log($"  - 距离: {hit.distance:F3}m");
                Debug.Log($"  - 碰撞点: {hit.point}");
                Debug.Log($"  - Collider类型: {hit.collider.GetType().Name}");
                Debug.Log($"  - Collider大小: {GetColliderSize(hit.collider)}");
                Debug.Log($"  - 物件位置: {hit.collider.transform.position}");
                Debug.Log($"  - 物件缩放: {hit.collider.transform.lossyScale}");
            }
        }
        else
        {
            Debug.Log($"在 {debugRayLength}m 范围内未检测到任何物件");
            Debug.Log("可能的原因:");
            Debug.Log("1. 物件没有Collider组件");
            Debug.Log("2. 物件在错误的Layer上");
            Debug.Log("3. 射线方向不正确");
            Debug.Log("4. 物件太远了");
        }
        
        // 检查场景中的所有Collider
        DebugAllColliders();
    }
    
    void DebugAllColliders()
    {
        Collider[] allColliders = FindObjectsOfType<Collider>();
        Debug.Log($"=== 场景中所有Collider (共{allColliders.Length}个) ===");
        
        for (int i = 0; i < allColliders.Length; i++)
        {
            var collider = allColliders[i];
            float distanceFromCamera = Vector3.Distance(debugCamera.transform.position, collider.transform.position);
            
            Debug.Log($"Collider[{i}]: {collider.gameObject.name}");
            Debug.Log($"  - 距离相机: {distanceFromCamera:F3}m");
            Debug.Log($"  - 类型: {collider.GetType().Name}");
            Debug.Log($"  - 启用状态: {collider.enabled}");
            Debug.Log($"  - Layer: {LayerMask.LayerToName(collider.gameObject.layer)}");
        }
    }
    
    string GetColliderSize(Collider col)
    {
        if (col is BoxCollider box)
            return $"BoxCollider - Size: {box.size}, Center: {box.center}";
        else if (col is SphereCollider sphere)
            return $"SphereCollider - Radius: {sphere.radius}, Center: {sphere.center}";
        else if (col is MeshCollider mesh)
            return $"MeshCollider - Convex: {mesh.convex}";
        else
            return col.GetType().Name;
    }
    
    void OnDrawGizmos()
    {
        if (showRayInScene && debugCamera != null)
        {
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Ray ray = debugCamera.ScreenPointToRay(screenCenter);
            
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray.origin, ray.direction * debugRayLength);
            
            // 画一个小球表示射线起点
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(ray.origin, 0.1f);
        }
    }
}

