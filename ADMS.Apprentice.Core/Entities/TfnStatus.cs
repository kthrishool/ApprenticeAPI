using System.ComponentModel;

namespace ADMS.Apprentice.Core.Entities
{
	// These statuses would be configured in ADW
	public enum TFNStatus
	{
		[Description("To be verified")] 
		TBVE,
		[Description("Submitted for verification")]
		SBMT,
		[Description("Verified")] 
		MTCH,
		[Description("Verification failed no match")] 
		NOCH,
		[Description("Verification system error")] 
		TERR,
	}
}