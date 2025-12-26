# 前端架构 (Web Client) 设计文档

**文档位置**: `crates/web-client/frontend_architecture.md`
**关联模块**: `bridge` (通信), `app` (宿主环境)

## 1. 技术栈选型
*   **框架**: Vue 3 (Composition API) + TypeScript
*   **构建工具**: Vite (快速构建，支持 WASM)
*   **状态管理**: Pinia (管理 UI 状态，如侧边栏开关、当前选中工具)
*   **组件库**: TailwindCSS + Shadcn/ui (轻量、可定制，适合覆盖在 3D 画布上)

## 2. 目录结构
```
crates/web-client/
├── index.html
├── src/
│   ├── api/            # 封装 Bridge 调用 (Tauri invoke / WASM call)
│   ├── components/     # UI 组件 (Toolbar, PropertyPanel)
│   ├── stores/         # Pinia 状态
│   ├── views/          # 页面布局
│   └── main.ts
└── package.json
```

## 3. 核心功能实现

### 3.1 透明叠加层 (Overlay)
前端背景默认设为透明 (`background: transparent`)，只在有 UI 元素的地方响应鼠标。
*   **CSS**:
    ```css
    body, #app {
        background: transparent;
        pointer-events: none; /* 让鼠标穿透 */
    }
    .interactive-ui {
        pointer-events: auto; /* UI 元素恢复响应 */
    }
    ```

### 3.2 通信层封装 (The Adapter)
为了屏蔽 Native 和 Web 的差异，前端实现一个 `BridgeAdapter`。

```typescript
// src/api/bridge.ts

interface Bridge {
    sendCommand(cmd: string, payload: any): Promise<void>;
    onEvent(event: string, callback: (payload: any) => void): void;
}

// Native 实现 (Tauri)
const TauriBridge: Bridge = {
    sendCommand: (cmd, payload) => window.__TAURI__.invoke(cmd, payload),
    onEvent: (event, cb) => window.__TAURI__.event.listen(event, cb),
};

// Web 实现 (WASM)
const WasmBridge: Bridge = {
    sendCommand: (cmd, payload) => wasm_module.send_command(cmd, payload),
    onEvent: (event, cb) => { /* 注册回调 */ }
};

// 导出统一实例
export const bridge = isTauri ? TauriBridge : WasmBridge;
```

## 4. 业务逻辑流

### 4.1 工具切换
1.  用户点击工具栏 "Draw Line"。
2.  Frontend 调用 `bridge.sendCommand("set_tool", { type: "Line" })`。
3.  Kernel 收到指令，将交互状态机切换为 `LineDrawingState`。
4.  用户在空白处点击（事件穿透到 Rust 窗口）。
5.  Kernel 处理点击，更新几何数据。
6.  Kernel 发出 `MeshUpdated` 事件。
7.  Frontend 收到事件（如果需要更新 UI 提示，如“已绘制第一个点”）。

### 4.2 属性编辑
1.  用户在 3D 场景选中一个长方体。
2.  Kernel 发出 `SelectionChanged` 事件，携带 `{ length: 10, width: 5, height: 2 }`。
3.  Frontend 监听该事件，弹出属性面板，填入数值。
4.  用户修改 Length 为 15。
5.  Frontend 调用 `bridge.sendCommand("update_entity", { id: 1, length: 15 })`。
6.  Kernel 重新计算几何，触发渲染更新。

