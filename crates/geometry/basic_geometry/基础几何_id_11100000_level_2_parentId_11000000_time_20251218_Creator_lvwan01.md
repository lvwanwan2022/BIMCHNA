# 文档信息
- **文件名**: 基础几何
- **Level**: 2
- **ID**: 11100000
- **ParentID**: 11000000
- **Time**: 20251218
- **Creator**: lvwan01

---

# 基础几何类型 (Basic Geometry)

## 1. 坐标与向量

### 1.1 XYZ
标准三维笛卡尔坐标/向量。

*   **属性字段 (Properties/Fields)**:
    *   `m_x`, `m_y`, `m_z` (Private fields)
    *   `X`, `Y`, `Z`: 坐标分量 (Read-only properties, modified via methods)
    *   `Length`: 向量长度
    *   `XY`: 投影到XY平面的XYZ对象 (Z=0)
    *   `UV`: 转换为UV对象 (X->U, Y->V)
    *   `BasisX`: (1, 0, 0)
    *   `BasisY`: (0, 1, 0)
    *   `BasisZ`: (0, 0, 1)
    *   `Zero`: (0, 0, 0)

*   **私有方法 (Private Methods)**:
    *   `GenerateId()`: 生成对象ID

*   **共有方法 (Public Methods)**:
    *   `ChangeX(double, string, string)`, `ChangeY(...)`, `ChangeZ(...)`: 修改坐标分量 (带权限验证)
    *   `IsZeroLength()`: 判断长度是否为0
    *   `IsUnitLength()`: 判断是否为单位向量
    *   `Normalize()`: 归一化 (返回新对象，若零向量则返回零向量)
    *   `Unitize()`: 同Normalize
    *   `GetLength()`: 获取长度
    *   `DotProduct(XYZ)`: 点积
    *   `CrossProduct(XYZ)`: 叉积
    *   `TripleProduct(XYZ, XYZ)`: 混合积
    *   `Add(XYZ)`, `Subtract(XYZ)`: 加减运算
    *   `Negate()`:取反
    *   `Multiply(double)`, `Divide(double)`: 数乘/除
    *   `Translation(XYZ)`: 平移
    *   `RotateByAxisAndDegree(XYZ, double)`: 绕轴旋转 (罗德里格公式)
    *   `IsAlmostEqualTo(XYZ, double)`: 模糊相等判断
    *   `DistanceTo(XYZ)`: 距离
    *   `AngleTo(XYZ)`: 夹角 (0~2π)
    *   `AngleOnPlaneTo(XYZ, XYZ)`: 平面上夹角
    *   `IsParallelTo(XYZ)`: 平行判断
    *   `IsPerpendicularTo(XYZ)`: 垂直判断
    *   `PerpendicularVector()`: 获取随机垂线向量
    *   `ToUV(string)`: 转换为UV (支持指定平面映射)
    *   `ToRAB()`: 转换为球坐标
    *   `TransformTo(TransformXYZ, out XYZ)`: 几何变换
    *   `ToString(...)`, `ToArray()`, `ToList()`, `FromList(IList<double>)`: 格式转换
    *   `IsXYZ()`, `IsUV()`, `IsRAB()`, `IsRS()`: 类型判断

*   **操作运算符 (Operators)**:
    *   `+`, `-`: 向量加减
    *   `*`: 点积 (Vector * Vector), 数乘 (Vector * double, double * Vector)
    *   `/`: 数除
    *   `[]`: 索引访问 (0->x, 1->y, 2->z)

### 1.2 UV
二维参数空间坐标。

*   **属性字段 (Properties/Fields)**:
    *   `m_u`, `m_v` (Private fields)
    *   `U`, `V`: 坐标分量
    *   `X` (alias for U), `Y` (alias for V)
    *   `Length`: 长度
    *   `Angle`: 角度 (0~2π)
    *   `XYZ`: 转换为XYZ对象 (Z=0)
    *   `BasisU`: (1, 0)
    *   `BasisV`: (0, 1)
    *   `Zero`: (0, 0)

*   **私有方法 (Private Methods)**:
    *   `GenerateId()`: 生成对象ID

