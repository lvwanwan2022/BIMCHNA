# 架构探索：双模渲染与统一交互架构 (Dual-Mode Rendering & Unified Interaction)

**创建时间**: 2025-12-26
**状态**: 草案/探索

## 1. 核心构想与需求分析

### 1.1 核心需求
用户希望构建一个具备以下特性的三维软件架构：
1.  **双重显示逻辑 (Dual Display Logic)**:
    *   **Mode A (Web-based)**: 底层基于浏览器技术，UI使用 HTML/Vue/React。
    *   **Mode B (Native)**: 底层基于 Vulkan/OpenGL，追求极致性能。
2.  **统一交互逻辑 (Unified Interaction)**: 无论底层显示如何，三维空间的操作（旋转、缩放、拾取、编辑）逻辑必须一致。
3.  **未来扩展性**:
    *   目标是三维建模(CAD)或三维渲染软件。
    *   建模与渲染可能采用不同的引擎策略。
4.  **技术栈**: Rust, 开源优先。

### 1.2 现状分析与挑战
*   **挑战**: Web前端(DOM/Canvas)与原生图形API(Vulkan)的上下文管理完全不同。如何让同一套Rust代码同时驱动这两者？
*   **机遇**: Rust生态中的 **WGPU** 库正是为了解决这个问题而生，它遵循WebGPU标准，既可以编译为WASM在浏览器运行（调用WebGL/WebGPU），也可以在桌面编译为Native代码直接调用Vulkan/Metal/DX12。

---

## 2. 架构设计方案

### 2.1 核心技术选型建议

为了实现“一套代码，两套表现”，建议采用以下技术栈：

