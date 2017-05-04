## 精灵按钮
精灵按钮`SpriteButton`为游戏提供了可点击的精灵控件。它通过**中断机制**来进行响应点击。同时它也提供了一些信号量诸如鼠标移入和离开等，以实现一些特殊的演绎效果（目前还未完善）。<br/>
精灵按钮提供的方法和属性如下：

| 方法或属性名 | 作用 |
| :-------- | :-------- |
| InitAnimationRenderTransform() | 初始化精灵的动画依赖 |
| MouseLeaveHandler(**object** sender, **MouseEventArgs** e) | 精灵按钮鼠标离开时的处理函数 |
| MouseEnterHandler(**object** sender, **MouseEventArgs** e) | 精灵按钮鼠标移入时的处理函数 |
| MouseOnHandler(**object** sender, **MouseEventArgs** e) | 精灵按钮鼠标按下时的处理函数 |
| MouseUpHandler(**object** sender, **MouseEventArgs** e) | 精灵按钮鼠标松开时的处理函数 |
| Id | 获取或设置按钮唯一编号 |
| Ntr | 获取或设置按下时的中断 |
| DisplayBinding | 获取或设置绑定前端显示控件 |
| Enable | 获取或设置按钮是否有效 |
| Eternal | 获取或设置按钮是否在点击后仍存留屏幕上 |
| X | 获取或设置按钮的X坐标 |
| Y | 获取或设置按钮的Y坐标 |
| Z | 获取或设置按钮的Z坐标 |
| ImageNormal | 获取或设置正常时的精灵对象 |
| ImageMouseOver | 获取或设置鼠标悬停时的精灵对象 |
| ImageMouseOn  | 获取或设置鼠标按下时的精灵对象 |
| IsMouseOver | 获取鼠标是否悬停在按钮上 |
| IsMouseOn | 获取鼠标是否按下按钮 |
| Anchor | 获取或设置动画锚点 |
| AnchorX | 获取锚点相对左上角的X坐标 |

### 精灵按钮的描述子
精灵按钮描述子对象`SpriteButtonDescriptor`是精灵按钮对象当前状态进行可序列化描述的对象。

| 方法或属性名 | 作用 |
| :-------- | :-------- |
| NormalDescriptor | 获取或设置按钮正常态精灵描述子 |
| OverDescriptor | 获取或设置按钮悬停态精灵描述子 |
| OnDescriptor | 获取或设置按钮按下态精灵描述子 |
| JumpLabel | 获取或设置按钮跳转标签 |
| InterruptFuncSign | 获取或设置按钮中断调用函数签名 |
| X | 获取或设置按钮X坐标 |
| Y | 获取或设置按钮Y坐标 |
| Z | 获取或设置按钮Z坐标 |
| Opacity | 获取或设置不透明度 |
| Enable | 获取或设置按钮是否可以点击 |
| Eternal | 获取或设置按钮是否可以在被点击后仍留存在屏幕上 |

### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 层次结构 | Yuri.PlatformCore.Graphic.SpriteButton |
| 最低版本 | 1.0 |
| 并行安全 | 是 |
|||
| 层次结构 | Yuri.PlatformCore.Graphic.SpriteButtonDescriptor |
| 最低版本 | 1.0 |
| 并行安全 | 是 |
