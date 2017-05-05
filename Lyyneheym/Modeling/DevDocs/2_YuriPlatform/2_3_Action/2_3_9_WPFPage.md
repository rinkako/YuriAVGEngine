## WPF页级别上的UI
Yuri引擎支持自定义WPF页面来实现更加复杂的响应式UI。

### WPF页面导航器
WPF页面导航器`ViewPageManager`为游戏窗体提供对前端页的通用接口，它允许用户在不同的WPF页面中切换。需要注意的是，在使用导航器服务时，如果切换的目标页面不是主舞台，那么导航器会通知总控制器`Director`暂停消息循环。

| 方法或属性名 | 作用 |
| :-------- | :-------- |
| RegisterPage(**string** pageId, **Page** pageRef) | 在页面管理器中注册一个WPF页面 |
| RetrievePage(**string** pageId) | 通过页面的唯一标识符获取WPF页面的引用 |
| Clear() | 清空管理器中所有的WPF页引用 |
| InitFirstPage(**Page** fp) | 初始化第一个页面 |
| NavigateTo(**string** toPageName) | 导航到目标页面 |
| GoBack() | 返回导航前的页面 |
| IsAtMainStage() | 获取当前是否位于主舞台页面 |
| CurrentPage | 获取当前呈现在屏幕上的页面 |


### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 层次结构 | Yuri.PlatformCore.Graphic.ViewPageManager |
| 最低版本 | 1.0 |
| 并行安全 | 否 |
