using System;

using Android.Hardware;
using Android.Runtime;

using Java.Interop;

namespace Scratch.MyCamera
{
	// IMyPictureCallback is a "copy" of Camera.IPictureCallback,
	// using JniObjectReference instead of byte[]
	//
	// Note: The 3rd parameter here is the full class name of an "invoker" type; below.
	[Register ("android/hardware/Camera$PictureCallback", "", "Scratch.MyCamera.IMyPictureCallbackInvoker")]
	public partial interface IMyPictureCallback : IJavaObject
	{
		// Note: The third value contains an *assembly-qualified* class name of the "invoker" type.
		// If the assembly name changes, this string MUST be updated.
		[Register ("onPictureTaken", "([BLandroid/hardware/Camera;)V", "GetOnPictureTaken_arrayBLandroid_hardware_Camera_Handler:Scratch.MyCamera.IMyPictureCallbackInvoker, Scratch.MyCamera, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")]
		void OnPictureTaken (JniObjectReference data, Android.Hardware.Camera camera);

	}

	// "Normal" implementation of IMyPictureCallback
	public class MyPictureCallback : Java.Lang.Object, IMyPictureCallback
	{
		public MyPictureCallback ()
		{
		}

		public void OnPictureTaken (JniObjectReference data, Camera camera)
		{
			Console.WriteLine ($"# jonp: data: {data.ToString ()}");
			int length = JniEnvironment.Arrays.GetArrayLength (data);
			if (length == 0)
				return;

			// Note use of `unsafe` code to obtain the underlying Java bytes
			// without copying the entire array between VMs.
			unsafe {
				sbyte* bytes = JniEnvironment.Arrays.GetByteArrayElements (data, isCopy:null);
				JniEnvironment.Arrays.ReleaseByteArrayElements (data, bytes, 0);
			}
		}
	}

	static class CameraExtensions
	{
		// Extension method to call Camera.takePicture() using an IMyPictureCallback instance.
		public static unsafe void TakeMyPicture (this Camera camera, Camera.IShutterCallback shutter, Camera.IPictureCallback raw, IMyPictureCallback jpeg)
		{
			const string __id = "takePicture.(Landroid/hardware/Camera$ShutterCallback;Landroid/hardware/Camera$PictureCallback;Landroid/hardware/Camera$PictureCallback;)V";
			try {
				JniArgumentValue* __args = stackalloc JniArgumentValue [3];
				__args [0] = new JniArgumentValue ((shutter == null) ? IntPtr.Zero : ((global::Java.Lang.Object)shutter).Handle);
				__args [1] = new JniArgumentValue ((raw == null) ? IntPtr.Zero : ((global::Java.Lang.Object)raw).Handle);
				__args [2] = new JniArgumentValue ((jpeg == null) ? IntPtr.Zero : ((global::Java.Lang.Object)jpeg).Handle);
				camera.JniPeerMembers.InstanceMethods.InvokeNonvirtualVoidMethod (__id, camera, __args);
			} finally {
			}
		}
	}

	// Generated code + manual updates.
	[global::Android.Runtime.Register ("android/hardware/Camera$PictureCallback", DoNotGenerateAcw = true)]
	class IMyPictureCallbackInvoker : global::Java.Lang.Object, IMyPictureCallback
	{

		internal new static readonly JniPeerMembers _members = new JniPeerMembers ("android/hardware/Camera$PictureCallback", typeof (IMyPictureCallbackInvoker));

		static IntPtr java_class_ref {
			get { return _members.JniPeerType.PeerReference.Handle; }
		}

		public override global::Java.Interop.JniPeerMembers JniPeerMembers {
			get { return _members; }
		}

		protected override IntPtr ThresholdClass {
			get { return class_ref; }
		}

		protected override global::System.Type ThresholdType {
			get { return _members.ManagedPeerType; }
		}

		IntPtr class_ref;

		public static IMyPictureCallbackInvoker GetObject (IntPtr handle, JniHandleOwnership transfer)
		{
			return global::Java.Lang.Object.GetObject<IMyPictureCallbackInvoker> (handle, transfer);
		}

		static IntPtr Validate (IntPtr handle)
		{
			if (!JNIEnv.IsInstanceOf (handle, java_class_ref))
				throw new InvalidCastException (string.Format ("Unable to convert instance of type '{0}' to type '{1}'.",
							JNIEnv.GetClassNameFromInstance (handle), "android.hardware.Camera.PictureCallback"));
			return handle;
		}

		protected override void Dispose (bool disposing)
		{
			if (this.class_ref != IntPtr.Zero)
				JNIEnv.DeleteGlobalRef (this.class_ref);
			this.class_ref = IntPtr.Zero;
			base.Dispose (disposing);
		}

		public IMyPictureCallbackInvoker (IntPtr handle, JniHandleOwnership transfer)
			: base (Validate (handle), transfer)
		{
			IntPtr local_ref = JNIEnv.GetObjectClass (((global::Java.Lang.Object)this).Handle);
			this.class_ref = JNIEnv.NewGlobalRef (local_ref);
			JNIEnv.DeleteLocalRef (local_ref);
		}

		static Delegate cb_onPictureTaken_arrayBLandroid_hardware_Camera_;
#pragma warning disable 0169
		static Delegate GetOnPictureTaken_arrayBLandroid_hardware_Camera_Handler ()
		{
			if (cb_onPictureTaken_arrayBLandroid_hardware_Camera_ == null)
				cb_onPictureTaken_arrayBLandroid_hardware_Camera_ = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, IntPtr, IntPtr>)n_OnPictureTaken_arrayBLandroid_hardware_Camera_);
			return cb_onPictureTaken_arrayBLandroid_hardware_Camera_;
		}

		static void n_OnPictureTaken_arrayBLandroid_hardware_Camera_ (IntPtr jnienv, IntPtr native__this, IntPtr native_data, IntPtr native_camera)
		{
			IMyPictureCallback __this = global::Java.Lang.Object.GetObject<IMyPictureCallback> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			Camera camera = global::Java.Lang.Object.GetObject<Camera> (native_camera, JniHandleOwnership.DoNotTransfer);
			__this.OnPictureTaken (new JniObjectReference (native_data), camera);
		}
#pragma warning restore 0169

		IntPtr id_onPictureTaken_arrayBLandroid_hardware_Camera_;
		public unsafe void OnPictureTaken (JniObjectReference data, Camera camera)
		{
			if (id_onPictureTaken_arrayBLandroid_hardware_Camera_ == IntPtr.Zero)
				id_onPictureTaken_arrayBLandroid_hardware_Camera_ = JNIEnv.GetMethodID (class_ref, "onPictureTaken", "([BLandroid/hardware/Camera;)V");
			JValue* __args = stackalloc JValue [2];
			__args [0] = new JValue (data.Handle);
			__args [1] = new JValue ((camera == null) ? IntPtr.Zero : ((global::Java.Lang.Object)camera).Handle);
			JNIEnv.CallVoidMethod (((global::Java.Lang.Object)this).Handle, id_onPictureTaken_arrayBLandroid_hardware_Camera_, __args);
		}
	}
}
