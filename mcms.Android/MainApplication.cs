using System;
using Android.App;
using Android.Runtime;
using mcms.Helpers;

namespace mcms.Droid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
	[Application(Debuggable = false)]
#endif
	[MetaData("com.google.android.maps.v2.API_KEY",
			  Value = GoogleMapsApikey.ApiKey)]
	public class MainApplication : Application
	{
		public MainApplication(IntPtr handle, JniHandleOwnership transer)
		  : base(handle, transer)
		{
		}

		public override void OnCreate()
		{
			base.OnCreate();
		}
	}
}
