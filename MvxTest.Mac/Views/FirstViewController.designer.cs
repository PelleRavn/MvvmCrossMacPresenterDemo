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
	[Register ("FirstViewController")]
	partial class FirstViewController
	{
		[Outlet]
		AppKit.NSButton CloseButton { get; set; }

		[Outlet]
		AppKit.NSButton NextButton { get; set; }

		[Outlet]
		AppKit.NSButton OpenModalButton { get; set; }

		[Outlet]
		AppKit.NSButton OpenWindowButton { get; set; }

		[Outlet]
		AppKit.NSTextField TextField { get; set; }

		[Outlet]
		AppKit.NSTextField TextLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (NextButton != null) {
				NextButton.Dispose ();
				NextButton = null;
			}

			if (OpenModalButton != null) {
				OpenModalButton.Dispose ();
				OpenModalButton = null;
			}

			if (OpenWindowButton != null) {
				OpenWindowButton.Dispose ();
				OpenWindowButton = null;
			}

			if (TextField != null) {
				TextField.Dispose ();
				TextField = null;
			}

			if (TextLabel != null) {
				TextLabel.Dispose ();
				TextLabel = null;
			}

			if (CloseButton != null) {
				CloseButton.Dispose ();
				CloseButton = null;
			}
		}
	}
}
