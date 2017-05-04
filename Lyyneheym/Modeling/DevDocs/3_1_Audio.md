## 音声
Yuri引擎使用NAudio作为底层的音声引擎，并对其提供的API进行了更高程度的封装。

### 播放通道（NAudioChannelPlayer）
Yuri引擎将每一个音声文件的播放称之为一个通道。通道可以调节音量、进行暂停和继续、播放和停止、以及播放完毕的回调。也就是说，一个通道是一个音声文件播放的控制器。在.NET Yuri引擎的实现中，它使用NAudio作为底层。<br/>
播放通道类提供了如下的服务：

| 方法或属性名 | 作用 |
| :-------- | :-------- |
| Init(**MemoryStream** playStream, **float** volume, **bool** loop, **Action** stopCallback) | 初始化通道，指定要播放的内存流、音量、是否循环和播放回调委托 |
| Play() | 播放通道 |
| Pause() | 暂停通道 |
| StopAndRelease() | 停止播放并释放这个通道的资源 |
| Dispose() | 释放通道 |
| Volume | 获取或设置通道音量 |
| IsPlaying | 获取该通道是否正在播放音乐 |
| IsLoop | 获取该通道是否循环播放 |
| CurrentTime | 获取通道播放的位置戳 |
| TotalTime | 获取通道音乐总长度 |

### 音频播放器（NAudioPlayer）
播放器类负责分配、初始化、维护和销毁通道。它是一个单例类，在运行时环境只有唯一实例。它提供了这些服务：

| 方法或属性名 | 作用 |
| :-------- | :-------- |
| InvokeChannel() | 请求一个通道 |
| InitAndPlay(**int** handle, **MemoryStream** ms, **float** vol, **bool** loop) | 初始化并播放通道 |
| StopAndRelease(**int** handle) | 停止通道并释放资源 |
| Pause(**int** handle) | 暂停通道的播放 |
| ResumePlay(**int** handle) | 恢复通道的播放 |
| SetVolume(**int** handle, **float** vol) | 调整通道的音量 |
| IsPlaying(**int** handle) | 获取通道是否处于播放状态 |
| Dispose() | 释放播放器中所有通道 |

### 音乐管理器（Musician）
音乐管理器负责游戏所有声效的维护和处理，它为画音渲染器`UpdateRender`所直接调用。引擎中任何需要播放声音的逻辑都应该通过它来播放。它提供了这些服务：

| 方法或属性名 | 作用 |
| :-------- | :-------- |
| PlayBGM(**string** resourceName, **MemoryStream** ms, **float** vol) | 播放背景音乐，如果当前已经有背景音乐就截断后再播放新的背景音乐 |
| PauseBGM()| 暂停背景音乐 |
| ResumeBGM() | 恢复播放背景音乐 |
| StopAndReleaseBGM() | 停止背景音乐 |
| SetBGMVolume(**float** vol) | 调整背景音乐的音量 |
| PlayBGS(**MemoryStream** ms, **float** vol, **int** track) | 播放背景音效 |
| StopBGS(**int** track) | 停止指定的背景声效 |
| SetBGSVolume(**int** vol, **int** track) | 调整指定背景声效的音量 |
| PlaySE(**MemoryStream** ms, **float** vol) | 播放声效 |
| PlayVocal(**MemoryStream** ms, **float** vol) | 播放角色语音，如果当前已经有角色语音正在播放就截断后再播放新的角色语音 |
| StopAndReleaseVocal() | 立即停止语音 |
| Dispose() | 释放音声相关资源 |
| BGSDefaultVolume | 获取或设置背景声效默认音量 |
| SEDefaultVolume | 获取或设置声效默认音量 |
| VocalDefaultVolume | 获取或设置语音默认音量 |
| IsBgmPlaying | 获取BGM是否正在播放 |
| IsBgmPaused | 获取BGM是否已经暂停 |
| CurrentBgm | 获取当前BGM名字 |
| IsAnyBgs | 获取是否有BGS在播放|
| IsMute | 获取或设置是否静音 |

### 程序集信息
| Property | Value |
| :-------- | :--------: |
| 层次结构   | Yuri.PlatformCore.Audio.NAudioChannelPlayer |
| 最低版本   | 1.0 |
| 并行安全   | - |
|||
| 层次结构   | Yuri.PlatformCore.Audio.NAudioPlayer |
| 最低版本   | 1.0 |
| 并行安全   | - |
|||
| 层次结构   | Yuri.PlatformCore.Audio.Musician |
| 最低版本   | 1.0 |
| 并行安全   | 是 |
