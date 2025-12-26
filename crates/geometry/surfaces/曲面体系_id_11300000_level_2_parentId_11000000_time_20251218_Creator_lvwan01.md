# 文档信息
- **文件名**: 曲面体系
- **Level**: 2
- **ID**: 11300000
- **ParentID**: 11000000
- **Time**: 20251218
- **Creator**: lvwan01

---

# 曲面体系 (Surfaces)

## 1. Surface
*   **数学基础**: 基于 NURBS 数学表达。
*   **核心属性**:
    *   `DegreeU`, `DegreeV`: U/V 方向阶数。
    *   `KnotsU`, `KnotsV`: 节点向量。
    *   `Rational`: 是否有理（权重）。
    *   `XYZData`: 控制点列表。
    *   `DomainU`, `DomainV`: 定义域区间。
*   **功能**: 支持面积计算、坐标变换、参数求值。

