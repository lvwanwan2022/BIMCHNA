# 核心逻辑层 (Kernel) 设计文档

**文档位置**: `crates/kernel/core_logic.md`
**关联模块**: `geometry` (上游数据), `display` (下游表现), `interaction` (输入指令)

## 1. 设计目标
Kernel 层是整个应用程序的“大脑”。它负责维护应用程序的真实状态（Truth），与渲染表现分离。
无论底层是 Vulkan 还是 WebGL，Kernel 层的逻辑必须保持一致。

## 2. 核心架构：ECS 与 场景图 (Scene Graph)
为了兼顾 CAD 的层级结构和游戏引擎的高性能，建议采用混合架构。

### 2.1 场景结构 (Scene Structure)
```rust
pub struct Scene {
    pub root: NodeId,
    pub registry: Registry, // 组件注册表
    pub resources: ResourceManager, // 材质、纹理、网格资源管理
}

pub struct Node {
    pub id: NodeId,
    pub parent: Option<NodeId>,
    pub children: Vec<NodeId>,
    pub transform: Transform, // 本地变换矩阵
    pub entity: Option<EntityId>, // 关联的实体数据
}
```

### 2.2 实体数据 (Entity Data)
实体数据直接链接到 `crates/geometry` 中的高精度数学模型。
```rust
pub struct Entity {
    pub geometry_ref: geometry::BrepModel, // 引用 Geometry crate 的数据
    pub render_proxy: Option<RenderProxyId>, // 指向 Display crate 的显示代理
    pub metadata: HashMap<String, String>, // 业务属性
}
```

## 3. 系统循环 (System Loop)
Kernel 维护一个主循环，负责驱动业务逻辑更新。

1.  **Input Phase**: 从 `crates/interaction` 获取标准化事件队列。
2.  **Update Phase**:
    *   执行脚本/插件逻辑。
    *   更新物理/约束求解器 (依赖 `geometry` 算法)。
    *   更新场景图变换矩阵 (Dirty Flag 模式)。
3.  **Render Sync Phase**:
    *   计算差异 (Diff)。
    *   将变换矩阵、材质变更、新增网格发送给 `crates/display`。

## 4. 与其他模块的关联实现

### 4.1 与 Geometry 的关联
*   **输入**: Kernel 持有 `geometry` 定义的 B-Rep 数据结构。
*   **操作**: 当发生建模操作（如布尔运算）时，Kernel 调用 `geometry` 的算法接口，更新 `BrepModel`，然后标记 `RenderProxy` 为 Dirty，触发重新网格化 (Tessellation)。

### 4.2 与 Display 的关联
*   **解耦**: Kernel **不引入** wgpu 或 opengl 依赖。
*   **接口**: 定义 `Renderable` trait 或 `RenderCommand` 队列。
    *   `Kernel` -> `RenderCommand::UpdateMesh(mesh_data)` -> `Display`
    *   `Display` 负责将 mesh_data 上传到 GPU 显存。

### 4.3 与 Interaction 的关联
*   **订阅**: Kernel 订阅 `interaction` 的 `EventBus`。
*   **射线检测**: 当 `interaction` 发出 `Click(x, y)` 事件时，Kernel 调用 `geometry` 或加速结构 (BVH) 进行射线求交，确定选中的 `NodeId`，然后更新选中状态。

