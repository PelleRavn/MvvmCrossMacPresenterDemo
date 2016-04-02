using MvvmCross.Core.ViewModels;
using MvvmCross.iOS.Platform;
using MvvmCross.Platform.Platform;
using MvxTest.Core;
using UIKit;

namespace MvxTest.iOS
{
	public class Setup : MvxIosSetup
	{
		public Setup (MvxApplicationDelegate applicationDelegate, UIWindow window) : base (applicationDelegate, window)
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
