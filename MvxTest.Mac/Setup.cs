using AppKit;
using Foundation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Mac.Platform;
using MvvmCross.Mac.Views.Presenters;
using MvvmCross.Platform.Platform;
using MvxTest.Core;

namespace MvxTest.Mac
{
    public class Setup : MvxMacSetup
    {
        public Setup(MvxApplicationDelegate applicationDelegate, NSWindow window)
            : base(applicationDelegate, window)
        {
        }

        protected override IMvxApplication CreateApp ()
        {
            return new App();
        }
        
        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }

		protected override IMvxMacViewPresenter CreatePresenter ()
		{
			return new MvxPrototypeMacViewPresenter (this.ApplicationDelegate, this.Window);
		}
    }
}
