#!/usr/bin/env bash
# AI Linux 壳构建：二进制 + deb + AppImage
# 依赖：gcc、pkg-config、gtk3、webkit2gtk-4.1、dpkg-deb、appimagetool（AppImage 官方工具）
set -e
cd "$(dirname "$0")"

VERSION=6.6.6
APPIMAGE_TOOL="${APPIMAGE_TOOL:-appimagetool}"

# ---------- 二进制 ----------
gcc -O2 -Wall -o ai-app ai_wrap.c $(pkg-config --cflags --libs gtk+-3.0 webkit2gtk-4.1)
echo "已编译 ai-app"

make_desktop_file() {
    cat <<'EOF'
[Desktop Entry]
Type=Application
Name=猫酱AI
Comment=猫酱AI 桌面客户端
Exec=ai-app
Icon=ai-app
Terminal=false
Categories=Network;
EOF
}

# ---------- deb ----------
S=/tmp/ai-deb/ai-app_${VERSION}_amd64
rm -rf /tmp/ai-deb
mkdir -p $S/DEBIAN $S/usr/bin $S/usr/share/applications $S/usr/share/pixmaps
install -m755 ai-app $S/usr/bin/ai-app
install -m644 ../icons/icon.png $S/usr/share/pixmaps/ai-app.png
make_desktop_file > $S/usr/share/applications/ai-app.desktop
SIZE=$(du -sk $S/usr | cut -f1)
cat > $S/DEBIAN/control <<EOF
Package: ai-app
Version: $VERSION
Section: net
Priority: optional
Architecture: amd64
Depends: libgtk-3-0, libwebkit2gtk-4.1-0
Maintainer: xiaopi668 <xiaopi668@users.noreply.github.com>
Installed-Size: $SIZE
Description: AI 桌面客户端（Linux）
 网页套壳应用，使用系统自带 WebKitGTK WebView。
EOF
dpkg-deb --build --root-owner-group $S ai-app_${VERSION}_amd64.deb
echo "已构建 ai-app_${VERSION}_amd64.deb"

# ---------- AppImage ----------
A=/tmp/ai-appimage/AI.AppDir
rm -rf /tmp/ai-appimage
mkdir -p $A/usr/bin $A/usr/share/applications $A/usr/share/icons/hicolor/512x512/apps
install -m755 ai-app $A/usr/bin/ai-app
install -m644 ../icons/icon.png $A/usr/share/icons/hicolor/512x512/apps/ai-app.png
cp ../icons/icon.png $A/ai-app.png
make_desktop_file > $A/usr/share/applications/ai-app.desktop
cp $A/usr/share/applications/ai-app.desktop $A/ai-app.desktop
cat > $A/AppRun <<'EOF'
#!/bin/sh
HERE="$(dirname "$(readlink -f "$0")")"
exec "$HERE/usr/bin/ai-app" "$@"
EOF
chmod +x $A/AppRun
OUT="$(pwd)/ai-app_${VERSION}_linux-x64.AppImage"
(cd /tmp && "$APPIMAGE_TOOL" /tmp/ai-appimage/AI.AppDir "$OUT")
echo "已构建 ai-app_${VERSION}_linux-x64.AppImage"
