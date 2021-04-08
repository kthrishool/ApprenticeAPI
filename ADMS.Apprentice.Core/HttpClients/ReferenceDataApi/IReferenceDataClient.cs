using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace ADMS.Apprentice.Core.HttpClients.ReferenceDataApi
{
    public interface IReferenceDataClient
    {
        [Get("/public/Geospatial/Address/Autocomplete?partialAddress={partialAddress}&maximumRows={maximumRows}&formatSpecifier={formatSpecifier}&context={context}")]
        [Headers("Authorization: Bearer")]
        public Task<AutocompleteAddressModel[]> AutocompleteAddress(string partialAddress, int maximumRows = 10, string formatSpecifier = null, string context = null);

        [Get("/public/Geospatial/Address/GetByAddressId?addressId={addressId}&boundarySpecification={boundarySpecification}&formatSpecifier={formatSpecifier}&context={context}")]
        [Headers("Authorization: Bearer")]
        public Task<DetailAddressModel> GetDetailAddressById(int addressId, string boundarySpecification = null, string formatSpecifier = null, string context = null);

        [Get("/public/Geospatial/PartialAddress/GetByFormattedLocality?formattedLocality={formattedLocality}&boundarySpecification={boundarySpecification}")]
        [Headers("Authorization: Bearer")]
        public Task<PartialAddressModel> GetAddressByFormattedLocality(string formattedLocality, string boundarySpecification = null);

        [Get("/public/Geospatial/Address/GetByFormattedAddress?formattedAddress={formattedAddress}&boundarySpecification={boundarySpecification}&formatSpecifier={formatSpecifier}&context={context}&locality={locality}&postCode={postCode}&state={state}")]
        [Headers("Authorization: Bearer")]
        public Task<DetailAddressModel> GetDetailAddressByFormattedAddress(string formattedAddress, string boundarySpecification = null, string formatSpecifier = null, string context = null, string locality = null, string postCode = null, string state = null);


        //[Get("/RelatedCodes/GetRelatedCodes?RelatedCodeType={RelatedCodeType}&SearchCode={SearchCode}&DominantSearch={DominantSearch}&CurrentCodesOnly={CurrentCodesOnly}&ExactLookup={ExactLookup}&MaxRows={MaxRows}&RowPosition={RowPosition}&CurrentDate={CurrentDate}&EndDateInclusive={EndDateInclusive}")]
        //[Headers("Authorization: Bearer")]
        //public Task<RelatedCodeModel[]> GetRelatedCodes(string RelatedCodeType, string SearchCode = "", bool DominantSearch = true, bool CurrentCodesOnly = true, bool ExactLookup = false, int MaxRows = 0, int RowPosition = 0, string CurrentDate = "", bool? EndDateInclusive = null);

        [Get("/ListCodes?CodeType={CodeType}&StartingCode={StartingCode}&CurrentCodesOnly={CurrentCodesOnly}&ExactLookup={ExactLookup}&MaxRows={MaxRows}&CurrentDate={CurrentDate}&EndDateInclusive={EndDateInclusive}")]
        [Headers("Authorization: Bearer")]
        public Task<IList<ListCodeResponseV1>> GetListCodes(string CodeType, string StartingCode = "", bool CurrentCodesOnly = true, bool ExactLookup = false, int MaxRows = 0, string CurrentDate = "", bool? EndDateInclusive = null);

        [Get("/RelatedCodes/PostcodeLocality?PostCode={PostCode}")] //PostcodeLocality
        [Headers("Authorization: Bearer")]
        public Task<PostcodeLocality[]> GetRelatedCodesByPostCode(string PostCode);
    }
}