using System;
using MvvmCross.Core.ViewModels;
using System.Windows.Input;

namespace MvxTest.Core.ViewModels
{
	public class ThirdViewModel : MvxViewModel
	{
		public ThirdViewModel ()
		{

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
