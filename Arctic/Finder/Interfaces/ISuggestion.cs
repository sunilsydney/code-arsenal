using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arctic.Finder.Interfaces
{
    public interface ISuggestion
    {
        Task<List<string>> GetSuggestions(string phrase);
    }
}
