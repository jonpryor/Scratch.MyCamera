using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Scratch.MyCamera
{
	// A mostly "standard" (with no error checking) Camera sample based on:
	// https://developer.android.com/guide/topics/media/camera.html
	[Activity (Label = "MyCamera", MainLauncher = true, Icon = "@mipmap/icon", ScreenOrientation = ScreenOrientation.Landscape)]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.Main);

			var camera  = GetCameraInstance ();
			var picture = new MyPictureCallback ();
			var p = new CameraPreview (this, camera);

			FindViewById<FrameLayout>(Resource.Id.camera_preview).AddView (p);
			FindViewById<Button>(Resource.Id.capture).Click += (sender, e) => {
				camera.TakeMyPicture (null, null, picture);
			};
		}

		static Camera GetCameraInstance ()
		{
			return Camera.Open ();
		}
	}

	class CameraPreview : SurfaceView, ISurfaceHolderCallback
	{

		ISurfaceHolder  mHolder;
		Camera          mCamera;

		public CameraPreview (Context context, Camera camera)
			: base (context)
		{
			mCamera = camera;
			mHolder = Holder;
			mHolder.AddCallback (this);
		}

		public void SurfaceChanged (ISurfaceHolder holder, [GeneratedEnum] Android.Graphics.Format format, int width, int height)
		{
			if (mHolder.Surface == null)
				return;
			mCamera.StopPreview ();
			mCamera.SetPreviewDisplay (mHolder);
			mCamera.StartPreview ();
		}

		public void SurfaceCreated (ISurfaceHolder holder)
		{
			mCamera.SetPreviewDisplay (mHolder);
			mCamera.StartPreview ();
		}

		public void SurfaceDestroyed (ISurfaceHolder holder)
		{
			// empty
		}
	}
}

