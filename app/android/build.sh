#!/usr/bin/env bash
# 猫酱AI Android 壳构建：aapt2 + javac + d8 + zipalign + apksigner
# 依赖：Android build-tools（aapt2/d8/zipalign/apksigner）与 android.jar
#   ANDROID_BUILD_TOOLS=.../build-tools ANDROID_JAR=.../android.jar bash build.sh
set -e
cd "$(dirname "$0")"
BT="${ANDROID_BUILD_TOOLS:?缺少 ANDROID_BUILD_TOOLS}"
AJ="${ANDROID_JAR:?缺少 ANDROID_JAR}"
KS="${ANDROID_KEYSTORE:-$HOME/.config/ai-android.keystore}"
PASS="${ANDROID_KEYSTORE_PASS:-aimaomi-666-cat}"
OUT="$(pwd)/AI-6.6.6.apk"
[ -f "$KS" ] || keytool -genkeypair -keystore "$KS" -alias aimaomi -keyalg RSA -keysize 2048 \
  -validity 10000 -storepass "$PASS" -keypass "$PASS" -dname "CN=猫酱AI,O=Carliy,C=CN"

rm -rf build && mkdir -p build/compiled build/classes
"$BT/aapt2" compile --dir res -o build/res.zip
"$BT/aapt2" link -o build/base.apk -I "$AJ" --manifest AndroidManifest.xml build/res.zip
javac -classpath "$AJ" -d build/classes $(find src -name '*.java')
"$BT/d8" --release --lib "$AJ" --output build $(find build/classes -name '*.class')
python3 - <<'PY'
import zipfile
z = zipfile.ZipFile('build/base.apk', 'a')
z.write('build/classes.dex', 'classes.dex')
z.close()
PY
"$BT/zipalign" -f 4 build/base.apk build/aligned.apk
"$BT/apksigner" sign --ks "$KS" --ks-pass "pass:$PASS" --key-pass "pass:$PASS" --out "$OUT" build/aligned.apk
"$BT/apksigner" verify "$OUT"
echo "已构建 $OUT"
