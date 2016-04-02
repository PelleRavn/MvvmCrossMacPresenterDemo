using MvvmCross.Core.ViewModels;
using System.Windows.Input;
using System;

namespace MvxTest.Core.ViewModels
{
	public class NewWindowViewModel : FirstViewModel
	{
		public NewWindowViewModel () : base ()
		{

		}
	}

	public class ModalViewModel : FirstViewModel
	{
		public ModalViewModel () : base ()
		{

		}
	}

	public class FirstViewModel : MvxViewModel
	{
		public FirstViewModel ()
		{
			Hello = Guid.NewGuid ().ToString ();
		}

		private string _hello;

		public string Hello { 
			get { return _hello; }
			set {
				_hello = value;
				RaisePropertyChanged (() => Hello);
			}
		}

		private MvxCommand _goNextCommand;

		public ICommand GoNextCommand {
			get {
				_goNextCommand = _goNextCommand ?? new MvxCommand (DoGoNextCommand);
				return _goNextCommand;
			}
		}

		private void DoGoNextCommand ()
		{
			ShowViewModel<FirstViewModel> ();
		}

		private MvxCommand _newWindowCommand;

		public ICommand NewWindowCommand {
			get {
				_newWindowCommand = _newWindowCommand ?? new MvxCommand (DoNewWindowCommand);
				return _newWindowCommand;
			}
		}

		private void DoNewWindowCommand ()
		{
			ShowViewModel<NewWindowViewModel> ();
		}

		private MvxCommand _modalCommand;

		public ICommand ModalCommand {
			get {
				_modalCommand = _modalCommand ?? new MvxCommand (DoModalCommand);
				return _modalCommand;
			}
		}

		private void DoModalCommand ()
		{
			ShowViewModel<SecondViewModel> ();
		}

		private MvxCommand _closeCommand;

		public ICommand CloseCommand {
			get {
				_closeCommand = _closeCommand ?? new MvxCommand (DoCloseCommand);
				return _closeCommand;
			}
		}

		private void DoCloseCommand ()
		{
			Close (this);
		}
	}
}

