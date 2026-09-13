// AI 桌面壳 · Windows（WinForms + 系统自带 WebView2 运行时）
using System;
using System.Windows.Forms;

namespace AI
{
    internal static class Program
    {
        private const string AppUrl = "https://a15449a37364fd9b2.app.workbuddy.host";

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var form = new Form
            {
                Text = "猫酱AI",
                ClientSize = new System.Drawing.Size(1200, 800),
                MinimumSize = new System.Drawing.Size(420, 320),
                StartPosition = FormStartPosition.CenterScreen,
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath)
            };

            var webView = new Microsoft.Web.WebView2.WinForms.WebView2
            {
                Dock = DockStyle.Fill
            };
            form.Controls.Add(webView);

            form.Load += async (s, e) =>
            {
                try
                {
                    await webView.EnsureCoreWebView2Async(null);
                    var core = webView.CoreWebView2;

                    // 外链/新窗口交给系统默认浏览器
                    core.NewWindowRequested += (s2, e2) =>
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(
                                new System.Diagnostics.ProcessStartInfo(e2.Uri) { UseShellExecute = true });
                        }
                        catch { }
                        e2.Handled = true;
                    };

                    // 下载保持默认行为（浏览器下载条）
                    webView.Source = new Uri(AppUrl);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "WebView2 初始化失败：请确认系统已安装 Microsoft Edge WebView2 运行时。\n\n" + ex.Message,
                        "猫酱AI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    form.Close();
                }
            };

            Application.Run(form);
        }
    }
}
