using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace TextMood
{
    public interface ITextModelApi
    {
        [Get("/GetTextModels")]
        Task<List<TextMoodModel>> GetTextModels();
    }
}
