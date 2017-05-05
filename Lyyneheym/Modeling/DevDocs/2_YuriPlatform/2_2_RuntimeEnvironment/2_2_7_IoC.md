## 依赖注入
YRE提供了从Python脚本访问引擎内部公共方法和属性的接口，利用Python脚本做依赖注入可以实现更为复杂的业务逻辑和UI界面。基于IronPython的注入接口被封装在静态的世界类`YuririWorld`中，它可以提供如下的服务：

| 函数名 | 作用 |
| :-------- | :-------- |
| Execute | 执行一个Python脚本字符串，它拥有访问和修改引擎公共内容的能力 |
| ExecuteFile | 执行一个Python脚本文件，它拥有访问和修改引擎公共上下文的能力 |
| ExecuteIsolation | 为Python脚本字符串创建一个与其他脚本以及游戏运行时环境隔离性的上下文并执行它 |
| ExecuteFileIsolation | 为Python脚本文件创建一个与其他脚本以及游戏运行时环境隔离性的上下文并执行它 |

外部的Python脚本通过引用变量`_YuririReflector_`来操作YRE。该变量的类型是引擎反射器`YuririReflector`，它利用**反射机制**提供了执行静态方法、创建类的实例和输出内容等接口。<br/>
依赖注入仍在继续改善中，目前不建议使用。
