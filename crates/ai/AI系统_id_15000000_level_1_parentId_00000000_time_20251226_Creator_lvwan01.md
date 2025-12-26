# 文档信息
- **文件名**: AI系统
- **Level**: 1
- **ID**: 15000000
- **ParentID**: 00000000
- **Time**: 20251226
- **Creator**: lvwan01

---

# AI 系统架构 (AI System Architecture)

## 1. 概述 (Overview)

AI 系统旨在为整个 CAID 软件提供智能辅助能力，通过集成大语言模型（LLM）等 AI 技术，提升用户的设计效率、降低二次开发门槛，并实现工作流的自动化串联。本模块作为基础设施层之上的服务层，横向支撑 UI、交互、脚本和文档等多个业务模块。

## 2. 核心功能模块 (Core Functional Modules)

### 2.1 AI 配置与管理 (AI Configuration & Management)
*   **功能**: 管理 AI 服务的连接信息和用户偏好。
*   **UI 组件**: `AISettingsPanel`
    *   **Provider 选择**: 支持 Deepseek, OpenAI, Azure, Local LLM 等。
    *   **API Key 管理**: 安全存储 API 密钥。
    *   **Model 选择**: 允许用户选择不同的模型（如 deepseek-coder, gpt-4）。
    *   **Prompt 模板管理**: 允许用户自定义系统级 Prompt。
*   **数据存储**: 配置信息持久化在本地配置文件或加密存储中。

### 2.2 智能命令系统 (Smart Command System)
*   **场景**: 用户不知道具体命令名称，或希望通过自然语言执行复杂操作。
*   **功能**:
    *   **命令补全 (Command Autocomplete)**: 基于上下文和历史记录，智能预测下一个可能的参数或命令。
    *   **NLP 转命令 (Natural Language to Command)**: 将 "创建一个半径为5的红色球体" 解析为 `CreateSphere(radius=5); SetMaterial(color=Red);`。
*   **集成点**: `crates/interaction` (Command Palette)。

### 2.3 二次开发辅助 (Scripting Copilot)
*   **场景**: 用户编写 Python/Lua 扩展脚本时。
*   **功能**:
    *   **代码生成**: 根据注释生成样板代码。
    *   **API 解释**: 解释选中的 API 用法。
    *   **错误修复**: 自动分析脚本运行错误并给出修复建议。
*   **集成点**: `crates/scripting` (内置编辑器)。

### 2.4 流程串联与自动化 (Workflow Orchestration)
*   **场景**: 需要将多个步骤串联成自动化工作流。
*   **功能**:
    *   **宏生成**: 观察用户的一系列操作，自动生成可重用的宏脚本。
    *   **智能工作流**: 用户描述目标（"把所有图层中名字包含'wall'的物体导出为STL"），AI 自动规划步骤并执行。

### 2.5 智能文档助手 (Documentation Assistant)
*   **场景**: 用户查询软件使用方法或开发文档。
*   **功能**:
    *   **RAG (Retrieval-Augmented Generation)**: 基于 `doc_system.db` 中的文档内容回答用户问题。
    *   **上下文感知**: 结合当前选中的对象类型，主动推荐相关文档。

## 3. 架构设计 (Architecture Design)

### 3.1 接口定义 (Trait Definition)
在 `crates/ai/src/lib.rs` 中定义核心 Trait，确保底层实现可替换。

```rust
pub trait AIService {
    async fn complete_text(&self, prompt: &str, context: &Context) -> Result<String>;
    async fn chat(&self, messages: &[Message]) -> Result<String>;
    async fn embed(&self, text: &str) -> Result<Vec<f32>>;
}
```

### 3.2 模块依赖
*   `ai` 依赖 `infrastructure` (日志, 基础类型)。
*   `ai` 依赖 `storage` (配置存储)。
*   `app` 依赖 `ai` (UI 呈现)。
*   `scripting` 依赖 `ai` (代码辅助)。

## 4. 实现路线图 (Roadmap)

1.  **Phase 1: 基础接入**
    *   实现 HTTP Client (基于 `reqwest`)。
    *   对接 Deepseek API。
    *   实现基础的设置面板。
2.  **Phase 2: 交互增强**
    *   接入命令搜索栏，实现模糊指令匹配。
    *   实现简单的文档问答。
3.  **Phase 3: 深度集成**
    *   脚本编辑器 Copilot 功能。
    *   基于上下文的参数预测。

## 5. 配置示例 (Configuration Example)

```json
{
  "active_provider": "deepseek",
  "providers": {
    "deepseek": {
      "endpoint": "https://api.deepseek.com/v1",
      "api_key": "sk-xxxxxxxx",
      "model": "deepseek-chat"
    }
  }
}
```