*   **图形后端 (RHI)**: **[wgpu](https://github.com/gfx-rs/wgpu)**
    *   *理由*: 它是Rust图形生态的事实标准。在Native端，它自动选择 Vulkan (Windows/Linux) 或 Metal (macOS)；在Web端，它通过WASM调用 WebGL2 或 WebGPU。这完美契合您“两套底层”但“一套代码”的需求。
*   **应用框架**: **[Tauri v2](https://v2.tauri.app/)**
    *   *理由*: Tauri 允许使用 Web 技术构建 UI，同时拥有 Rust 后端。Tauri v2 增强了对多窗口和移动端的支持。
*   **CAD/几何内核**: **[truck](https://github.com/ratel-rust/truck)** (B-Rep建模) 或 **[dimforge/parry](https://github.com/dimforge/parry)** (几何计算)。

### 2.2 架构分层图

```mermaid
graph TD
    subgraph "表现层 (Presentation Layer)"
        WebUI[Web Frontend (Vue/React)] -- DOM Events/Commands --> IPC[Tauri IPC / WASM Bridge]
        NativeWindow[Native Window (winit)]
    end

    subgraph "逻辑中台 (Application Core - Rust)"
        InputMgr[交互管理器 (Interaction Manager)]
        SceneGraph[场景图 (Scene Graph)]
        CommandSys[命令系统 (Command System)]
    end

    subgraph "渲染抽象层 (Render Abstraction)"
        RenderProxy[渲染代理 (Render Proxy)]
        WGPU[WGPU Backend]
    end

    subgraph "底层驱动 (Drivers)"
        Vulkan[Vulkan/DX12 (Native)]
        WebGL[WebGL/WebGPU (Browser)]
    end

    IPC --> InputMgr
    InputMgr --> CommandSys
    CommandSys --> SceneGraph
    SceneGraph --> RenderProxy
    RenderProxy --> WGPU
    WGPU --> Vulkan
    WGPU --> WebGL
    
    %% UI Overlay 关系
    WebUI -.->|Overlay UI| NativeWindow
```

---

### 2.3 两种实现模式的详细设计

为了满足您的“切换”需求，我们设计两种运行模式，它们共享 90% 的 Rust 代码。

#### 方案 A：原生高性能模式 (Native Overlay Mode) —— *推荐用于重度CAD*
这是高性能CAD软件的首选方案。
*   **原理**: 创建一个原生的系统窗口（由 Rust `winit` 并在其上通过 `wgpu` 跑 Vulkan）。
*   **UI**: 使用 Tauri 创建一个**透明的 WebView 窗口**覆盖在原生窗口之上。
*   **交互流**:
    1.  鼠标事件先穿过透明 Web UI。
    2.  如果点击的是 UI 按钮，Web 响应。
    3.  如果点击的是空白处（穿透），事件传递给底层的 Rust 窗口，触发三维交互。
*   **优点**: 3D 渲染性能能够跑满硬件（Vulkan），UI 开发效率极高（Vue/React）。

#### 方案 B：纯 Web 模式 (Browser Mode) —— *推荐用于轻量级查看/分享*
这是未来SaaS化的基础。
*   **原理**: Rust 代码编译为 `.wasm` 文件。
*   **UI**: 标准的 Vue/React 页面。
*   **渲染**: WGPU 代码编译为 WASM 后，直接接管 HTML `<canvas>` 元素，调用浏览器的 WebGL2/WebGPU。
*   **优点**: 用户无需安装，打开浏览器即可使用；代码与方案A几乎完全共用。

---

## 3. 核心模块设计 (Rust端)

为了实现“三维交互逻辑一样”，我们需要将交互逻辑与视口(Viewport)解耦。

### 3.1 交互总线 (Interaction Bus)
不要在渲染循环里直接写交互逻辑。
定义一组标准事件：
```rust
enum InteractionEvent {
    Hover(EntityId),
    Select(EntityId, SelectionMode),
    OrbitCamera { delta: Vec2 },
    PanCamera { delta: Vec2 },
}
```
无论事件来自 Web UI (JS 通过 Tauri invoke 发送) 还是来自原生窗口 (winit event)，都转化为这个枚举，统一发给 Core 处理。

### 3.2 渲染与建模的分离
您提到“建模软件和渲染软件可以不是一个三维显示引擎”。这是一个非常深刻的见解。

*   **数据模型 (The Truth)**:
    *   使用高精度的数学表示（如 B-Rep, NURBS）。
    *   库推荐: `truck-geometry` 或自定义结构。
    *   *这部分不依赖任何图形API*。

*   **显示模型 (The Proxy)**:
    *   将高精度模型“离散化(Tessellation)”为三角网格(Mesh)用于显示。
    *   如果做**建模软件**：显示引擎侧重于线框、边缘捕捉、快速刷新。
    *   如果做**渲染软件**：显示引擎侧重于光线追踪(Ray Tracing)、PBR材质。
    *   **架构策略**: 定义一个 `Renderable` trait，不同的引擎（实时编辑器 vs 离线渲染器）实现不同的 trait 方法来读取数据模型。

---

## 4. 实施路线图 (Roadmap)

1.  **阶段一：验证双端架构**
    *   初始化 Tauri 项目。
    *   配置 WGPU 环境。
    *   实现一个简单的 Demo：在 Native 窗口画一个三角形，同时有一个 Vue 写的小面板控制颜色。

2.  **阶段二：交互系统**
    *   实现 `Camera` 控制逻辑（平移、旋转、缩放）。
    *   实现 `RayCasting`（射线拾取）：从屏幕坐标发射射线检测物体。这一步是交互统一的关键。

3.  **阶段三：数据层与几何**
    *   引入简单的几何结构。
    *   实现“模型数据”到“WGPU Buffer”的单向同步流。

## 5. 待讨论问题

1.  **UI 事件穿透**: 在 Windows/Mac 上，让 WebView 透明并允许鼠标事件穿透到底层窗口可能需要一些系统级的 Trick（Tauri 正在完善此功能），是否考虑备选方案（如 UI 也由 Rust 绘制，用 `egui` 或 `slint`）？
    *   *思考*: 如果必须用 Vue/React，通过 WebSocket 或本地 Socket 通讯也是一种解耦方式，甚至可以将渲染进程作为子进程独立运行。
2.  **WebGPU 兼容性**: 目前浏览器对 WebGPU 的支持还在普及中，WGPU 的 WebGL2 后备方案性能尚可，但对于超大规模 CAD 模型可能不仅如 Native Vulkan。

---
*本文档由 AI 助手初步整理，旨在梳理架构思路。*

