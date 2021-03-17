﻿using System.Threading.Tasks;
using Adms.Shared.Attributes;
using ADMS.Apprentice.Core.Models;

namespace ADMS.Apprentice.Core.Services
{
    [RegisterWithIocContainer]
	public interface IApprenticeTFNRetreiver
	{
		ApprenticeTFNModel Get(int id);
	}
}