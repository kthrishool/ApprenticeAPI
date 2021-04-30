﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace ADMS.Apprentice.Core.HttpClients.ReferenceDataApi
{
    [ExcludeFromCodeCoverage]
    public record Boundary(
        string Name,
        string Value
        );    
}
