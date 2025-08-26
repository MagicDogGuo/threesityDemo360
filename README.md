# 360Demo Unity Project

## 專案描述
這是一個 Unity 360 度全景演示專案，支援 PC 和 VR 平台。

## ⚠️ 重要技術限制

### 1. 渲染 API 限制
**⚠️ 不能使用 Vulkan 渲染 API**
- 原因：VideoPlayer 組件在 Vulkan 下會出現問題
- 建議：使用 DirectX 11/12 或 OpenGL 作為渲染 API

### 2. 視線追蹤模組限制

#### PC 平台
- 使用：`HeadGazeInteractionModule`
- 適用於：桌面 PC 應用程式
- 功能：頭部視線追蹤互動

#### VR 平台
- 使用：`MetaXRGazeRaycast`
- 適用於：Meta Quest 等 VR 設備
- 原因：解決 VR 環境下的 raycast 問題
- **注意**：不要使用 `HeadGazeInteractionModule`，會導致 raycast 功能異常

## 平台配置

### PC 配置
```csharp
// 使用 HeadGazeInteractionModule
[SerializeField] private HeadGazeInteractionModule headGazeModule;
```

### VR 配置
```csharp
// 使用 MetaXRGazeRaycast
[SerializeField] private MetaXRGazeRaycast vrGazeRaycast;
```

## 專案結構
```
Assets/
├── Scripts/
│   ├── HeadGazeInteractionModule.cs    # PC 平台視線追蹤
│   ├── GazeObject.cs                   # 視線互動物件
│   └── ...
├── Scenes/
│   ├── 360Demo.unity                   # PC 版本場景
│   └── 360Demo_forVR.unity            # VR 版本場景
└── Resources/
    ├── 360Images/                      # 360 度圖片資源
    ├── Audios/                         # 音頻資源
    └── NormalImages/                   # 一般圖片資源
```

## 建置設定

### PC 版本
1. 選擇場景：`360Demo.unity`
2. 渲染 API：DirectX 11/12 或 OpenGL
3. 視線模組：`HeadGazeInteractionModule`

### VR 版本
1. 選擇場景：`360Demo_forVR.unity`
2. 渲染 API：DirectX 11/12 或 OpenGL
3. 視線模組：`MetaXRGazeRaycast`

## 依賴套件
- Unity 2022.3 LTS 或更新版本
- Universal Render Pipeline (URP)
- **Meta All-in-One SDK** (主要 XR 插件)
- **Oculus Integration** (Oculus 設備支援)
- AVPro Video (影片播放)

## XR 設定

### 必要設定
1. **XR Plugin Management** 必須啟用
2. **Oculus** 插件必須勾選並啟用
3. **Meta All-in-One SDK** 必須安裝並配置

### Project Settings 配置
```
Edit > Project Settings > XR Plugin Management
├── Initialize XR on Startup: ✓ 啟用
├── Plug-in Providers:
│   ├── Oculus: ✓ 啟用
│   └── 其他插件: 可選
└── Oculus Settings:
    ├── Shared Depth Buffer: 建議啟用
    ├── Dash Support: 建議啟用
    └── Low Overhead Mode: 建議啟用
```

## 故障排除

### VideoPlayer 問題
- 確保未使用 Vulkan 渲染 API
- 檢查影片格式是否支援
- 確認影片檔案路徑正確

### VR Raycast 問題
- 確認使用 `MetaXRGazeRaycast` 而非 `HeadGazeInteractionModule`
- 檢查 Meta All-in-One SDK 版本
- 確認 VR 設備連接正常

### XR 設定問題
- 確認 Project Settings > XR Plugin Management 已啟用
- 確認 Oculus 插件已勾選並啟用
- 檢查 Meta All-in-One SDK 是否正確安裝
- 確認 Initialize XR on Startup 已啟用
- 如果 VR 無法啟動，檢查 Oculus 軟體是否正常運行

## 開發注意事項
1. 新增功能時需考慮平台差異
2. 視線追蹤相關功能需分別實作 PC 和 VR 版本
3. 避免在 VR 場景中使用 PC 專用組件
4. 定期測試兩個平台的相容性
5. **XR 開發時必須使用 Meta All-in-One SDK 而非舊版 Oculus SDK**
6. **確保 Project Settings 中的 XR Plugin Management 正確配置**
7. **VR 測試前確認 Oculus 軟體和設備連接正常**

## 版本歷史
- 當前分支：`main`
- 主要功能：360 度全景展示、視線追蹤互動、影片播放
- 支援平台：PC、Meta Quest VR

## 聯絡資訊
如有問題或建議，請聯繫開發團隊。