*   **共有方法 (Public Methods)**:
    *   `Normalize()`: 归一化
    *   `GetLength()`: 获取长度
    *   `IsZeroLength()`: 判断长度是否为0
    *   `IsUnitLength()`: 判断是否为单位向量
    *   `ToXYZ(double)`: 转换为XYZ (可指定Z值)
    *   `DotProduct(UV)`: 点积
    *   `CrossProduct(UV)`: 二维叉积 (返回标量)
    *   `Add(UV)`, `Subtract(UV)`, `Negate()`, `Multiply(double)`, `Divide(double)`: 基础运算
    *   `IsAlmostEqualTo(UV, double)`: 模糊相等
    *   `DistanceTo(UV)`: 距离
    *   `RotateByDegree(double)`: 旋转
    *   `IsParallelTo(UV)`, `IsPerpendicularTo(UV)`: 平行/垂直判断
    *   `AngleTo(UV)`: 夹角
    *   `ConvertToRS()`: 转换为极坐标
    *   `TransformTo(TransformXYZ, out UV)`: 几何变换
    *   `ToString(...)`, `ToArray()`, `ToList()`, `FromList(IList<double>)`: 格式转换
    *   `IsXYZ()`, `IsUV()`, `IsRAB()`, `IsRS()`: 类型判断

*   **操作运算符 (Operators)**:
    *   `+`, `-`: 向量加减
    *   `*`: 数乘
    *   `/`: 数除
    *   `[]`: 索引访问 (0->u, 1->v)

### 1.3 RAB
球坐标系表示。

*   **属性字段 (Properties/Fields)**:
    *   `m_r`, `m_a`, `m_b` (Private fields)
    *   `R`: 半径 (Radius)
    *   `A`: 方位角 (Angle with BasisX)
    *   `B`: 仰角 (Angle with BasisZ)
    *   `Zero`: (0, 0, 0)
    *   `BasisX`, `BasisY`, `BasisZ`: 基向量表示

*   **私有方法 (Private Methods)**:
    *   `GenerateId()`: 生成对象ID

*   **共有方法 (Public Methods)**:
    *   `GetLength()`: 获取长度 (R的绝对值)
    *   `IsZeroLength()`, `IsUnitLength()`: 长度判断
    *   `Normalize()`: 归一化
    *   `DotProduct(RAB)`: 点积 (先转XYZ计算)
    *   `CrossProduct(RAB)`: 叉积 (转XYZ计算后转回RAB)
    *   `TripleProduct(RAB, RAB)`: 混合积
    *   `Add(RAB)`, `Subtract(RAB)`, `Negate()`, `Multiply(double)`, `Divide(double)`: 基础运算
    *   `IsAlmostEqualTo(RAB, double)`: 模糊相等
    *   `DistanceTo(RAB)`: 距离
    *   `AngleTo(RAB)`, `AngleOnPlaneTo(RAB, RAB)`: 夹角计算
    *   `IsParallel(RAB)`, `IsPerpendicular(RAB)`: 平行/垂直判断
    *   `ConvertToXYZ()`: 转换为XYZ
    *   `ToString(...)`: 格式化输出
    *   `IsXYZ()`, `IsUV()`, `IsRAB()`, `IsRS()`: 类型判断

*   **操作运算符 (Operators)**:
    *   `+`, `-`: 向量加减
    *   `*`: 数乘
    *   `/`: 数除
    *   `[]`: 索引访问 (0->r, 1->a, 2->b)

### 1.4 RS
极坐标系表示。

*   **属性字段 (Properties/Fields)**:
    *   `m_r`, `m_a` (Private fields)
    *   `R`: 半径
    *   `Sita` / `Angle`: 角度
    *   `Zero`: (0, 0)
    *   `BasisX`, `BasisY`: 基向量表示

*   **私有方法 (Private Methods)**:
    *   `GenerateId()`: 生成对象ID

*   **共有方法 (Public Methods)**:
    *   `GetLength()`: 获取长度
    *   `IsZeroLength()`, `IsUnitLength()`: 长度判断
    *   `Normalize()`: 归一化
    *   `DotProduct(RS)`, `CrossProduct(RS)`: 点积/叉积 (先转UV计算)
    *   `Add(RS)`, `Subtract(RS)`, `Negate()`, `Multiply(double)`, `Divide(double)`: 基础运算
    *   `IsAlmostEqualTo(RS, double)`: 模糊相等
    *   `DistanceTo(RS)`: 距离
    *   `AngleTo(RS)`, `AngleOnPlaneTo(RS, RS)`: 夹角计算
    *   `RotateByDegree(double)`: 旋转
    *   `IsParallel(RS)`, `IsPerpendicular(RS)`: 平行/垂直判断
    *   `ConvertToUV()`: 转换为UV
    *   `ToString(...)`, `ToList()`, `FromList(IList<double>)`: 格式转换
    *   `IsXYZ()`, `IsUV()`, `IsRAB()`, `IsRS()`: 类型判断

*   **操作运算符 (Operators)**:
    *   `+`, `-`: 向量加减
    *   `*`: 数乘
    *   `/`: 数除
    *   `[]`: 索引访问 (0->r, 1->a)

