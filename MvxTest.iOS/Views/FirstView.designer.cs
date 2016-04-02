// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MvxTest.iOS.Views
{
	[Register ("FirstView")]
	partial class FirstView
	{
		[Outlet]
		UIKit.UITextField InputField { get; set; }

		[Outlet]
		UIKit.UILabel TextLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (InputField != null) {
				InputField.Dispose ();
				InputField = null;
			}

			if (TextLabel != null) {
				TextLabel.Dispose ();
				TextLabel = null;
			}
		}
	}
}

