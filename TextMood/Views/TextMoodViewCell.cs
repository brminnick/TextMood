using TextMood.Shared;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;
using static Xamarin.CommunityToolkit.Markup.GridRowsColumns;

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

            enum Row { Title, Description }

            static Grid CreateDataTemplate(ITextMoodModel textModel) => new()
            {
                Padding = new Thickness(20, 5),
                RowSpacing = 2,
                ColumnSpacing = 10,

                RowDefinitions = Rows.Define(
                        (Row.Title, Auto),
                        (Row.Description, Auto)),

                Children =
                {
                    new Label { Text = textModel.Text }.Font(bold: true)
                        .Row(Row.Title),

                    new Label { Text = $"{EmojiServices.GetEmoji(textModel.SentimentScore)} {textModel.CreatedAt.ToLocalTime():g}" }
                        .Row(Row.Description)

                }
            };
        }
    }
}