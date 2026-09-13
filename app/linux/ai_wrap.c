/* AI 桌面壳 · Linux（WebKitGTK，使用系统自带 WebView，无浏览器界面） */
#include <gtk/gtk.h>
#include <webkit2/webkit2.h>

#define APP_URL   "https://a15449a37364fd9b2.app.workbuddy.host"
#define APP_TITLE "猫酱AI"

static void on_title_changed(WebKitWebView *view, GParamSpec *pspec, GtkWindow *win) {
    const gchar *title = webkit_web_view_get_title(view);
    if (title && *title) gtk_window_set_title(win, title);
}

/* 新窗口/外链一律交给系统默认浏览器打开 */
static gboolean on_decide_policy(WebKitWebView *view, WebKitPolicyDecision *decision,
                                 WebKitPolicyDecisionType type, gpointer user_data) {
    if (type == WEBKIT_POLICY_DECISION_TYPE_NEW_WINDOW_ACTION) {
        WebKitNavigationPolicyDecision *nd = WEBKIT_NAVIGATION_POLICY_DECISION(decision);
        WebKitNavigationAction *action = webkit_navigation_policy_decision_get_navigation_action(nd);
        WebKitURIRequest *req = webkit_navigation_action_get_request(action);
        const gchar *uri = req ? webkit_uri_request_get_uri(req) : NULL;
        if (uri && g_str_has_prefix(uri, "http")) {
            g_app_info_launch_default_for_uri_async(uri, NULL, NULL, NULL, NULL);
        }
        webkit_policy_decision_ignore(decision);
        return TRUE;
    }
    return FALSE;
}

int main(int argc, char **argv) {
    gtk_init(&argc, &argv);

    GtkWindow *win = GTK_WINDOW(gtk_window_new(GTK_WINDOW_TOPLEVEL));
    gtk_window_set_title(win, APP_TITLE);
    gtk_window_set_default_size(win, 1200, 820);
    {
        const gchar *appdir = g_getenv("APPDIR");
        gchar *icon = NULL;
        if (appdir && *appdir)
            icon = g_build_filename(appdir, "usr/share/icons/hicolor/512x512/apps/ai-app.png", NULL);
        else
            icon = g_strdup("/usr/share/pixmaps/ai-app.png");
        gtk_window_set_icon_from_file(win, icon, NULL);
        g_free(icon);
    }

    WebKitWebView *view = WEBKIT_WEB_VIEW(webkit_web_view_new());
    gtk_container_add(GTK_CONTAINER(win), GTK_WIDGET(view));

    g_signal_connect(view, "notify::title", G_CALLBACK(on_title_changed), win);
    g_signal_connect(view, "decide-policy", G_CALLBACK(on_decide_policy), NULL);
    g_signal_connect(win, "destroy", G_CALLBACK(gtk_main_quit), NULL);

    webkit_web_view_load_uri(view, APP_URL);
    gtk_widget_show_all(GTK_WIDGET(view));
    gtk_widget_show(GTK_WIDGET(win));
    gtk_main();
    return 0;
}
