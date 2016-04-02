// WARNING
//
// This file has been generated automatically by Xamarin Studio Indie to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MvxTest.Mac.Views
{
	[Register ("SecondViewController")]
	partial class SecondViewController
	{
		[Outlet]
		AppKit.NSButton BackButton { get; set; }

		[Outlet]
		AppKit.NSTextField TextField { get; set; }

		[Outlet]
		AppKit.NSTextField TextLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (TextField != null) {
				TextField.Dispose ();
				TextField = null;
			}

			if (TextLabel != null) {
				TextLabel.Dispose ();
				TextLabel = null;
			}

			if (BackButton != null) {
				BackButton.Dispose ();
				BackButton = null;
			}
		}
	}
}
