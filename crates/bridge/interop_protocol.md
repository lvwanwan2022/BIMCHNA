# 桥接层 (Bridge) 通信协议设计

**文档位置**: `crates/bridge/interop_protocol.md`
**关联模块**: `web-client` (前端), `interaction` (后端处理), `app` (Tauri 宿主)

## 1. 设计目标
定义 Web 前端 (Vue/React) 与 Rust 核心 (Kernel) 之间的通信标准。
无论前端是作为 Native App 的 Overlay 运行 (Tauri IPC)，还是作为纯 Web App 运行 (WASM Bindgen)，通信接口应保持一致。

## 2. 通信模式

### 2.1 命令模式 (Command Pattern) - 前端调用后端
用于前端触发后端操作。
*   **Tauri 实现**: `#[tauri::command]`
*   **WASM 实现**: `#[wasm_bindgen]`

**协议定义 (JSON Payload)**:
```json
{
  "category": "Camera",
  "action": "Pan",
  "payload": { "x": 10.5, "y": -5.0 }
}
```

**Rust 接口定义**:
```rust
// crates/bridge/src/commands.rs

#[derive(Serialize, Deserialize)]
pub enum AppCommand {
    LoadModel { path: String },
    SelectEntity { id: u32, mode: String },
    CameraOperation { op: CameraOp },
}

// 统一入口
pub fn handle_command(cmd: AppCommand, kernel: &mut Kernel) {
    match cmd {
        AppCommand::LoadModel { path } => kernel.load_file(path),
        // ... 分发给 kernel 或 interaction 模块
    }
}
```

### 2.2 事件模式 (Event Pattern) - 后端通知前端
用于后端状态变化更新前端 UI（如属性面板更新）。
*   **Tauri 实现**: `window.emit("event-name", payload)`
*   **WASM 实现**: 调用 JS 注册的回调函数。

## 3. 接口实现细节

### 3.1 视口同步 (Viewport Sync)
由于 WebUI 可能是透明覆盖层，前端需要知道 3D 场景的投影信息来实现 2D 标签跟随 3D 物体。
*   **接口**: `GetScreenCoordinates(world_pos: Vec3) -> Vec2`
*   **落地**: Bridge 层调用 `display` 层的 Camera 矩阵进行计算，返回给前端。

### 3.2 资源传递
对于大文件（如模型上传），不能使用 JSON 序列化。
*   **Native**: 直接传递文件路径，Rust `std::fs` 读取。
*   **Web**: 前端传递 `File` 对象 -> `Uint8Array` -> Rust `Vec<u8>`。
*   **Bridge 策略**: 定义 `ByteSource` 枚举来抽象这两种来源。

## 4. 模块关联
*   **Web-Client**: 生成 TypeScript 类型定义 (`ts-rs` crate)，确保前端发送的 JSON 符合 Rust 结构体定义。
*   **Interaction**: Bridge 接收到的鼠标/键盘事件会被转化为 `interaction` 模块的标准 Input 事件。

