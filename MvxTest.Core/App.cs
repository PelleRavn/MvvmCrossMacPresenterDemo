using MvvmCross.Platform.IoC;
using MvxTest.Core.ViewModels;

namespace MvxTest.Core
{
	public class App : MvvmCross.Core.ViewModels.MvxApplication
	{
		public override void Initialize ()
		{
			CreatableTypes ()
                .EndingWith ("Service")
                .AsInterfaces ()
                .RegisterAsLazySingleton ();
				
			RegisterAppStart<FirstViewModel> ();
		}
	}
}
