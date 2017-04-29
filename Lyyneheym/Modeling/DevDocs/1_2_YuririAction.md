## Yuriri动作文法
- **并行安全**：是指在**信号系统的（反）激活函数**和**场景并行处理函数**中Yuri Engine保证这个命令不会导致调用堆栈出现无法预期的不安全行为，但**不能保证**用户编写的业务逻辑也是并行安全的。
- **单例安全**：指在一个时间段内，只会被一个堆栈（不一定是主调用堆栈）调用时，它是并行安全的。
- **Derivator**：对应该属性在进行退化LL(1)文法推导过程中表达式解析树根节点的语法类型。关于该文法的细节，请参照**文法设计**小节。

<br/>

### 显示对话
---
#### 功能
在当前文本层上显示文本
#### 语法
``` markdown
[
The messages to be print.
]
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------: | :-------: | :-------: |
| - | - | - |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 否 |

<br/>

### 切换角色状态
---
#### 功能
设定当前对话角色名字，播放的语音以及显示的立绘
#### 语法
``` markdown
@a 
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| name | 角色名 | id |
| face | 立绘表情 | id |
| loc | 立绘横向相对位置号 | wunit |
| vid | 语音编号 | wunit |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 否 |

<br/>

### 显示图片
---
#### 功能
在屏幕上显示一个可变化的2D精灵图片
#### 语法
``` markdown
@picture
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| id | 图片ID，也是精灵图片的Z值 | wunit |
| filename | 资源文件名 | id |
| x | 图片中心在屏幕上的X坐标 | wunit |
| y | 图片中心在屏幕上的Y坐标 | wunit |
| opacity | 不透明度比 | wunit |
| xscale | 横向缩放比 | wunit |
| yscale | 纵向缩放比 | wunit |
| ro | 顺时针旋转角度 | wunit |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 精灵动画
---
#### 功能
对屏幕上显示的精灵元素（图片、按钮）进行补间动画
#### 语法
``` markdown
@move
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| name | 动画所作用的精灵类型 | id |
| id | 要作用的精灵在该类别下的ID | wunit |
| time | 动画持续时间（毫秒） | wunit |
| target | 要应用补间动画的属性名 | id |
| dash | 不透明度比 | disjunct |
| acc | 加速度 | wunit |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 移除图片
---
#### 功能
将屏幕上的一个贴图图片移除
#### 语法
``` markdown
@deletepicture
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| id | 该图片的ID | wunit |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 放置立绘
---
#### 功能
在屏幕上的指定区块上显示标准规格立绘
#### 语法
``` markdown
@cstand
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| id | 立绘ID，也是立绘在屏幕相对分区的位置号 | wunit |
| name | 角色名字 | id |
| face | 角色表情 | id |
| x | [弃用的] | wunit |
| y | [弃用的] | wunit |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 移除图片
---
#### 功能
将屏幕上的一个立绘移除
#### 语法
``` markdown
@deletecstand
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| id | 立绘的ID | wunit |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 放置按钮
---
#### 功能
将按钮放置到屏幕上
#### 语法
``` markdown
@button
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| id | 文本层ID | wunit |
| normal | 正常状态的图标文件名 | id |
| over | 悬停状态的图标文件名 | id |
| on | 按下状态的图标文件名 | id |
| x | 按钮中心在屏幕的X坐标 | wunit |
| y | 按钮中心在屏幕的Y坐标 | wunit |
| target | 点击后**跳转**的目标 | wunit |
| sign | 点击后**调用**的函数 | wunit |
| type | 按钮类型（一次性/常驻） | wunit |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 移除按钮
---
#### 功能
将屏幕上的一个按钮移除
#### 语法
``` markdown
@deletebutton
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| id | 按钮的ID | wunit |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 显示背景
---
#### 功能
将一张图片加载到背景图层中
#### 语法
``` markdown
@bg
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| id | 背景层ID，指定前景或背景 | wunit |
| filename | 资源文件名 | id |
| x | 图片中心在屏幕上的X坐标 | wunit |
| y | 图片中心在屏幕上的Y坐标 | wunit |
| opacity | 不透明度比 | wunit |
| xscale | 横向缩放比 | wunit |
| yscale | 纵向缩放比 | wunit |
| ro | 顺时针旋转角度 | wunit |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 过渡
---
#### 功能
对屏幕上的背景图做过场动画，将背景层切换到前景层
#### 语法
``` markdown
@trans
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| name | 过渡效果名字 | id |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 否 |

<br/>

### 场景镜头
---
#### 功能
使用场景镜头动画
#### 语法
``` markdown
@scamera
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| name | 镜头特效名 | id |
| x | 镜头中心横向相对区块号 | wunit |
| y | 镜头中心纵向相对区块号 | wunit |
| ro | 镜头聚焦缩放比 | wunit |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 否 |

<br/>

