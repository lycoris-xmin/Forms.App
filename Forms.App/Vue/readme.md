1. 提交信息格式：
提交信息通常遵循以下结构：

php-template
```
<类型>(<范围>): <简短描述>

<更详细的描述（可选）>
```

- 类型（Type）
常见的类型有：
  - feat：新增功能（feature）
  - fix：修复 bug
  - docs：文档变更
  - style：代码格式（不影响功能的更改，如空格、缩进、分号等）
  - refactor：代码重构（即不修复 bug 也不添加功能的代码更改）
  - perf：性能优化
  - test：增加或修改测试
  - chore：日常事务（如构建工具的更改、依赖更新等）
  - ci：持续集成相关的更改
  - build：影响构建系统或外部依赖的更改
  - revert：回滚提交

- 范围（Scope）
范围是可选的，用于描述更改涉及的具体模块或部分。比如：
  - feat(user-profile)：对 user-profile 模块的功能更新
  - fix(login)：修复 login 相关问题

- 简短描述（Subject）
简短描述要在 50 字符以内，且以动词开头，描述本次提交做了什么更改。

- 详细描述（Body）
详细描述提供更多的背景信息、为什么进行此更改，可能的副作用或已知问题等。每行最好不超过 72 个字符。

2. 示例：
- `feat(auth): add JWT authentication`
- `fix(header): resolve issue with sticky header`
- `docs(readme): update installation instructions`
- `style(navbar): align buttons properly`
- `refactor(user-profile): simplify user info retrieval`