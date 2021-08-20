CREATE TABLE [dbo].[PriorApprenticeshipQualification]
(
	[PriorApprenticeshipQualificationId]     INT IDENTITY(1,1) NOT NULL,
	[ApprenticeId]                           INT NOT NULL,
	[EmployerName]                           VARCHAR(200) NOT NULL,
	[QualificationCode]                      VARCHAR(10) NOT NULL,
	[QualificationDescription]               VARCHAR(200) NOT NULL,
	[QualificationLevel]                     VARCHAR(10) NOT NULL,
	[QualificationANZSCOCode]                VARCHAR(10) NOT NULL,
	[StartDate]                              DATE NOT NULL,
	[CountryCode]                            VARCHAR(10) NOT NULL,
	[StateCode]                              VARCHAR(10) NULL,
	[ApprenticeshipReference]                VARCHAR(30) NULL,
	[CreatedBy]                              NVARCHAR(1024) NOT NULL,
	[CreatedOn]                              DATETIME2(7) NOT NULL,
	[UpdatedBy]                              NVARCHAR(1024) NOT NULL,
	[UpdatedOn]                              DATETIME2(7) NOT NULL,
	[Version]                                TIMESTAMP NOT NULL,
	[_AuditEventId]                          BIGINT NOT NULL,
  
	CONSTRAINT [PK_PriorApprenticeshipQualification] PRIMARY KEY CLUSTERED ([PriorApprenticeshipQualificationId] ASC),
	CONSTRAINT [FK_PriorApprenticeshipQualification_Apprentice] FOREIGN KEY ([ApprenticeId]) REFERENCES [dbo].[Apprentice] ([ApprenticeId])
);
GO
CREATE NONCLUSTERED INDEX [IX_PriorApprenticeshipQualification_ApprenticeId]
    ON [dbo].[PriorApprenticeshipQualification]([ApprenticeId] ASC) WITH (FILLFACTOR = 90);
