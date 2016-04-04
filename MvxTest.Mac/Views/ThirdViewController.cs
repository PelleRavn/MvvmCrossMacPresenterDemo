using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using MvvmCross.Mac.Views;
using MvvmCross.Binding.BindingContext;
using MvxTest.Core.ViewModels;

namespace MvxTest.Mac.Views
{
	public partial class ThirdViewController : MvxViewController, IMvxMacSheetView
	{
		#region Constructors

		// Called when created from unmanaged code
		public ThirdViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ThirdViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public ThirdViewController () : base ("ThirdView", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
			this.Title = "Sheet";
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var set = this.CreateBindingSet<ThirdViewController, ThirdViewModel>();
			set.Bind(CloseButton).To(vm => vm.CloseCommand);
			set.Apply();
		}

	}
}
