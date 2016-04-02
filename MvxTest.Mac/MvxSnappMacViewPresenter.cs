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
using Foundation;

namespace MvxTest.Mac
{
	public enum WindowPresentationStyle
	{
		NewWindow,
		Modal,
		Sheet
	}

	public class MvxSnappMacViewPresenter
		: MvxBaseMacViewPresenter
	{
		private readonly NSApplicationDelegate _applicationDelegate;
		private readonly NSWindow _window;

		private Dictionary<NSWindow, Stack<NSViewController>> _windowViewControllers = new Dictionary<NSWindow, Stack<NSViewController>>();
		private Dictionary<IMvxViewModel, NSWindow> _vmWindowDictionary = new Dictionary<IMvxViewModel, NSWindow>();
		private NSWindow _presentedModal;
		private NSWindow _presentedSheet;

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

		public virtual CGRect GetRectForWindowWithViewController(NSViewController viewController, WindowPresentationStyle presentationStyle) {
			return this.Window?.Frame ?? new CGRect (200, 200, 600, 400);
		}

		public MvxSnappMacViewPresenter(NSApplicationDelegate applicationDelegate, NSWindow window)
		{
			this._applicationDelegate = applicationDelegate;
			this._window = window;

			window.WillClose += Window_WillClose;
			_windowViewControllers.Add (window, new Stack<NSViewController> ());
		}

		public override void Show(MvxViewModelRequest request)
		{
			var view = this.CreateView(request);

			this.Show(view, request);
		}

		public override void ChangePresentation(MvxPresentationHint hint)
		{
			if (hint is MvxClosePresentationHint) {
				this.Close ((hint as MvxClosePresentationHint).ViewModelToClose);
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

		protected virtual void ShowAsModal(NSViewController viewController, MvxViewModelRequest request)
		{
			if (_presentedModal != null) {
				Mvx.Exception ("Only one modal at a time is allowed!");
				return;
			}

			_presentedModal = new NSWindow (this.GetRectForWindowWithViewController(viewController, WindowPresentationStyle.Modal), NSWindowStyle.Titled, NSBackingStore.Buffered, false, NSScreen.MainScreen);
			if (!string.IsNullOrEmpty(viewController.Title)) {
				_presentedModal.Title = viewController.Title;
			}

			_presentedModal.ContentView = viewController.View;

			NSApplication.SharedApplication.RunModalForWindow (_presentedModal);
		}

		protected virtual void ShowInNewWindow(NSViewController viewController, MvxViewModelRequest request)
		{
			if (_presentedSheet != null) {
				Mvx.Exception ("Only one sheet at a time is allowed!");
				return;
			}

			var window = new NSWindow (this.GetRectForWindowWithViewController(viewController, WindowPresentationStyle.NewWindow), NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled, NSBackingStore.Buffered, false, NSScreen.MainScreen);

			window.WillClose += Window_WillClose;
			if (!string.IsNullOrEmpty(viewController.Title)) {
				window.Title = viewController.Title;
			}

			var stack = new Stack<NSViewController> ();
			stack.Push (viewController);

			window.ContentView = viewController.View;
			NSWindowController windowController = new NSWindowController (window);
			windowController.ShouldCascadeWindows = true;
			windowController.ShowWindow (null);

			_windowViewControllers.Add (window, stack);
			MvxViewController vc = viewController as MvxViewController;
			_vmWindowDictionary.Add (vc.ViewModel, window);
		}

		protected virtual void ShowInSheet(NSViewController viewController, MvxViewModelRequest request)
		{
			_presentedSheet = new NSWindow (this.GetRectForWindowWithViewController(viewController, WindowPresentationStyle.Sheet), NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled, NSBackingStore.Buffered, false, NSScreen.MainScreen);
			_presentedSheet.ContentView = viewController.View;

			NSApplication.SharedApplication.BeginSheet (_presentedSheet, NSApplication.SharedApplication.MainWindow);
		}

		void Window_WillClose (object sender, EventArgs e)
		{
			NSNotification notification = (NSNotification)sender;
			var window = (NSWindow)notification.Object;
			window.WillClose -= Window_WillClose;
			if (_windowViewControllers.ContainsKey (window)) {
				var stack = _windowViewControllers [window];

				// Remove all view controllers that may be in the stack of the window
				while (stack.Any()) {
					var vc = stack.Pop () as MvxViewController;
					if (vc != null && vc.ViewModel != null) {
						_vmWindowDictionary.Remove (vc.ViewModel);
					}
				}

				_windowViewControllers.Remove (window);
				stack.Clear ();
			}
		}

		protected virtual void Show(NSViewController viewController, MvxViewModelRequest request)
		{
			if (viewController is IMvxMacModalView) {
				this.ShowAsModal (viewController, request);
			} else if (viewController is IMvxMacNewWindowView) {
				this.ShowInNewWindow (viewController, request);
			} else if(viewController is IMvxMacSheetView) {
				this.ShowInSheet (viewController, request);
			} else {
				// If MainWindow is null, then we are properbly starting the app, and should select the Window we got offered
				var currentWindow = NSApplication.SharedApplication.MainWindow ?? Window;
				var stack = _windowViewControllers [currentWindow];
				stack.Push (viewController);

				if (!string.IsNullOrEmpty(viewController.Title)) {
					currentWindow.Title = viewController.Title;
				}

				currentWindow.ContentView = viewController.View;
				MvxViewController vc = viewController as MvxViewController;
				_vmWindowDictionary.Add (vc.ViewModel, currentWindow);
			}
		}

		public virtual void Close(IMvxViewModel toClose)
		{
			if (_presentedModal != null) {
				NSApplication.SharedApplication.StopModal ();
				_presentedModal.Close ();
				_presentedModal = null;
			} else if(_presentedSheet != null) {
				NSApplication.SharedApplication.EndSheet (_presentedSheet);
				_presentedSheet.Close ();
				_presentedSheet = null;
			} else {
				var window = _vmWindowDictionary [toClose];
				var stack = _windowViewControllers [window];
				if (stack.Count > 1) {
					stack.Pop ();
					var viewController = stack.Peek ();

					window.ContentView = viewController.View;
					if (!string.IsNullOrEmpty (viewController.Title)) {
						window.Title = viewController.Title;
					}

					_vmWindowDictionary.Remove (toClose);
				} else {
					stack.Clear();
					_windowViewControllers.Remove (window);

					window.WillClose -= Window_WillClose;
					window.Close ();
				}
			}
		}
	}

	public interface IMvxMacModalView
	{
		
	}

	public interface IMvxMacNewWindowView
	{

	}

	public interface IMvxMacSheetView
	{

	}
}

