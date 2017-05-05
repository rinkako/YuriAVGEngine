## 通知
Yuri引擎提供了向用户推送通知的队列服务。游戏业务逻辑可以通过通知管理器`NotificationManager`向前端推送通知，如果推送的通知前面还有通知正在等待显示，那么后来推送的通知项将会在队列中等待。<br/>
通知管理器提供如下的服务：

| 方法或属性名 | 作用 |
| :-------- | :-------- |
| Notify(**string** label, **string** detail, **string** icoFilename) | 发布一个复杂样式的通知到通知队列中并处理队列，它包含大标题、详情和图标，以动画的形式从屏幕右上角的边界处弹出 |
| SystemMessageNotify(**string** msg, **int** delayMS) | 发布一个持续显示一定时间的系统级通知，它将以简单文本的形式被直接显示在屏幕左上角 |
| IsNotificating | 获取或设置当前是否正在播放通知 |

### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 层次结构 | Yuri.PlatformCore.NotificationManager |
| 最低版本 | 1.0 |
| 并行安全 | 是 |

