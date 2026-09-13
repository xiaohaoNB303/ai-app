package top.ai.com;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.webkit.WebView;
import android.webkit.WebViewClient;

/** 猫酱AI Android 壳：系统 WebView 加载与各端一致的网址 */
public class MainActivity extends Activity {

    private static final String HOST_SUFFIX = "workbuddy.host";
    private WebView web;

    @Override
    protected void onCreate(Bundle state) {
        super.onCreate(state);
        web = new WebView(this);
        web.getSettings().setJavaScriptEnabled(true);
        web.getSettings().setDomStorageEnabled(true);
        web.setWebViewClient(new WebViewClient() {
            @Override
            public boolean shouldOverrideUrlLoading(WebView view, String url) {
                try {
                    String host = Uri.parse(url).getHost();
                    if (url.startsWith("http") && host != null && !host.endsWith(HOST_SUFFIX)) {
                        startActivity(new Intent(Intent.ACTION_VIEW, Uri.parse(url)));
                        return true;
                    }
                } catch (Exception ignored) {
                }
                return false;
            }
        });
        web.setDownloadListener((url, userAgent, contentDisposition, mimetype, contentLength) ->
                startActivity(new Intent(Intent.ACTION_VIEW, Uri.parse(url))));
        setContentView(web);
        web.loadUrl("https://a15449a37364fd9b2.app.workbuddy.host");
    }

    @Override
    public void onBackPressed() {
        if (web != null && web.canGoBack()) web.goBack();
        else super.onBackPressed();
    }
}
