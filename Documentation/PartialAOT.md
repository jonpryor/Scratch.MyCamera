# Partial AOT

An interesting question came up about AOT:

Enabling `$(AotAssemblies)` can result in large `.apk` sizes, as all assemblies
are AOT'd, and that can result in many large AOT native libraries. Would it be
possible to only AOT *some* assemblies?

Within the normal build system, no.

However, it can be hacked together.

## partial-aot.sh

Enter [`partial-aot.sh`](../partial-aot.sh), which is a shell script which does
the following:

1. Build a `.apk` with `$(AotAssemblies)` set to `False`, the "normal JIT"
    case, for comparison.

2. Build a `.apk` with `$(AotAssemblies)` set to `True`, AOT'ing assemblies.

3. Take an *intermediate `.apk`* from (2), and *remove* the "undesirable"
    native libraries, so that only a *subset* of the AOT information is
    kept within the .apk.

4. Use the internal `_Sign` target to sign (3).

This allows exploring a "partial AOT" scenario.

For this scenario, we *only* AOT the assemblies:

* `Java.Interop.dll`
* `Mono.Android.dll`
* `mscorlib.dll`
* `System.dll`


To demonstrate launch times, install, launch, and extract files for each variation:

* `scratch.mycamera-Signed+FullJIT.apk`: A "normal" Release app.
* `scratch.mycamera-Signed+AOT.apk`: A "normal" AOT app.
* `scratch.mycamera-Signed+PartialAOT.apk`:
    The result of the above hacked build process.

Times are measured on a Nexus 6P running Android 7.1.1, *first* run.
(No multiple app startups and averages.)

# JIT behavior

    $ adb uninstall scratch.mycamera
    $ adb install scratch.mycamera-Signed+FullJIT.apk
    
    # Run the app...
    $ adb logcat
    ...
    I ActivityManager: Displayed scratch.mycamera/md5dd3eb63369467f97725b9eea3119d868.MainActivity: +1s377ms

This particular app took ~1.4s to launch.

    $ adb shell run-as scratch.mycamera cat files/.__override__/counters.txt > counters-FullJIT.txt
    $ adb shell run-as scratch.mycamera cat files/.__override__/methods.txt > methods-FullJIT.txt

The app has been modified to set `debug.mono.log` to contain `timing`, which
creates the `counters.txt` and `methods.txt` files.

`counters.txt` contains JIT diagnostic information every time
`Runtime.register()` is invoked. It's not entirely ideal, but it's interesting.
Of primary interest is the `Total time spent JITting (sec)` value, which
is how much time has been spent JIT'ing, *cumulative*, since process startup.

`methods.txt` contains a list of every method that is JIT'd, in the order that it is JIT'd.

The final time in `counters-FullJIT.txt` is:

		Total time spent JITting (sec)      : 0.4724

`methods-FullJIT.txt` contains 1562 lines.

# AOT Behavior

    $ adb uninstall scratch.mycamera
    $ adb install scratch.mycamera-Signed+AOT.apk
    
    # Run the app...
    $ adb logcat
    ...
    I ActivityManager: Displayed scratch.mycamera/md5dd3eb63369467f97725b9eea3119d868.MainActivity: +862ms

Enabling AOT shortens startup time by 515ms, to ~63% of JIT startup time.

    $ adb shell run-as scratch.mycamera cat files/.__override__/counters.txt > counters-AOT.txt
    $ adb shell run-as scratch.mycamera cat files/.__override__/methods.txt > methods-AOT.txt

The final time in `counters-FullJIT.txt` is:

    Total time spent JITting (sec)      : 0.0337

Using AOT shortens JIT time by 0.43s, to ~7% of JIT time.

`methods-FullJIT.txt` contains 1186 lines.


# Partial AOT Behavior

    $ adb uninstall scratch.mycamera
    $ adb install scratch.mycamera-Signed+PartialAOT.apk
    
    # Run the app...
    $ adb logcat
    ...
    I ActivityManager: Displayed scratch.mycamera/md5dd3eb63369467f97725b9eea3119d868.MainActivity: +898ms

Additionally, and as importantly, is that the AOT information is being used:

		D Mono    : AOT: loaded AOT Module for System.dll.

Partial AOT shortens startup time by 479ms, to ~65% of JIT startup time.

    $ adb shell run-as scratch.mycamera cat files/.__override__/counters.txt > counters-PartialAOT.txt
    $ adb shell run-as scratch.mycamera cat files/.__override__/methods.txt > methods-PartialAOT.txt

The final time in `counters-PartialAOT.txt` is:

    Total time spent JITting (sec)      : 0.0471

Using AOT shortens JIT time by 0.43s, to ~10% of JIT time.

`methods-PartialAOT.txt` contains 1220 lines.
