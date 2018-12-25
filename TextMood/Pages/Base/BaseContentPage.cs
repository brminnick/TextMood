﻿using Xamarin.Forms;

namespace TextMood
{
	public abstract class BaseContentPage<T> : ContentPage where T : BaseViewModel, new()
	{
		#region Constructors
		protected BaseContentPage() => BindingContext = ViewModel;
		#endregion

		#region Properties
		protected T ViewModel { get; } = new T();
		#endregion
	}
}
