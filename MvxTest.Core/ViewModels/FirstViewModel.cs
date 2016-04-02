using MvvmCross.Core.ViewModels;
using System.Windows.Input;

namespace MvxTest.Core.ViewModels
{
	public class FirstViewModel : MvxViewModel
	{
		private string _hello = "Hello MvvmCross";

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
			ShowViewModel<SecondViewModel> ();
		}
	}
}

