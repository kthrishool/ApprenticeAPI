﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Adms.Shared.Paging;
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

    }
}