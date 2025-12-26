# 文档信息
- **文件名**: 总框架
- **Level**: 0
- **ID**: 00000000
- **ParentID**: null
- **Time**: 20251218
- **Creator**: lvwan01

---

# 软件总框架设计书 (General Architecture)

## 1. 项目概述 (Project Overview)

### 1.1 项目愿景
开发一款高性能、跨平台的三维建模与渲染软件（对标 Rhino）。核心目标是构建一个架构完整、模块高度解耦、操作全流程可追溯（Undo/Redo）且具备高度可维护性的下一代 CAID（计算机辅助工业设计）工具。

### 1.2 设计哲学
*   **文档驱动**: 设计先行，代码紧随。
*   **模块化**: 强解耦设计，各模块通过标准接口（Trait/Interface）通信。
*   **命令化**: 一切操作皆命令，确保用户行为可记录、可撤销、可重放。
*   **AI Native**: 核心架构原生支持 AI 交互，为未来智能化辅助预留接口。

## 2. 技术架构 (Technical Architecture)

为了实现高效开发与高性能运行的平衡，本项目采纳以下技术栈：

*   **核心语言**: **Rust**
    *   *理由*: 内存安全，强类型系统强制模块解耦，Cargo 包管理利于依赖追踪。
*   **图形 API**: **wgpu** (Based on Vulkan/Metal/DX12)
    *   *实现*: 采用 WebGPU 标准的 Rust 实现，提供现代化的图形抽象层，兼顾跨平台兼容性与底层硬件性能。
*   **UI 框架**: **egui**
    *   *理由*: 纯 Rust 编写的 Immediate Mode GUI，集成方便，性能优秀，适合高频交互的编辑器类应用。
*   **数据交换**: **fbxcel**
    *   *实现*: 核心支持 FBX 格式的导入与解析。

## 3. 核心模块详解 (Core Modules)

### 3.1 基础设施层 (Infrastructure) - Level 1
*   **ID**: `10000000`
*   **功能**: 提供全局通用的基础服务。
*   **核心组件**:
    *   **日志系统**: 统一的日志记录。
    *   **数学库**: 基于 `glam` (Vec3, Mat4, Quat)。

### 3.2 几何内核 (Geometry Kernel) - Level 1
*   **ID**: `11000000`
*   **功能**: 处理纯数学几何计算。
*   **状态**: 待实现 (NURBS, BRep 数据结构)。

### 3.3 显示引擎 (Display Engine) - Level 1
*   **ID**: `12000000`
*   **功能**: 将几何数据转化为屏幕上的可视化图像。
*   **核心组件**:
    *   **RHI**: 基于 `wgpu` 的 RenderGraph 设计。
    *   **Shader System**: WGSL 实现 PBR (Physically Based Rendering) 渲染管线。
    *   **视口系统**: 支持多视口、透视/正交切换、Orbit/Game 导航模式。
    *   **Importer**: 内置 FBX 场景图解析，支持层级变换 (Transform Hierarchy) 与材质映射。

### 3.4 交互与 UI 系统 (Interaction & UI System) - Level 1
*   **ID**: `13000000`
*   **功能**: 处理用户输入、界面布局及命令执行。
*   **现状**: 核心逻辑集成在 `app` crate 中，`interaction` crate 暂作为逻辑迁移的目标容器。
*   **核心组件**:
    *   **UI 布局**: 仿 Rhino 布局 (Menu Bar, Toolbar, Viewport, Panels)。
    *   **工具集**: 集成标准 CAD 图标库 (SVG)。
    *   **交互逻辑**: 
        *   **Mouse Picking**: 射线拾取。
        *   **Zoom Box**: 框选缩放交互。
        *   **Context Menu**: 上下文菜单系统。

### 3.5 数据模型与持久化 (Data Model & Persistence) - Level 1
*   **ID**: `14000000`
*   **功能**: 核心数据结构的定义、管理。
*   **核心组件**:
    *   **FBX Support**: 完整的 DOM 解析与资源 (Texture/Material) 映射。

## 4. 扩展性设计 (Extensibility)
*   **模块化接口**: 模块间通过 Rust Trait 定义边界。
*   **插件系统**: (规划中) 支持 Lua/Python 脚本扩展。

## 5. 开发原则与规范 (Development Standards)
1.  **文档驱动**: 更新 Markdown 文档 -> AI 辅助编码。
2.  **ID 规范**: 8 位编码 (L0: `00000000`, L1: `1xxxxxxx`)。
3.  **文件/目录**: Snake Case (`snake_case`).
