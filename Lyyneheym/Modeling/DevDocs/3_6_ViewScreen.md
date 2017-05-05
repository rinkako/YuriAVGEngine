## 前端的渲染
Yuri引擎进行前端渲染是通过前端控制器`ViewManager`根据画面信息管理器`ScreenMananger`中各可视元素的描述子中描述的画面状态去进行画面渲染的。

### 前端管理器
前端管理器`ViewManager`负责将后台的所有可视对象描述子渲染到前端画面。因此，画面状态改变总是遵循这样的逻辑顺序进行的：

- 主渲染器`UpdateRender`修改画面信息管理器`ScreenManager`中对应变化项目的描述子
- 前端管理器`ViewManager`被要求重绘画面，它从`ScreenManager`得到需要重绘的对象的描述子，并根据描述子所描述的目标状态重新渲染它

在必要的时候，`ViewManager`会强制重绘整个画面，例如读取存档和回滚时。<br/>
前端管理器提供了如下的画面渲染服务：

| 方法或属性名 | 作用 |
| :-------- | :-------- |
| ReDraw() | 重绘整个画面 |
| Draw(**int** id, **ResourceType** rType) | 将描述子转化为相应前端项目并显示到前端 |
| GetSprite(**int** id, **ResourceType** rType) | 获取画面上的精灵实例 |
| GetMessageLayer(**int** id) | 获取画面上的文字层实例 |
| GetSpriteButton(**int** id) | 获取画面上的按钮实例 |
| GetBranchButton(**int** id) | 获取画面上的按钮实例 |
| GetViewport2D(**ViewportType** vt) | 获取画面上的2D视窗 |
| RemoveView(**ResourceType** rType) | 将指定类型的所有项目从画面移除|
| RemoveSprite(**int** id, **ResourceType** rType) | 将指定精灵从画面移除并释放 |
| RemoveButton(**int** id) | 将指定按钮从画面移除并释放 |
| GetTransitionBox() | 获取主视窗上的过渡容器 |
| ApplyTransition(**string** transTypeName) | 为背景层执行过渡动画 |
|RenderFrameworkElementToJPEG(**FrameworkElement** ui, **string** filename) | 将WPF窗体控件转化为JPEG图片 |
| InitMessageLayer() | 初始化文字层实例 |
| InitViewport2D() | 初始化2D视图，在游戏开始时调用 |
| InitViewport3D() | 初始化3D视图，在游戏开始时调用 |
| SetWindowReference(**MainWindow** wnd) | 为视窗管理器设置窗体的引用并更新视窗向量 |
| MaskFrameRef | 获取或设置黑场遮罩层的前端WPF对象Frame的引用 |
| View2D | 获取2D主舞台页面的引用 |
| View3D | 获取3D主舞台页面的引用 |
| Is3DStage | 获取当前是否为3D舞台 |

### 画面信息管理器
画面信息管理器`ScreenManager`是前端管理器当前状态进行可序列化描述的对象。

| 方法或属性名 | 作用 |
| :-------- | :-------- |
| AddBackground2D(**int** id, **string** source, **double** X, **double** Y, **int** Z, **double** Angle, **double** Opacity, **double** ScaleX, **double** ScaleY, **SpriteAnchorType** anchor, **Int32Rect** cut) | 为屏幕增加一个2D背景精灵描述子 |
| AddBackground3D(**string** source, **double** depth) | 为屏幕增加一个3D背景精灵描述子 |
| AddCharacterStand2D(**int** id, **string** source, **double** X, **double** Y, **int** Z, **double** Angle, **double** Opacity, **SpriteAnchorType** anchor, **Int32Rect** cut) | 为屏幕增加一个2D立绘精灵描述子 |
| AddCharacterStand3D(**int** id, **string** source, **int** depth) | 为屏幕增加一个3D立绘精灵描述子 |
| AddPicture(**int** id, **string** source, **double** X, **double** Y, **int** Z, **double** ScaleX, **double** ScaleY, **double** Angle, **double** Opacity, **SpriteAnchorType** anchor, **Int32Rect** cut) | 为屏幕增加一个图片精灵描述子 |
| AddButton(**int** id, **bool** enable, **double** X, **double** Y, **string** jumpTarget, **string** funcCallSign, **string** type, **SpriteDescriptor** normalDesc, **SpriteDescriptor** overDesc, **SpriteDescriptor** onDesc) | 为屏幕增加一个按钮描述子 |
| AddBranchButton(**int** id, **double** X, **double** Y, **string** jumpTarget, **string** text, **SpriteDescriptor** normalDesc, **SpriteDescriptor** overDesc, **SpriteDescriptor** onDesc) | 为屏幕增加一个选择支描述子 |
| EditMsgLayer(**int** id, **string** source, **bool** Visible, **double** W, **double** H, **Thickness** Padding, **double** X, **double** Y, **int** Z, **double** Opacity, **string** FontName, **Color** FontColor, **double** FontSize, **HorizontalAlignment** Ha, **VerticalAlignment** Va, **double** LineHeight, **bool** shadow) | 为屏幕增加一个文字层描述子 |
| RemoveSprite(**int** spriteId, **ResourceType** rType) | 从屏幕上移除一个精灵描述子 |
| RemoveMsgLayer(**int** layerId) | 从屏幕上移除一个文字层 |
| RemoveButton(**int** id) | 从屏幕上移除一个按钮 |
| RemoveBranchButton(**int** id) | 从屏幕上移除一个选择支 |
| GetViewboxDescriptor(**ViewportType** vt) | 获取一个视窗的描述子 |
| GetSpriteDescriptor(**int** id, **ResourceType** rType) | 获取一个精灵的描述子 |
| GetMsgLayerDescriptor(**int** id) | 获取一个文字层描述子 |
| GetButtonDescriptor(**int** id) | 获取一个按钮描述子 |
| GetBranchButtonDescriptor(**int** id) | 获取一个选择支描述子 |
| Backlay() | 交换背景的前后层的描述子 |
| InitViewboxes() | 初始化视窗向量 |
| InitMessageLayerDescriptors() | 初始化文字层描述子 |
| SCameraScale | 2D：获取或设置场景镜头当前相对于立绘层的缩放比；3D：获取或设置场景镜头的Z坐标 |
| SCameraCenterRow | 获取或设置场景镜头中央的屏幕分块行号 |
| SCameraCenterCol | 获取或设置场景镜头中央的屏幕分块列号 |
| SCameraFocusRow | 获取或设置场景镜头聚焦的屏幕分块行号 |
| SCameraFocusCol | 获取或设置场景镜头聚焦的屏幕分块列号 |


### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 层次结构 | Yuri.PlatformCore.Graphic.ViewManager |
| 最低版本 | 1.0 |
| 并行安全 | 是 |
|||
| 层次结构 | Yuri.PlatformCore.Graphic.ScreenManager |
| 最低版本 | 1.0 |
| 并行安全 | 是 |