### 切换文本层
---
#### 功能
将当前操作文本层切换到指定的层
#### 语法
``` markdown
@msglayer
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| id | 文本层ID | wunit |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 修改文本层属性
---
#### 功能
将指定的文本层的属性进行修改
#### 语法
``` markdown
@msglayeropt
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| id | 文本层ID | wunit |
| target | 要修改的属性名 | id |
| dash | 属性的目标值表达式 | disjunct |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 直接打印文本
---
#### 功能
将字符串直接显示在指定文字层上
#### 语法
``` markdown
@draw
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| id | 文本层ID | wunit |
| dash | 要打印内容的表达式 | disjunct |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 分发通知
---
#### 功能
推送一条系统级通知到屏幕
#### 语法
``` markdown
@notify
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| name | 通知大标题 | id |
| target | 通知详情 | id |
| filename | 通知Logo的文件名 | id |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 播放BGM
---
#### 功能
循环播放背景音乐
#### 语法
``` markdown
@bgm
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| filename | 文件名 | id |
| vol | 音量值 [1, 1000] | wunit |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 播放BGS
---
#### 功能
循环播放背景音效，支持多道播放
#### 语法
``` markdown
@bgs
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| filename | 文件名 | id |
| vol | 音量值 [1, 1000] | wunit |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 播放SE
---
#### 功能
播放一遍音效
#### 语法
``` markdown
@se
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| filename | 文件名 | id |
| vol | 音量值 [1, 1000] | wunit |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 播放Vocal
---
#### 功能
播放一遍角色的一条语音
#### 语法
``` markdown
@vocal
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| name | 角色名 | id |
| vid | 语音编号 | wunit |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 停止BGM
---
#### 功能
停止当前正在播放的背景音乐
#### 语法
``` markdown
@stopbgm
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------: | :-------: | :-------: |
| - | - | - |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 停止BGS
---
#### 功能
停止当前正在播放的所有背景音效
#### 语法
``` markdown
@stopbgs
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------: | :-------: | :-------: |
| - | - | - |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 停止Vocal
---
#### 功能
停止当前正在播放的角色语音
#### 语法
``` markdown
@stopvocal
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------: | :-------: | :-------: |
| - | - | - |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 返回
---
#### 功能
退出当前场景或函数
#### 语法
``` markdown
@return
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------: | :-------: | :-------: |
| - | - | - |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 返回开头
---
#### 功能
将游戏场景切换回入口场景
#### 语法
``` markdown
@title
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------: | :-------: | :-------: |
| - | - | - |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 否 |

<br/>

### 呼叫存档画面
---
#### 功能
暂停当前游戏并打开存档画面
#### 语法
``` markdown
@save
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------: | :-------: | :-------: |
| - | - | - |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 单例安全 |

<br/>

### 呼叫读取画面
---
#### 功能
暂停当前游戏并打开读取画面
#### 语法
``` markdown
@load
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------: | :-------: | :-------: |
| - | - | - |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 单例安全 |

<br/>

### 选择支
---
#### 功能
在屏幕上出现选择项并根据用户选择跳转到指定标签
#### 语法
``` markdown
@branch
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| link | 标签跳转链，形如"text1, lable1; ..." | id |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 否 |

<br/>

### 标签
---
#### 功能
为当前的脚本位置插入一个可作为跳转目的地的标签
#### 语法
``` markdown
@label
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| name | 标签的名字 | id |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 跳转
---
#### 功能
跳转到目标标签处继续执行
#### 语法
``` markdown
@jump
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| filename | 跳转的场景名 | id |
| target | 跳转的标签名 | id |
| cond | 跳转触发条件 | disjunct |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.Director |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 开关操作
---
#### 功能
修改指定的开关变量的状态
#### 语法
``` markdown
@switch
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| id | 开关的ID号 | wunit |
| state | 目标状态表达式 | disjunct |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 变量操作
---
#### 功能
修改指定的变量的状态
#### 语法
``` markdown
@var
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| name | 变量名 | wunit |
| dash | 目标值表达式 | disjunct |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 中断循环
---
#### 功能
从当前的循环里跳出并执行循环语句块后的脚本
#### 语法
``` markdown
@break
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 结束游戏
---
#### 功能
保存游戏相关信息并结束游戏程序
#### 语法
``` markdown
@shutdown
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 否 |

<br/>

### 延时等待
---
#### 功能
等待一定的时间间隔后再继续执行
#### 语法
``` markdown
@wait
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| time | 等待的毫秒数 | wunit |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.Director |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 等待用户
---
#### 功能
暂停脚本执行以等待用户的鼠标或键盘输入
#### 语法
``` markdown
@waituser
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 否 |

<br/>

### 等待动画结束
---
#### 功能
暂停脚本执行直到游戏里所有动画结束
#### 语法
``` markdown
@waitani
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.UpdateRender |
| 最低版本   | 1.0 |
| 并行安全   | 否 |

<br/>

### 函数调用
---
#### 功能
暂停当前处理并做函数调用，直到函数结束后再继续执行
#### 语法
``` markdown
@call
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| name | 函数的名字 | id |
| sign | 实参列表，形如"(arg1, arg2, ...)" | id |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.Director |
| 最低版本   | 1.0 |
| 并行安全   | 是 |

<br/>

### 信号量操作
---
#### 功能
操作游戏引擎的信号量系统
#### 语法
``` markdown
@semaphore
```
#### 参数
| Parameter Name | Value Type | Derivator |
| :-------- | :-------- | :-------- |
| name | 操作类型名 | id |
| target | 操作信号量的名字 | id |
| activator | 绑定信号激活处理函数名 | id |
| deactivator | 绑定信号反激活处理函数名 | id |
| dash | 操作目标值的表达式 | disjunct |
#### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 命名空间   | Yuri.YuriPlatform.PlatformCore.Director |
| 最低版本   | 1.0 |
| 并行安全   | 是 |
