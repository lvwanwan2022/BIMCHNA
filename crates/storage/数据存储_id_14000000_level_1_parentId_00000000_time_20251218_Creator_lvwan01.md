# 文档信息
- **文件名**: 数据存储
- **Level**: 1
- **ID**: 14000000
- **ParentID**: 00000000
- **Time**: 20251218
- **Creator**: lvwan01

---

# 数据存储设计 (Data Persistence)

## 1. 模块概述
负责文件 I/O 和数据交换。

## 2. 自定义格式 (.bmch)
*   基于二进制的序列化格式（如 Bincode 或 FlatBuffers）。
*   支持增量保存。

## 3. 数据交换
*   **Import**: OBJ, STL, IGES, STEP.
*   **Export**: OBJ, STL, GLTF.

