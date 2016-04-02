using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using MvvmCross.Mac.Views;
using MvvmCross.Binding.BindingContext;
using MvxTest.Core.ViewModels;
using MvvmCross.Core.ViewModels;

namespace MvxTest.Mac.Views
{
	[MvxViewFor(typeof(FirstViewModel))]
	public partial class FirstViewController : MvxViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public FirstViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public FirstViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public FirstViewController () : base ("FirstView", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var set = this.CreateBindingSet<FirstViewController, FirstViewModel> ();
			set.Bind (NextButton).To (vm => vm.GoNextCommand);
			set.Apply ();
		}
	}
}
