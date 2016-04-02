using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using ObjCRuntime;
using System.Windows.Input;
using AudioToolbox;
using CoreGraphics;

namespace MvxTest.iOS
{
	public partial class Name : UIView
	{
		public Name ()
		{
			Initialize ();
		}

		public Name (CGRect rect) : base (rect)
		{
			Initialize ();
		}

		public Name (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public Name (NSCoder coder) : base (coder)
		{
		}

		private UIView LoadView ()
		{
			try {
				var arr = NSBundle.MainBundle.LoadNib ("Name", null, null);
				var v = (UIView)Runtime.GetNSObject (arr.ValueAt (0));
				return v;
			} catch (Exception e) {
				Console.WriteLine (e.Message);
			}
			return null;
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			Initialize ();
		}

		public void Initialize ()
		{
			var view = LoadView ();
			if (view != null) {
				view.Frame = this.Bounds;
				this.AddSubview (view);
			}
		}

		//		public event EventHandler ExampleChanged;
		//		private int _example;
		//		public int Example
		//		{
		//			set {
		//				_example = value;
		//
		//				var handler = this.ExampleChanged;
		//				handler?.Invoke(this, EventArgs.Empty);
		//
		//			}
		//			get {
		//				return _example;
		//			}
		//		}
	}


}

