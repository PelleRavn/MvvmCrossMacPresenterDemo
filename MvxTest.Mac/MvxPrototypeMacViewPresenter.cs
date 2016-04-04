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

	public interface IMvxMacModalView
	{

	}

	public interface IMvxMacNewWindowView
	{

	}

	public interface IMvxMacSheetView
	{

	}

	public class MvxPrototypeMacViewPresenter
		: MvxBaseMacViewPresenter
	{
		private readonly NSApplicationDelegate _applicationDelegate;
		private readonly NSWindow _rootWindow;

		// Dictionary to know what View Controllers are inside of each open Window. Used when we need to present a View Controller for the current Window in focus
		private Dictionary<NSWindow, Stack<NSViewController>> _windowViewControllers = new Dictionary<NSWindow, Stack<NSViewController>>();
		// Dictionary to know what View Models are inside of each Window. Used when we Close(this) from a View Model
		private Dictionary<IMvxViewModel, NSWindow> _viewModelWindowDictionary = new Dictionary<IMvxViewModel, NSWindow>();

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
				return this._rootWindow;
			}
		}

		/// <summary>
		/// Overridable method to dictate the size and position of Windows that are about the be created
		/// </summary>
		/// <returns>The rect for the new window</returns>
		/// <param name="viewController">View Controller that will be presented in the Window</param>
		/// <param name="presentationStyle">Presentation style enum</param>
		public virtual CGRect GetRectForWindowWithViewController(NSViewController viewController, WindowPresentationStyle presentationStyle) {
			return this.Window?.Frame ?? new CGRect (200, 200, 600, 400);
		}

		public MvxPrototypeMacViewPresenter(NSApplicationDelegate applicationDelegate, NSWindow window)
		{
			this._applicationDelegate = applicationDelegate;
			this._rootWindow = window;

			// If we close the first Window, we wanna know!
			window.WillClose += Window_WillClose;

			// Adding the first Window to our dictionary, so it will be handled like every other Window we create
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

		protected virtual void Show(IMvxMacView view, MvxViewModelRequest request)
		{
			var viewController = view as NSViewController;
			if (viewController == null)
				throw new MvxException("The passed object is a IMvxMacView and not a NSViewController/MvxViewController, as it should be!");

			this.Show(viewController, request);
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
				// If NSApplication.MainWindow is null, then we are properbly starting the app, and should select the Window we got on creation time
				var currentWindow = NSApplication.SharedApplication.MainWindow ?? Window;

				// Getting the stack for the current Window we wanna present a view in
				var stack = _windowViewControllers [currentWindow];
				stack.Push (viewController);

				// Setting Title, if available
				if (!string.IsNullOrEmpty(viewController.Title)) {
					currentWindow.Title = viewController.Title;
				}

				// Setting current content view to window
				currentWindow.ContentView = viewController.View;

				// Getting ViewModel from ViewController
				MvxViewController vc = viewController as MvxViewController;
				_viewModelWindowDictionary.Add (vc.ViewModel, currentWindow);
			}
		}

		public virtual void Close(IMvxViewModel toClose)
		{
			if (_presentedModal != null) {
				// We should close the Modal

				NSApplication.SharedApplication.StopModal ();

				_presentedModal.Close ();
				_presentedModal = null;

			} else if(_presentedSheet != null) {
				// We should close the Sheet

				NSApplication.SharedApplication.EndSheet (_presentedSheet);

				_presentedSheet.Close ();
				_presentedSheet = null;

			} else {
				// "Normal" ViewController close

				// The Window where the current ViewModel/View Controller should be presented in
				var window = _viewModelWindowDictionary [toClose];
				var stack = _windowViewControllers [window];

				if (stack.Count > 1) {

					// Removing the top View Controller (because it's the current one)
					stack.Pop ();

					// Getting the one before the one we closed to present it
					var viewController = stack.Peek ();

					// Setting content view, and title for Window, if available
					window.ContentView = viewController.View;
					if (!string.IsNullOrEmpty (viewController.Title)) {
						window.Title = viewController.Title;
					}

					// Gone from ViewModel index
					_viewModelWindowDictionary.Remove (toClose);
				} else {
					// Last one? We will close the current Window
					stack.Clear();
					_windowViewControllers.Remove (window);

					window.WillClose -= Window_WillClose;
					window.Close ();
				}
			}
		}

		private IMvxMacView CreateView(MvxViewModelRequest request)
		{
			return Mvx.Resolve<IMvxMacViewCreator>().CreateView(request);
		}

		void Window_WillClose (object sender, EventArgs e)
		{
			// This is actually an NSConcreteNotification, but that's not a public API, so a NSNotification should be good enough
			NSNotification notification = (NSNotification)sender;

			var window = (NSWindow)notification.Object;

			// Remember to unsubscribe, to avoid people getting angry!
			window.WillClose -= Window_WillClose;

			// Make sure the window is not already gone!
			if (_windowViewControllers.ContainsKey (window)) {
				var stack = _windowViewControllers [window];

				// Remove all View Controllers that may be in the stack for the window
				while (stack.Any()) {

					// Getting the ViewModel from the ViewController
					var vc = stack.Pop () as MvxViewController;
					if (vc != null && vc.ViewModel != null) {

						// Removing it, to avoid leakes!
						_viewModelWindowDictionary.Remove (vc.ViewModel);
					}
				}

				_windowViewControllers.Remove (window);
				stack.Clear ();
			}
		}

		protected virtual void ShowAsModal(NSViewController viewController, MvxViewModelRequest request)
		{
			if (_presentedModal != null) {
				throw new MvxException ("Only one modal at a time is allowed!");
			}

			// The Window we will present the Modal as
			_presentedModal = new NSWindow (this.GetRectForWindowWithViewController(viewController, WindowPresentationStyle.Modal), NSWindowStyle.Titled, NSBackingStore.Buffered, false, NSScreen.MainScreen);

			// Setting Title if Window, if available
			if (!string.IsNullOrEmpty(viewController.Title)) {
				_presentedModal.Title = viewController.Title;
			}

			_presentedModal.ContentView = viewController.View;

			// Present the new modal Window with NSApplication
			NSApplication.SharedApplication.RunModalForWindow (_presentedModal);
		}

		protected virtual void ShowInNewWindow(NSViewController viewController, MvxViewModelRequest request)
		{
			if (_presentedSheet != null) {
				throw new MvxException ("Only one sheet at a time is allowed!");
			}

			var window = new NSWindow (this.GetRectForWindowWithViewController(viewController, WindowPresentationStyle.NewWindow), NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled, NSBackingStore.Buffered, false, NSScreen.MainScreen);

			// So we know, if the user closes the Window using the OS buttons, and not via a close-hint
			window.WillClose += Window_WillClose;

			// Setting Title, if available
			if (!string.IsNullOrEmpty(viewController.Title)) {
				window.Title = viewController.Title;
			}

			// The new stack view we need to save
			var stack = new Stack<NSViewController> ();
			stack.Push (viewController);

			window.ContentView = viewController.View;

			// WindowController to display the new Window
			NSWindowController windowController = new NSWindowController (window);
			windowController.ShouldCascadeWindows = true; // Attempt to change the size a bit of the Window, so we can see something changed
			windowController.ShowWindow (null);

			_windowViewControllers.Add (window, stack);

			// Getting ViewModel from ViewController
			MvxViewController vc = viewController as MvxViewController;
			_viewModelWindowDictionary.Add (vc.ViewModel, window);
		}

		protected virtual void ShowInSheet(NSViewController viewController, MvxViewModelRequest request)
		{
			// Creating Window for the Sheet presentation
			_presentedSheet = new NSWindow (this.GetRectForWindowWithViewController(viewController, WindowPresentationStyle.Sheet), NSWindowStyle.Resizable, NSBackingStore.Buffered, false, NSScreen.MainScreen);

			// Setting content view for the new Window
			_presentedSheet.ContentView = viewController.View;

			// Showing the Sheet with NSApplication. We can use MainWindow here, because we can't present a Sheet as the first opening Window
			NSApplication.SharedApplication.BeginSheet (_presentedSheet, NSApplication.SharedApplication.MainWindow);
		}
	}
}

