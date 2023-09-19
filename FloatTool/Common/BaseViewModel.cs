using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FloatTool.Common
{
	public abstract class BaseViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		public BaseViewModel()
		{

		}

		public void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
				return;

			field = value;
			OnPropertyChanged(propertyName);
		}
	}
}
