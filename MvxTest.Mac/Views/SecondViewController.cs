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
	[MvxViewFor(typeof(SecondViewModel))]
	public partial class SecondViewController : MvxViewController, IMvxMacModalView
	{
		#region Constructors

		// Called when created from unmanaged code
		public SecondViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SecondViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public SecondViewController () : base ("SecondView", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
			this.Title = "Vild modal!";
		}

		#endregion

		public override void ViewDidLoad ()
		{
			
			base.ViewDidLoad ();
			var set = this.CreateBindingSet<SecondViewController, SecondViewModel>();
			set.Bind(TextField).To(vm => vm.Hello);
			set.Bind(TextLabel).To(vm => vm.Hello);
			set.Bind(BackButton).To(vm => vm.BackCommand);
			set.Apply();
		}
	}
}
