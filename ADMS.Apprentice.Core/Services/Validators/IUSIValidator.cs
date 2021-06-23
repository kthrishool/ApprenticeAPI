﻿using System;
using ADMS.Apprentice.Core.Entities;
using Adms.Shared.Attributes;

namespace ADMS.Apprentice.Core.Services.Validators
{
    [RegisterWithIocContainer]
    public interface IUSIValidator
    {
        ValidationExceptionBuilder Validate(Profile profile);
    }
}