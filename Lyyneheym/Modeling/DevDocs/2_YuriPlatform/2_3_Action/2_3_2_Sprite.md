## 精灵
Yuri引擎使用WPF作为底层的画面引擎，并对其提供的API进行了更高程度的封装。在Yuri引擎中，**精灵**对象即是对WPF可视化对象的一种封装，它的状态由可序列化的**精灵描述子**记录。

### 精灵对象
精灵对象`YuriSprite`是对WPF的可视化UI控件的一种封装，为游戏的图形资源提供展示、用户互动和动画效果。

| 方法或属性名 | 作用 |
| :-------- | :-------- |
| Init(**string** resName, **ResourceType** resType, **MemoryStream** ms, **Int32Rect?** cutrect) | 使用一个内存流初始化精灵对象，它只能被执行一次 |
| GetPixelColor(**double** X, **double** Y) | 获取一个相对于左上角的像素点的颜色 |
| IsEmptyRegion(**double** X, **double** Y, **int** threshold) | 判断一个相对于左上角的像素点是否全透明 |
| InitAnimationRenderTransform() | 初始化精灵的动画依赖 |
| Anchor | 获取或设置精灵动画锚点 |
| AnchorX | 获取精灵锚点相对精灵左上角的X坐标 |
| AnchorY | 获取精灵锚点相对精灵左上角的Y坐标 |
| CutRect | 获取或设置纹理切割矩形 |
| SpriteBitmapImage | 获取或设置纹理源 |
| DisplayBinding | 获取或设置前端显示控件 |
| AnimationElement | 获取或设置背景层实际显示控件的引用 |
| DisplayX | 获取或设置前端显示控件的X值 |
| DisplayY | 获取或设置前端显示控件的Y值 |
| DisplayZ | 获取或设置前端显示控件的Z值 |
| DisplayOpacity | 获取或设置前端显示控件的透明度 |
| DisplayWidth | 获取或设置前端显示控件的宽度 |
| DisplayHeight | 获取或设置前端显示控件的高度 |
| ImageWidth | 获取源图片的宽度 |
| ImageHeight | 获取源图片的高度 |
| IsDisplaying | 获取当前精灵是否被绑定到Image前端对象上 |
| IsScaling | 获取精灵是否被缩放 |
| ResourceType | 获取精灵的资源类型 |
| ResourceName | 获取精灵的资源名 |
| AnimateCount | 获取或设置正在进行的动画数量 |
| IsInit | 获取精灵是否已经初始化 |
| Descriptor | 获取或设置精灵的描述子 |
| TranslateTransformer | 获取或设置绑定的平移变换器 |
| ScaleTransformer | 获取或设置绑定的缩放变换器 |
| RotateTransformer | 获取或设置绑定的旋转变换器 |

### 精灵的描述子
精灵描述子对象`SpriteDescriptor`是精灵对象当前（以及动画结束后将要到达的）状态进行可序列化描述的对象。

| 属性名 | 作用 |
| :-------- | :-------- |
| Id | 获取或设置精灵id |
| X | 获取或设置精灵X坐标 |
| Y | 获取或设置精灵Y坐标 |
| Z | 获取或设置精灵Z坐标 |
| Slot3D | 获取或设置3D精灵所在的立绘槽号 |
| Deepth3D | 获取或设置3D精灵距离镜头的深度Z坐标 |
| Angle | 获取或设置精灵角度 |
| Opacity | 获取或设置精灵不透明度 |
| ScaleX | 获取或设置精灵X缩放 |
| ScaleY | 获取或设置精灵Y缩放 |
| BlurRadius | 获取或设置精灵的模糊半径 |
| ShadowRadius | 获取或设置精灵的投影半径 |
| AnchorType | 获取或设置精灵锚点方式 |
| ResourceType | 获取或设置精灵的资源类型 |
| ResourceName | 获取或设置精灵的资源名 |
| CutRect | 获取或设置精灵的纹理切割矩 |
| ToX | 获取或设置精灵在动画结束后的X坐标 |
| ToY | 获取或设置精灵在动画结束后的Y坐标 |
| ToZ | 获取或设置精灵在动画结束后的Z坐标 |
| ToAngle | 获取或设置精灵在动画结束后的角度 |
| ToOpacity | 获取或设置精灵在动画结束后的不透明度 |
| ToScaleX | 获取或设置精灵在动画结束后的X缩放 |
| ToScaleY | 获取或设置精灵在动画结束后的Y缩放 |
| ToBlurRadius | 获取或设置精灵在动画结束后的模糊半径 |
| ToShadowRadius | 获取或设置精灵在动画结束后的投影半径 |

### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 层次结构 | Yuri.PlatformCore.Graphic.YuriSprite |
| 最低版本 | 1.0 |
| 并行安全 | 是 |
|||
| 层次结构 | Yuri.PlatformCore.Graphic.SpriteDescriptor |
| 最低版本 | 1.0 |
| 并行安全 | 是 |
