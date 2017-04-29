## Yuriri逻辑文法

### 条件分支
---
#### 功能
按照真值表达式的真伪选择不同的执行路径
#### 语法
``` markdown
@if cond = disjunct
  TrueRouteStatements
@else [可选的]
  FalseRouteStatements [可选的]
@endif
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :------- | :------- | :------- |
| cond | 真值表达式 | disjunct |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.RuntimeManager |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 循环
---
#### 功能
在真值表达式为真时对某个代码片段进行反复执行
#### 语法
``` markdown
@for cond = disjunct
  TrueRouteStatements
@endfor
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :------- | :------- | :------- |
| cond | 真值表达式 | disjunct |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.RuntimeManager |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 函数体
---
#### 功能
定义一个函数
#### 语法
``` markdown
@function sign="foo(arg1, ...)"
  Scripts
@endfunction
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :------- | :------- | :------- |
| sign | 函数签名结构化字符串 | id |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.Yuriri.SceneFunction |
| 最低版本   | 1.0 |
| 并行安全   | - |
