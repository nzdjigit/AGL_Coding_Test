using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ApplicationLayer.Models;
using Newtonsoft.Json;

namespace ApplicationLayer.Services
{
    internal class OwnerAndPetsDataProcessor : IDataProcessor
    {
        private readonly HttpClient _httpClient;

        public OwnerAndPetsDataProcessor(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Dictionary<string, List<string>>> ProcessDataAsync(string dataSourceUrl)
        {
            if (string.IsNullOrWhiteSpace(dataSourceUrl))
            {
                throw new ArgumentException("A valid data source URL must be provided.");
            }

            var jsonResult = await _httpClient.GetStringAsync(dataSourceUrl);

            if (string.IsNullOrWhiteSpace(jsonResult))
            {
                throw new InvalidDataException("Data Source returned an empty result.");
            }

            var ownersAndPets = JsonConvert.DeserializeObject<List<Owner>>(jsonResult);

            return ProcessOwnersAndPetsData(ownersAndPets);
        }

        private Dictionary<string, List<string>> ProcessOwnersAndPetsData(List<Owner> ownersAndPetsData)
        {
            return ownersAndPetsData.Where(owner => owner.Pets != null)
                                    .GroupBy(owner => owner.Gender)
                                    .Select(grouping => new
                                    {
                                        Gender = grouping.Key,
                                        CatPets = grouping.SelectMany(owner => owner.Pets)
                                                                                    .Where(pet => pet.Type.Equals("cat", StringComparison.OrdinalIgnoreCase))
                                                                                    .OrderBy(pet => pet.Name)
                                    })
                                    .ToDictionary(v => v.Gender, v => v.CatPets.Select(cat => cat.Name).ToList());
        }
    }
}
