CREATE TABLE [dbo].[PriorApprenticeshipQualification]
(
	[PriorApprenticeshipQualificationId] int IDENTITY(1,1) NOT NULL,
	[ApprenticeId] int NOT NULL,
	[ApprenticeshipId] varchar(15) NULL,
	[QualificationCode] varchar(10) NOT NULL,
	[QualificationDescription] varchar(200) NULL,
	[QualificationLevel] varchar(10) NULL,
	[QualificationANZSCOCode] varchar(10) NULL,
	[StartDate] date NULL,
	[EndDate] date NULL,
	[CountryCode] varchar(10) NULL,
	[StateCode] varchar(10) NULL,
	[CreatedBy] nvarchar(1024) NOT NULL,
	[CreatedOn] datetime2(7) NOT NULL,
	[UpdatedBy] nvarchar(1024) NOT NULL,
	[UpdatedOn] datetime2(7) NOT NULL,
	[Version] timestamp NOT NULL,
	[_AuditEventId] bigint NOT NULL,
	CONSTRAINT [PK_PriorApprenticeshipQualification] PRIMARY KEY CLUSTERED ([PriorApprenticeshipQualificationId] ASC),
	CONSTRAINT [FK_PriorApprenticeshipQualification_Apprentice] FOREIGN KEY ([ApprenticeId]) REFERENCES [dbo].[Apprentice] ([ApprenticeId])
);
GO
CREATE NONCLUSTERED INDEX [IX_PriorApprenticeshipQualification_ApprenticeId]
    ON [dbo].[PriorApprenticeshipQualification]([ApprenticeId] ASC) WITH (FILLFACTOR = 90);
