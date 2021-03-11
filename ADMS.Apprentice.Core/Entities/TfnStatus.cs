using System.ComponentModel;

namespace ADMS.Apprentice.Core.Entities
{
	// TODO These statuses would be configured in ADW
	public enum TFNStatus
	{
		[Description("New")]
		New,

		[Description("Awaiting verification")]
		Await,

		[Description("Verified")]
		Verified,

		[Description("Failed")]
		Failed
	}
}