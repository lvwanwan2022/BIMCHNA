# 文档信息
- **文件名**: 交互系统
- **Level**: 1
- **ID**: 13000000
- **ParentID**: 00000000
- **Time**: 20251218
- **Creator**: lvwan01

---

# 交互系统设计 (Interaction System)

## 1. 模块概述
负责处理用户输入、视口交互、命令执行及工具状态管理。
**当前状态**: 核心交互逻辑目前紧耦合在 `app/src/ui/mod.rs` 的主循环中，`crates/interaction` 目前作为占位符，未来将逐步迁移以实现架构解耦。

## 2. 交互实现现状 (Current Implementation)

### 2.1 输入处理 (Input Handling)
基于 `egui` 的事件系统 (`ctx.input()`, `ui.interact()`) 实现。

*   **摄像机导航**:
    *   **Orbit Mode**: 
        *   右键拖拽: 旋转 (Rotate).
        *   中键拖拽: 平移 (Pan).
        *   滚轮: 缩放 (Zoom).
    *   **Game Mode**:
        *   WASD: 前后左右移动.
        *   Q/E (或 Space/X): 上下移动.
        *   Shift: 加速.
        *   鼠标移动 (Left/Right Drag): 视角旋转 (Look).
*   **快捷键**:
    *   `Escape`: 取消当前操作 (如框选缩放).

### 2.2 拾取系统 (Picking)
*   **实现方式**: CPU 端 AABB 射线检测 (`view.pick_mesh`).
*   **触发**: 左键点击 (Primary Click).
*   **反馈**:
    *   选中物体 ID 存储于 `MainWindow.selected_mesh`.
    *   渲染侧通过 `fs_selection` shader 绘制固定颜色的高亮 Overlay.
    *   控制台输出选中物体元数据 (Label, Translucency, AABB).

### 2.3 视口操作工具
*   **Zoom Box (框选缩放)**:
    *   **触发**: 上下文菜单 -> "Zoom to Box".
    *   **交互**: 
        *   拖拽绘制矩形框 (自定义 `painter` 绘制虚线与半透明填充).
        *   松开时计算框选区域对应的世界空间视锥体.
        *   调用 `camera_controller.zoom_to_fit(min, max)` 聚焦目标区域.
*   **Context Menu (上下文菜单)**:
    *   **触发**: 双击中键 (Double Click Middle Mouse).
    *   **功能**:
        *   Zoom Extents (全景).
        *   Zoom to Selected (聚焦选中).
        *   Zoom Random (随机聚焦 - 测试用).
        *   Zoom to Box (框选缩放).
        *   Previous Viewport (视图撤销).

## 3. 规划架构 (Planned Architecture)

### 3.1 命令模式 (Command Pattern)
*   **目标**: 将 `app` 中的直接函数调用封装为 `Command` 对象.
*   **结构**:
    *   `trait Command { fn execute(&mut self); fn undo(&mut self); }`
    *   **Command Stack**: 统一管理撤销/重做栈.

### 3.2 工具状态机 (Tool FSM)
*   **目标**: 将 `Zoom Box` 等临时状态迁移至通用的状态机.
*   **状态流**: `Idle` -> `Active (Drawing/Dragging)` -> `Commit` -> `Idle`.

### 3.3 界面布局 (Rhino-like)
当前已在 `ui/mod.rs` 中实现了基础框架：

#### 3.3.1 菜单栏 (Menu Bar)
*   **File**: New, Open, Save, Import (FBX 支持), Export.
*   **Edit**: Undo, Redo, Cut, Copy, Paste, Delete, Select All.
*   **View**: Zoom Extents, Pan, Rotate, Render Styles (Wireframe/Shaded).
*   **Geometry Categories**: Curve, Surface, Solid (占位符, 等待几何内核对接).

#### 3.3.2 工具栏 (Toolbar)
*   已集成 SVG 图标资源 (`crates/interaction/src/assets/*.svg`).
*   包含标准 CAD 工具图标: Select, Point, Polyline, Curve, Circle, Boolean, Trim, etc.
*   当前点击仅触发 UI 响应，尚未绑定具体 `Tool` 实现.

## 4. 资源与资产 (Assets)
*   SVG 图标库位于 `crates/interaction/src/assets/`.
*   通过 `egui_extras` 加载并在 `MainWindow` 中渲染为 `ImageButton`.
