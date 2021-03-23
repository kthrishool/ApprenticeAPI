using System.Collections.Generic;
using System.Threading.Tasks;
using Adms.Shared.Paging;
using Refit;

namespace ADMS.Apprentice.Api.HttpClients.ReferenceDataApi
{
    public interface IReferenceDataClient
    {
        [Get("/public/Geospatial/Address/Autocomplete?partialAddress={partialAddress}&maximumRows={maximumRows}&formatSpecifier={formatSpecifier}&context={context}")]
        [Headers("Authorization: Bearer")]
        public Task<AutocompleteAddressModel[]> AutocompleteAddress(string partialAddress, int maximumRows = 10, string formatSpecifier = null, string context = null);

        [Get("/public/Geospatial/Address?addressId={addressId}&boundarySpecification={boundarySpecification}&formatSpecifier={formatSpecifier}&context={context}")]
        [Headers("Authorization: Bearer")]
        public Task<DetailAddressModel> GetDetailAddressById(int addressId, string boundarySpecification = null, string formatSpecifier = null, string context = null);
    }
}