#!/bin/bash -e

MSBUILD="${MSBUILD:-xbuild}"

PROJECT="Scratch.MyCamera/Scratch.MyCamera.csproj"
APK_SIGNED="Scratch.MyCamera/bin/Release/scratch.mycamera-Signed.apk"
APK_UNSIGNED="Scratch.MyCamera/obj/Release/android/bin/scratch.mycamera.apk"

AOT_REMOVE_LIBS="
	lib/armeabi-v7a/libaot-Scratch.MyCamera.dll.so
	lib/armeabi-v7a/libaot-System.Xml.dll.so
	lib/armeabi-v7a/libaot-System.Core.dll.so
	lib/armeabi-v7a/libaot-System.Runtime.Serialization.dll.so
	lib/armeabi-v7a/libaot-System.ServiceModel.Internals.dll.so
	"

"$MSBUILD" $MSBUILD_FLAGS /p:Configuration=Release /t:SignAndroidPackage "$PROJECT"
cp "$APK_SIGNED" "scratch.mycamera-Signed+FullJIT.apk"


"$MSBUILD" $MSBUILD_FLAGS /p:Configuration=Release /t:SignAndroidPackage /p:AotAssemblies=True "$PROJECT"
mv "$APK_SIGNED" "scratch.mycamera-Signed+AOT.apk"

for lib in $AOT_REMOVE_LIBS ; do
	zip -d "$APK_UNSIGNED" "$lib"
done


"$MSBUILD" $MSBUILD_FLAGS /p:Configuration=Release /t:_Sign /p:AotAssemblies=True "$PROJECT"
cp "$APK_SIGNED" "scratch.mycamera-Signed+PartialAOT.apk"
