## 精灵动画
Yuri引擎为精灵对象提供了许多常用的2D动画效果，它们被封装在`SpriteAnimation`类中，其底层实现是使用WPF的故事板对象。动画特效分**依赖动画**和**互斥动画**两种类型，依赖动画之间可以叠加，即：平移和缩放可以同步进行；而互斥动画相对于一个精灵对象而言同一时刻只能执行一个，并且只能保留一种互斥动画的作用效果。

### 依赖动画
精灵动画类`SpriteAnimation`提供了这些依赖动画：

| 方法名 | 作用 |
| :-------- | :-------- |
| XYMoveAnimation(**YuriSprite** sprite, **Duration** duration, **double** fromX, **double** toX, **double** fromY, **double** toY, **double** accX, **double** accY) | 在笛卡尔平面上的规定时间内以指定的加速度从一个点向另一个点平移精灵 |
| XMoveAnimation(**YuriSprite** sprite, **Duration** duration, **double** fromX, **double** toX, **double** accX) | 在笛卡尔平面上的规定时间内以指定的加速度从一个点向另一个点横向平移精灵 |
| YMoveAnimation(**YuriSprite** sprite, **Duration** duration, **double** fromY, **double** toY, **double** accY) | 在笛卡尔平面上的规定时间内以指定的加速度从一个点向另一个点纵向平移精灵 |
| ZMoveAnimation(**YuriSprite** sprite, **Duration** duration, **int** fromZ, **int** toZ, **double** accZ) | 在图层层次上规定时间内以指定的加速度从一个深度向另一个深度平移精灵 |
| ScaleAnimation(**YuriSprite** sprite, **Duration** duration, **double** fromScaleX, **double** toScaleX, **double** fromScaleY, **double** toScaleY, **double** accX, **double** accY) | 在笛卡尔平面上规定时间内以指定的加速度关于锚点放缩精灵 |
| OpacityAnimation(**YuriSprite** sprite, **Duration** duration, **double** fromOpacity, **double** toOpacity, **double** acc) | 规定时间内以指定的加速度变更精灵的不透明度 |
| RotateAnimation(**YuriSprite** sprite, **Duration** duration, **double** fromTheta, **double** toTheta, **double** acc) | 在笛卡尔平面上规定时间内以指定的加速度关于锚点旋转精灵 |
| PropertyAnimation(**YuriSprite** sprite, **Duration** duration, **double** fromValue, **double** toValue, **double** acc, **PropertyPath** propath) | 让精灵在指定时间内从起始值到目标值以一定的加速度在指定依赖属性上作用一个双精度动画 |

### 互斥动画
精灵动画类`SpriteAnimation`提供了这些互斥动画：

| 方法名 | 作用 |
| :-------- | :-------- |
| BlurMutexAnimation(**YuriSprite** sprite, **Duration** duration, **double** fromRadius, **double** toRadius) | 规定时间内从指定的起始半径到目标半径对精灵进行高斯模糊 |
| ShadowingMutexAnimation(**YuriSprite** sprite, **Duration** duration, **Color** shadColor, **double** shadOpacity, **double** fromRadius, **double** toRadius) | 规定时间内从指定的起始半径到目标半径对精灵以指定的颜色和不透明度渲染外发光投影 |

### 辅助函数
精灵动画类`SpriteAnimation`提供了这些辅助函数：

| 方法或属性名 | 作用 |
| :-------- | :-------- |
| SkipAllAnimation() | 跳过所有动画 |
| FindMaxTimeSpan() | 获取当前动画里最大的时间间隔值 |
| ClearAnimateWaitingDict() | 结束全部动画并清空正在演绎动画字典 |
| IsAnyAnimation | 获取当前是否有精灵动画正在进行 |

### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 层次结构 | Yuri.PlatformCore.Graphic.SpriteAnimation |
| 最低版本 | 1.0 |
| 并行安全 | 是 |
