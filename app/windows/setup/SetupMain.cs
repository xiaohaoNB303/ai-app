// AI 安装程序 · Windows（当前用户安装，无需管理员）
// 双击后：释放壳程序到 %LOCALAPPDATA%\AI，创建开始菜单/桌面快捷方式，注册系统卸载入口。
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;

namespace AISetup
{
    internal static class Program
    {
        private const string AppName = "猫酱AI";
        private const string Version = "6.6.6";
        private const string UninstallBat =
            "@echo off\r\n" +
            "echo Uninstalling AI ...\r\n" +
            "del \"%APPDATA%\\Microsoft\\Windows\\Start Menu\\Programs\\猫酱AI.lnk\" >nul 2>&1\r\n" +
            "del \"%USERPROFILE%\\Desktop\\猫酱AI.lnk\" >nul 2>&1\r\n" +
            "reg delete \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\AI\" /f >nul 2>&1\r\n" +
            "cd /d \"%LOCALAPPDATA%\"\r\n" +
            "rmdir /s /q \"%LOCALAPPDATA%\\AI\"\r\n" +
            "echo AI uninstalled.\r\n" +
            "pause\r\n";

        private static readonly string[] Payload =
        {
            "AI.exe", "AI.exe.config", "Microsoft.Web.WebView2.Core.dll",
            "Microsoft.Web.WebView2.WinForms.dll", "WebView2Loader.dll", "ai.ico"
        };

        [STAThread]
        private static void Main()
        {
            try
            {
                string dst = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AI");
                Directory.CreateDirectory(dst);

                Assembly self = Assembly.GetExecutingAssembly();
                foreach (string name in Payload)
                {
                    using (Stream src = self.GetManifestResourceStream(name))
                    {
                        if (src == null) throw new IOException("缺少内嵌资源：" + name);
                        using (FileStream output = File.Create(Path.Combine(dst, name)))
                            src.CopyTo(output);
                    }
                }
                File.WriteAllText(Path.Combine(dst, "uninstall.bat"), UninstallBat);

                string exe = Path.Combine(dst, "AI.exe");
                string ico = Path.Combine(dst, "ai.ico");

                CreateShortcut(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"Microsoft\Windows\Start Menu\Programs\" + AppName + ".lnk"), exe, ico, dst);
                CreateShortcut(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    AppName + ".lnk"), exe, ico, dst);

                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Uninstall\AI"))
                {
                    key.SetValue("DisplayName", AppName);
                    key.SetValue("DisplayVersion", Version);
                    key.SetValue("Publisher", "xiaopi668");
                    key.SetValue("DisplayIcon", ico);
                    key.SetValue("UninstallString", "\"" + Path.Combine(dst, "uninstall.bat") + "\"");
                    key.SetValue("NoModify", 1, RegistryValueKind.DWord);
                    key.SetValue("NoRepair", 1, RegistryValueKind.DWord);
                }

                if (MessageBox.Show(
                        AppName + " " + Version + " 安装完成（开始菜单 + 桌面快捷方式）。\r\n\r\n是否立即运行？",
                        "猫酱AI 安装程序", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(exe);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("安装失败：" + ex.Message, "猫酱AI 安装程序",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void CreateShortcut(string linkPath, string target, string icon, string workDir)
        {
            object shell = Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell"));
            object link = shell.GetType().InvokeMember("CreateShortcut",
                BindingFlags.InvokeMethod, null, shell, new object[] { linkPath });
            link.GetType().InvokeMember("TargetPath",
                BindingFlags.SetProperty, null, link, new object[] { target });
            link.GetType().InvokeMember("IconLocation",
                BindingFlags.SetProperty, null, link, new object[] { icon });
            link.GetType().InvokeMember("WorkingDirectory",
                BindingFlags.SetProperty, null, link, new object[] { workDir });
            link.GetType().InvokeMember("Save",
                BindingFlags.InvokeMethod, null, link, null);
        }
    }
}
