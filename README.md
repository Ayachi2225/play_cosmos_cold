# 打出宇宙冷漠自动播放宇宙冷漠

有三首歌，四种模式。原版，仅副歌，群星版（62 人合唱），可以可以在主页面的模组配置选择哪个版本的歌播放，也可以选择轮换模式（三首轮着来）

支持联机，任何人打出都会使安装方播放本地选择的歌曲；未安装方可以正常联机，只是听不到替换歌曲。歌曲偏好和轮换进度仅读取本地配置。

[演示](bilibili.com)

[宇宙冷漠 幻琉 HL](https://www.bilibili.com/video/BV19X9eBpEfS)

[62 人合唱宇宙冷漠](https://www.bilibili.comiadeo/BV1m9GE6wEPt)

## 依赖

依赖 baselib 提供歌曲配置界面

## 手动安装

1. 下载最新版本压缩包，解压后得到 `CosmosColdMusic` 文件夹。
2. 将整个 `CosmosColdMusic` 文件夹复制到游戏的 `mods` 目录下
3. 确保已安装依赖 BaseLib（见上文「依赖」），否则无法在游戏内切换歌曲。
4. 启动游戏，模组会自动加载；在 BaseLib 提供的模组配置界面里选择歌曲版本。

从游戏 exe 到 `CosmosColdMusic.json` 的完整层级结构如下：

```text
Slay the Spire 2\
├── SlayTheSpire2.exe
├── mods\
│   └── CosmosColdMusic\
│       ├── CosmosColdMusic.dll
│       ├── CosmosColdMusic.json
│       └── audio\
│           ├── original.mp3
│           ├── refrain.mp3
│           └── stars.mp3
```

也可以从源码构建：运行 `build.ps1` 生成压缩包，解压后按上面的结构复制到 `mods` 目录即可。

## 测试

在开发者控制台输入以下命令，将宇宙冷漠加入手牌，然后正常打出：

```text
card COSMIC_INDIFFERENCE
```
