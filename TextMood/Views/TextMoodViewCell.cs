using System;
using System.Net;
using System.Linq;

using Xamarin.Forms;
using TextMood.Shared;

namespace TextMood
{
	public class TextMoodViewCell : ViewCell
	{
		#region Constant Fields
		readonly Label _titleLabel, _descriptionLabel;
		#endregion

		#region Constructors
		public TextMoodViewCell()
		{
			_titleLabel = new Label
			{
				FontAttributes = FontAttributes.Bold,
			};

			_descriptionLabel = new Label();

			var gridLayout = new Grid
			{
				Padding = new Thickness(20, 5),
				RowSpacing = 2,
				ColumnSpacing = 10,

				RowDefinitions = {
					new RowDefinition{ Height = new GridLength(0, GridUnitType.Auto) },
					new RowDefinition{ Height = new GridLength(0, GridUnitType.Auto) }

				},
				ColumnDefinitions = {
					new ColumnDefinition{ Width = new GridLength(0, GridUnitType.Auto) }
				}
			};

			gridLayout.Children.Add(_titleLabel, 0, 0);
			gridLayout.Children.Add(_descriptionLabel, 0, 1);

			View = gridLayout;
		}
		#endregion

		#region Methods
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			var textModel = BindingContext as TextMoodModel;

			var emoji = EmojiServices.GetEmoji(textModel.SentimentScore);

			_titleLabel.Text = textModel.Text;
			_descriptionLabel.Text = $"{emoji} {textModel.CreatedAt.ToLocalTime().ToString("g")}";
		}
		#endregion
	}
}