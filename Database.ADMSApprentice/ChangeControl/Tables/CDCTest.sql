CREATE TABLE ChangeControl.CDCTest
(
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DESCRIPTION] [nvarchar](100) NULL,
	[SQL_TMST] [datetime] NULL,
	 CONSTRAINT [PK__ChangeControl__CDCTest] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)
)