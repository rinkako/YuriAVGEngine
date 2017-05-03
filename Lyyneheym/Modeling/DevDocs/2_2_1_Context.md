## 上下文

### 定义
在YRE中，管理变量作用域、变量名及其引用对象的包装被称为**上下文**。YRE负责管理各场景、函数的上下文初始化、切换、序列化等工作。<br/>

### 上下文的类型

#### IRuntimeContext（接口）
IRuntimeContext接口为YRE的上下文提供了一些常用接口的声明，它被所有上下文类型所实现。

| 函数名 | 作用 |
| :-------- | :-------- |
| Assign | 在此上下文中申请一个变量来储存指定对象的引用，如果指定变量名已存在，将覆盖原有的对象 |
| Remove | 从此上下文移除一个变量 |
| Fetch | 从此上下文中取一个变量名对应的对象 |
| Exist | 查找此上下文中是否存在某个变量 |
| Clear | 清空此上下文 |
| GetSymbols | 使用指定的筛选谓词查找此上下文符合条件的变量名 |

#### EvaluatableContext
可求值上下文。它意味着该上下文所包装的变量是非只读的，可运算的。可求值表达式是一个可序列化抽象类，不能直接创建使用它上下文对象。<br/>
它实现IRuntimeContext所声明的所有接口，并引入**名字空间**的概念，不同场景下的同名上下文变量可以通过命名空间进行隔离。<br/>

#### SimpleContext
简单上下文，用于函数调用中存放临时性变量的上下文，它只是可求值上下文的一个简单派生类，并且不应该在保存游戏状态（主动存档、快照）时被保存。<br/>
通常来说，它被存放在YRE的堆区中Eden Space区域。

#### SaveableContext
可保存上下文，用于场景内静态变量的存放，它记录所绑定的场景名称，维护场景中的变量及其引用。它可以被序列化，在保存游戏时应该被保存。<br/>
通常来说，它被存放在YRE的堆区中Permanent Space区域。

#### PersistContext
持久性上下文，用于存放存档无关的持久性变量，如游戏周目信息、成就信息等，这些与存档情况、回滚状态无关的变量都被统一放置于持久性上下文中。<br/>
持久性上下文实现了两个自有的方法，以独立进行上下文提交活动。

| 函数名 | 作用 |
| :-------- | :-------- |
| SaveToSteadyMemory | 保存持久上下文到稳定储存器 |
| LoadFromSteadyMemory | 从稳定储存器将持久上下文读入内存 |

通常来说，它被存放在YRE的堆区中Permanent Space区域。

### 上下文的继承关系

【TODO 图ContextRelationship】

### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 层次结构   | Yuri.PlatformCore.VM.IRuntimeContext |
| 最低版本   | 1.0 |
| 并行安全   | - |

| Property | Value |
| :-------- | :--------: |
| 层次结构   | Yuri.PlatformCore.VM.EvaluatableContext |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

| Property | Value |
| :-------- | :--------: |
| 层次结构   | Yuri.PlatformCore.VM.SimpleContext |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

| Property | Value |
| :-------- | :--------: |
| 层次结构   | Yuri.PlatformCore.VM.SaveableContext |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

| Property | Value |
| :-------- | :--------: |
| 层次结构   | Yuri.PlatformCore.VM.PersistContext |
| 最低版本   | 1.0 |
| 并行安全   | 是 |