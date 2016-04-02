using System;
using MvvmCross.Mac.Views.Presenters;
using AppKit;
using MvvmCross.Core.ViewModels;
using MvvmCross.Mac.Views;
using MvvmCross.Platform;
using MvvmCross.Platform.Exceptions;
using System.Linq;
using System.Collections.Generic;
using CoreGraphics;

namespace MvxTest.Mac
{
	public class MvxSnappMacViewPresenter
		: MvxBaseMacViewPresenter
	{
		private readonly NSApplicationDelegate _applicationDelegate;
		private readonly NSWindow _window;

		private Stack<NSViewController> _viewControllers = new Stack<NSViewController> ();
		private NSWindow _presentedModal;

		protected virtual NSApplicationDelegate ApplicationDelegate
		{
			get
			{
				return this._applicationDelegate;
			}
		}

		protected virtual NSWindow Window
		{
			get
			{
				return this._window;
			}
		}

		public MvxSnappMacViewPresenter(NSApplicationDelegate applicationDelegate, NSWindow window)
		{
			this._applicationDelegate = applicationDelegate;
			this._window = window;
		}

		public override void Show(MvxViewModelRequest request)
		{
			var view = this.CreateView(request);

			this.Show(view, request);
		}

		public override void ChangePresentation(MvxPresentationHint hint)
		{
			if (hint is MvxClosePresentationHint)
			{
				this.Close((hint as MvxClosePresentationHint).ViewModelToClose);
				return;
			}

			base.ChangePresentation(hint);
		}

		private IMvxMacView CreateView(MvxViewModelRequest request)
		{
			return Mvx.Resolve<IMvxMacViewCreator>().CreateView(request);
		}

		protected virtual void Show(IMvxMacView view, MvxViewModelRequest request)
		{
			var viewController = view as NSViewController;
			if (viewController == null)
				throw new MvxException("Passed in IMvxMacView is not a UIViewController");

			this.Show(viewController, request);
		}

		protected virtual void Show(NSViewController viewController, MvxViewModelRequest request)
		{
			if (viewController is IMvxMacModalView) {
				_presentedModal = new NSWindow (new CGRect (200, 200, 600, 400), NSWindowStyle.Titled | NSWindowStyle.Closable, NSBackingStore.Buffered, false, NSScreen.MainScreen);
				if (!string.IsNullOrEmpty(viewController.Title)) {
					_presentedModal.Title = viewController.Title;
				}

				_presentedModal.ContentView = viewController.View;

				NSApplication.SharedApplication.RunModalForWindow (_presentedModal);
			} else {
				
				_viewControllers.Push (viewController);

				if (!string.IsNullOrEmpty(viewController.Title)) {
					this.Window.Title = viewController.Title;
				}

				this.Window.ContentView = viewController.View;
			}
		}

		public virtual void Close(IMvxViewModel toClose)
		{
			if (_presentedModal != null) {
				NSApplication.SharedApplication.StopModal();
				_presentedModal.Close ();
				_presentedModal = null;
			} else {
				if (_viewControllers.Count > 1) {
					_viewControllers.Pop ();
					var viewController = _viewControllers.Peek ();

					this.Window.ContentView = viewController.View;
				}
			}
		}
	}

	public interface IMvxMacModalView
	{
		
	}

	public interface IMvxMacInWindowView
	{

	}
}

