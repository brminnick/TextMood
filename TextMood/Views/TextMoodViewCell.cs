using Xamarin.Forms;
using TextMood.Shared;

namespace TextMood
{
    public class TextMoodDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => new TextMoodDataTemplate((ITextMoodModel)item);

        class TextMoodDataTemplate : DataTemplate
        {
            public TextMoodDataTemplate(ITextMoodModel textMoodModel) : base(() => CreateDataTemplate(textMoodModel))
            {

            }
            static Grid CreateDataTemplate(ITextMoodModel textModel)
            {
                var emoji = EmojiServices.GetEmoji(textModel.SentimentScore);

                var titleLabel = new Label
                {
                    FontAttributes = FontAttributes.Bold,
                    Text = textModel.Text
                };
                var descriptionLabel = new Label
                {
                    Text = $"{emoji} {textModel.CreatedAt.ToLocalTime().ToString("g")}"
                };

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

                gridLayout.Children.Add(titleLabel, 0, 0);
                gridLayout.Children.Add(descriptionLabel, 0, 1);

                return gridLayout;
            }
        }
    }
}