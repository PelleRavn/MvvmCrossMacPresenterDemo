using System;
using MvvmCross.Core.ViewModels;
using System.Windows.Input;

namespace MvxTest.Core.ViewModels
{
	public class SecondViewModel : MvxViewModel
	{
		public SecondViewModel ()
		{

		}

		private string _hello;

		public string Hello { 
			get { return _hello; }
			set {
				_hello = value;
				RaisePropertyChanged (() => Hello);
			}
		}

		private MvxCommand _backCommand;

		public ICommand BackCommand {
			get {
				_backCommand = _backCommand ?? new MvxCommand (DoBackCommand);
				return _backCommand;
			}
		}

		private void DoBackCommand ()
		{
			Close (this);
		}
	}
}
