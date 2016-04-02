using Android.Content;
using MvvmCross.Droid.Platform;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.Platform;
using MvxTest.Core;

namespace MvxTest.Droid
{
	public class Setup : MvxAndroidSetup
	{
		public Setup (Context applicationContext) : base (applicationContext)
		{
		}

		protected override IMvxApplication CreateApp ()
		{
			return new App ();
		}

		protected override IMvxTrace CreateDebugTrace ()
		{
			return new DebugTrace ();
		}
	}
}
