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
	public partial class NewWindowViewController : FirstViewController, IMvxMacNewWindowView
	{
		// Called when created from unmanaged code
		public NewWindowViewController (IntPtr handle) : base (handle)
		{
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public NewWindowViewController (NSCoder coder) : base (coder)
		{
		}

		// Call to load from the XIB/NIB file
		public NewWindowViewController ()
		{
		}
	}

	public partial class ModalViewController : FirstViewController, IMvxMacModalView
	{
		// Called when created from unmanaged code
		public ModalViewController (IntPtr handle) : base (handle)
		{
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ModalViewController (NSCoder coder) : base (coder)
		{
		}

		// Call to load from the XIB/NIB file
		public ModalViewController ()
		{
		}
	}

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
			this.Title = "First Window";
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var set = this.CreateBindingSet<FirstViewController, FirstViewModel> ();
			set.Bind (TextField).To (vm => vm.Hello);
			set.Bind (TextLabel).To (vm => vm.Hello);
			set.Bind (NextButton).To (vm => vm.GoNextCommand);
			set.Bind (OpenModalButton).To (vm => vm.ModalCommand);
			set.Bind (OpenSheetButton).To (vm => vm.SheetCommand);
			set.Bind (OpenWindowButton).To (vm => vm.NewWindowCommand);
			set.Bind (CloseButton).To (vm => vm.CloseCommand);
			set.Apply ();
		}
	}
}
