using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationLayer.Services
{
    public interface IDataProcessor
    {
        Task<Dictionary<string, List<string>>> ProcessDataAsync(string dataSourceUrl);
    }
}