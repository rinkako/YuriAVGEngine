## 文本层
Yuri引擎使用文本层`MessageLayer`来为前端提供一个可变的文本显示控件，它的底层实现是使用了WPF的文本流内容容器TextBlock。

### 文本层对象
文本层对象`MessageLayer`提供的方法和属性如下：

| 方法或属性名 | 作用 |
| :-------- | :-------- |
| Reset() | 将文字层恢复初始状态 |
| StyleReset() | 恢复默认文字层的样式 |
| Id | 获取或设置文字层id |
| Text | 获取或设置文字层的文本 |
| Visibility | 获取或设置文字层是否可见 |
| FontName | 设置文字层字体 |
| FontSize | 获取或设置文字层字号 |
| FontColor | 获取或设置文字层的纯色颜色 |
| LineHeight | 获取或设置行距 |
| Opacity | 获取或设置文字层透明度 |
| X | 获取或设置文字层X坐标 |
| Y | 获取或设置文字层Y坐标 |
| Z | 获取或设置文字层深度坐标 |
| Height | 获取或设置文字层高度 |
| Width | 获取或设置文字层宽度 |
| Padding | 获取或设置文字层边距 |
| HorizontalAlignment | 获取或设置文字层水平对齐属性 |
| VerticalAlignment | 获取或设置文字层竖直对齐属性 |
| FontShadow | 获取或设置文字层的阴影状态 |
| DisplayBinding | 获取或设置文字层在前端的控件引用 |
| BackgroundSprite | 获取或设置文字层的背景精灵 |

### 文本层描述子
文本层描述子对象`MessageLayerDescriptor`是文本层对象当前状态进行可序列化描述的对象。

| 属性名 | 作用 |
| :-------- | :-------- |
| Id | 获取或设置文字层id |
| Text | 获取或设置文字层的文本 |
| Visible | 获取或设置文字层是否可见 |
| FontName | 设置文字层字体 |
| FontSize | 获取或设置文字层字号 |
| FontColorR | 获取或设置文本层字色R通道值 |
| FontColorG | 获取或设置文本层字色G通道值 |
| FontColorB | 获取或设置文本层字色B通道值 |
| LineHeight | 获取或设置行距 |
| Opacity | 获取或设置文字层透明度 |
| X | 获取或设置文字层X坐标 |
| Y | 获取或设置文字层Y坐标 |
| Z | 获取或设置文字层深度坐标 |
| Height | 获取或设置文字层高度 |
| Width | 获取或设置文字层宽度 |
| Padding | 获取或设置文字层边距 |
| HorizontalAlignment | 获取或设置文字层水平对齐属性 |
| VerticalAlignment | 获取或设置文字层竖直对齐属性 |
| FontShadow | 获取或设置文字层的阴影状态 |
| BackgroundResourceName | 获取或设置文本层背景图资源名称 |

### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 层次结构 | Yuri.PlatformCore.Graphic.MessageLayer |
| 最低版本 | 1.0 |
| 并行安全 | 是 |
|||
| 层次结构 | Yuri.PlatformCore.Graphic.MessageLayerDescriptor |
| 最低版本 | 1.0 |
| 并行安全 | 是 |
