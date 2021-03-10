using System.ComponentModel;

namespace ADMS.Apprentice.Core.Entities
{
	public enum TfnStatus
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