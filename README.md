# 猫酱AI · 多平台网页套壳客户端

「猫酱AI」Android 应用（`top.ai.com`）的桌面端同款实现：一个使用**系统自带 WebView** 的网页套壳，不捆绑任何浏览器内核（不用 Electron）。

所有平台加载同一地址（解包 Android 版 APK 的 `assets/app_config.json` 所得）：

```
https://a15449a37364fd9b2.app.workbuddy.host
```

改加载地址：改 `linux/ai_wrap.c` 里的 `APP_URL`、`windows/Program.cs` 里的 `AppUrl`，重新构建即可。

## 目录结构

```
linux/    Linux 壳（C · WebKitGTK）+ build.sh（一键产出二进制 / deb / AppImage）
windows/  Windows 壳（C# WinForms · WebView2 · net48）+ setup/ 安装器工程
icons/    应用图标（SVG / PNG / ICO）
```

## 各平台实现

| 平台 | WebView | 说明 |
|---|---|---|
| Android | 系统 WebView | 见官网仓库 `assets/AI.apk` |
| Windows | 系统 WebView2 运行时 | .NET Framework 4.8 WinForms，Win10/11 零额外依赖 |
| Linux | 系统 WebKitGTK | 纯 C，二进制约 17 KB |

行为统一：单实例、外链/新窗口交给系统默认浏览器、窗口标题跟随网页。

## Linux 构建

依赖：`gcc`、`pkg-config`、`gtk3`、`webkit2gtk-4.1`、`dpkg-deb`、[appimagetool](https://github.com/AppImage/AppImageKit/releases)。

```bash
./app/linux/build.sh
# 产出：ai-app、ai-app_6.6.6_amd64.deb、ai-app_6.6.6_linux-x64.AppImage
```

## Windows 构建

依赖：.NET SDK（在 Linux/macOS/Windows 上均可交叉编译）。

```bash
# 1) 壳程序（WebView2 系统运行时）
cd windows
dotnet publish -c Release -r win-x64 --no-self-contained -o publish

# 2) 安装器（内嵌壳程序载荷，双击安装到 %LOCALAPPDATA%\AI，
#    创建开始菜单/桌面快捷方式并注册系统卸载入口）
cd setup
dotnet publish -c Release -o publish
# 产出 publish/AI-Setup.exe → 重命名为 AI-Setup-6.6.6.exe
```

## 安装包下载

各平台现成安装包发布在官网仓库的 Releases：
<https://github.com/xiaopi668/AI-official-website/releases/tag/v666>

官网下载页源码：<https://github.com/xiaopi668/AI-official-website>
