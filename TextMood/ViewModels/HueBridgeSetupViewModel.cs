using System;
namespace TextMood
{
	public class HueBridgeSetupViewModel : BaseViewModel
	{
		#region Fields
		bool _isSaveButtonEnabled;
		string _bridgeIPEntryText;
		#endregion

		#region Properties
		public bool IsSaveButtonEnabled
		{
			get => _isSaveButtonEnabled;
			set => SetProperty(ref _isSaveButtonEnabled, value);
		}

		public string BridgeIPEntryText
		{
			get => _bridgeIPEntryText;
			set => SetProperty(ref _bridgeIPEntryText, value, () => IsSaveButtonEnabled = IsValidIPAddress(value));
		}
		#endregion

		#region Methods
		bool IsValidIPAddress(string text)
		{
			try
			{
				var indexOfFirstDecimal = text.IndexOf('.');
				var indexOfSecondDecimal = text.IndexOf('.', indexOfFirstDecimal + 1);
				var indexOfThirdDecimal = text.IndexOf('.', indexOfSecondDecimal + 1);

				var firstOctetText = text.Substring(0, indexOfFirstDecimal);
				var isFirstOctectTextAnInt = int.TryParse(firstOctetText, out int firstOctetInt);

				var secondOctetText = text.Substring(indexOfFirstDecimal + 1, indexOfSecondDecimal - indexOfFirstDecimal - 1);
				var isSecondOctectTextAnInt = int.TryParse(secondOctetText, out int secondOctetInt);

				var thirdOctetText = text.Substring(indexOfSecondDecimal + 1, indexOfThirdDecimal - indexOfSecondDecimal - 1);
				var isThirdOctectTextAnInt = int.TryParse(thirdOctetText, out int thirdOctetInt);

				var fourthOctetText = text.Substring(indexOfThirdDecimal + 1);
				var isFourthOctectTextAnInt = int.TryParse(fourthOctetText, out int fourthOctetInt);

				if (!isFirstOctectTextAnInt || !isSecondOctectTextAnInt || !isThirdOctectTextAnInt || !isFourthOctectTextAnInt)
					return false;

				if (IsNumberBetween0And255(firstOctetInt) && IsNumberBetween0And255(secondOctetInt) && IsNumberBetween0And255(thirdOctetInt) && IsNumberBetween0And255(fourthOctetInt))
					return true;

				return false;
			}
			catch (Exception)
			{
				return false;
			}

			bool IsNumberBetween0And255(int number) => number >= 0 && number <= 255;
		}
		#endregion
	}
}