## 2. 辅助结构

### 2.1 Interval
一维数值区间。

*   **属性字段 (Properties/Fields)**:
    *   `d_s`, `d_e` (Private fields)
    *   `Start`: 起点
    *   `End`: 终点
    *   `Length`: 长度 (绝对值)

*   **共有方法 (Public Methods)**:
    *   `IsValueInInterval(double)`: 判断值是否在区间内
    *   `IsForward()`: 判断是否正向 (Start <= End)
    *   `IsBackward()`: 判断是否反向 (Start > End)
    *   `IsValueOnEnd(double)`: 判断是否在端点
    *   `IsCrossWithAnother(Interval)`: 判断区间相交
    *   `IsIncludeAnother(Interval)`: 判断包含
    *   `ToString()`: 格式化输出

### 2.2 PlaneXYZ
三维空间平面 (由原点、X轴、Y轴定义)。

*   **属性字段 (Properties/Fields)**:
    *   `o_po`, `u_vec`, `v_vec` (Private fields)
    *   `Origin`: 原点
    *   `Xaxis`: X轴向量 (Setting normalizes input)
    *   `Yaxis`: Y轴向量 (Setting normalizes input)
    *   `Normal`: 法向量 (Xaxis x Yaxis)
    *   `XaxisLine`, `YaxisLine`: 轴线
    *   `XYPlane`, `XZPlane`, `YZPlane`: 标准平面静态实例

*   **共有方法 (Public Methods)**:
    *   `CreateBy3Point(XYZ, XYZ, XYZ)`: 三点创建平面
    *   `IsIntersected(PlaneXYZ/LineXYZ/XLineXYZ)`: 相交判断
    *   `IsParallelTo`/`IsParallel(...)`: 平行判断
    *   `IsPerpendicularTo`/`IsPerpendicular(...)`: 垂直判断
    *   `GetIntersectXline(PlaneXYZ)`: 获取面面交线 (XLine)
    *   `GetIntersectLine(PlaneXYZ)`: 获取面面交线 (Line)
    *   `GetIntersectPoint(LineXYZ/XLineXYZ)`: 线面交点
    *   `GetProjectPoint(XYZ)`: 点投影
    *   `GetProjectLine(LineXYZ)`: 线投影
    *   `GetClosestPoint(XYZ)`: 获取最近点 (同GetProjectPoint)
    *   `GetPerpendicularXLineWithinPlane(...)`: 获取平面内垂线
    *   `DistanceTo(XYZ)`: 点面距离
    *   `IsPointOnPlane(XYZ)`: 点是否在平面上
    *   `ConvertUVToXYZ(UV)`, `ConvertXYZToUV(XYZ)`: 坐标转换 (点)
    *   `ConvertVectorUVToXYZ(UV)`, `ConvertVectorXYZToUV(XYZ)`: 坐标转换 (向量)
    *   `ConvertLineUVToLineXYZ(LineUV)`, `ConvertLineXYZToLineUV(LineXYZ)`: 线段转换
    *   `TransformTo(TransformXYZ, out PlaneXYZ)`: 几何变换
    *   `ToString(...)`, `ToList()`, `FromList(List<double>)`: 格式转换

*   **操作运算符 (Operators)**:
    *   `[]`: 索引访问 (0->Origin, 1->Xaxis, 2->Yaxis)

### 2.3 XPlane
轻量级平面 (由原点、法向量定义)。

*   **属性字段 (Properties/Fields)**:
    *   `o_po`, `x_nor` (Private fields)
    *   `Origin`: 原点
    *   `Normal`: 法向量

*   **共有方法 (Public Methods)**:
    *   `CreateBy3Point(XYZ, XYZ, XYZ)`: 三点创建平面
    *   `IsIntersected(XPlane/LineXYZ/XLineXYZ)`: 相交判断
    *   `IsParallel(XPlane/LineXYZ/XLineXYZ)`: 平行判断
    *   `IsPerpendicular(XPlane/LineXYZ/XLineXYZ)`: 垂直判断
    *   `DistanceTo(XYZ)`: 点面距离
    *   `IsPointOnPlane(XYZ)`: 点是否在平面上
    *   `GetClosestPoint(XYZ)` / `GetProjectPoint(XYZ)`: 获取最近点/投影点
    *   `GetIntersectPoint(LineXYZ/XLineXYZ)`: 线面交点
    *   `TransformTo(TransformXYZ, out XPlane)`: 几何变换
    *   `ToString(...)`, `ToList()`, `FromList(List<double>)`: 格式转换

*   **操作运算符 (Operators)**:
    *   `[]`: 索引访问 (0->Origin, 1->Normal)
