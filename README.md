# Example of manually binding Java interfaces.

A Xamarin.Android customer asks:

> our SDK streams video in real time and in doing so, it appears that
> marshalling byte arrays from Java to C# is causing GC thrashing in
> both Java/Mono. Is it possible to get direct access to the byte
> arrays or IntPtrs for the byte arrays so we can reuse buffers and
> come up with a single copy or no copy solution?

I'm not sure which exact interface they're using, so I pick at
(nearly) random the `Camera.IPictureCallback` interface:

        https://developer.xamarin.com/api/type/Android.Hardware.Camera+IPictureCallback/

The `Camera.IPictureCallback.OnPictureTaken()` method accepts a
`byte[]` parameter.

Assume, for demonstration purposes, that the default array marshaling
behavior was inadequate or non-performant. How can we improve matters?

For starters, build the [xamarin-android][0] repo. (This unfortunately
requires macOS at this point in time.) When the build completes, the
generated sources for `Mono.Android.dll` will be found in
`src/Mono.Android/obj/$(Configuration)/android-$(AndroidApiLevel)`,
e.g. `src/Mono.Android/obj/Debug/android-25` for API-25 bindings.
Within here will be the generated code for the Camera type, which
allows us to easily copy the existing `Camera.IPictureCallback`
interface and `Camera.IPictureCallbackInvoker` types.

These types are copied into
[`Scratch.MyCamera/ManualCamera.cs`](Scratch.MyCamera/ManualCamera.cs),
renaming `Camera.IPictureCallback` to `IMyPictureCallback`, and
Camera.IPictureCallbackInvoker` to `IMyPictureCallbackInvoker`.
Once copied, we change `IMyPictureCallback.OnPictureTaken()` to take a
`Java.Interop.JniObjectReference` instead of a `byte[]`.
(`JniObjectReference` is a struct which represents a JNI object
reference, either a JNI Local, Global, or WeakGLobal reference.)

Next, `IMyPictureCallbackInvoker` is updated to marshal the JNI
parameters as intended.

Finally, we add a `Camera.TakeMyPicture()` extension method which
takes `IMyPictureCallback` as a parameter type.

[0]: https://github.com/xamarin/xamarin-android/

