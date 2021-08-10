CREATE TABLE [dbo].[Apprentice] (
    [ApprenticeId]                    INT             IDENTITY (1, 1) NOT NULL,
    [TitleCode]                       VARCHAR (10)    NULL,
    [Surname]                         VARCHAR (50)    NOT NULL,
    [FirstName]                       VARCHAR (50)    NOT NULL,
    [OtherNames]                      VARCHAR (50)    NULL,
    [PreferredName]                   VARCHAR (50)    NULL,
    [GenderCode]                      VARCHAR (10)    NULL,
    [BirthDate]                       DATE            NOT NULL,
    [EmailAddress]                    VARCHAR (320)   NULL,
    [PreferredContactType]            VARCHAR (10)    NULL,
    [SelfAssessedDisabilityCode]      VARCHAR (10)    NULL,
    [IndigenousStatusCode]            VARCHAR (10)    NULL,
    [CitizenshipCode]                 VARCHAR (10)    NULL,
    [HighestCompletedSchoolLevelCode] VARCHAR (10)    NULL,
    [LeftSchoolDate]                  DATE            NULL,
    [ProfileTypeCode]                 VARCHAR (10)    NOT NULL,
    [VisaNumber]                      VARCHAR (25)    NULL,
    [CountryOfBirthCode]              VARCHAR (10)    NULL,
    [LanguageCode]                    VARCHAR (10)    NULL,
    [InterpretorRequiredFlag]         BIT             NULL,
    [CustomerReferenceNumber]         VARCHAR (10)    NULL,
    [NotPovidingUSIReasonCode]        VARCHAR (10)    NULL,  
    [NewApprenticeId]                 INT             NULL,
    [DuplicateDate]                   DATETIME2 (0)   NULL,
    [DuplicateApprovedBy]             NVARCHAR (1024) NULL,
    [DeceasedFlag]                    BIT             NOT NULL,
    [ActiveFlag]                      BIT             NOT NULL,
    [InactiveDate]                    DATETIME2 (0)   NULL,
    [CreatedBy]                       NVARCHAR (1024) NOT NULL,
    [CreatedOn]                       DATETIME2 (7)   NOT NULL,
    [UpdatedBy]                       NVARCHAR (1024) NOT NULL,
    [UpdatedOn]                       DATETIME2 (7)   NOT NULL,
    [Version]                         ROWVERSION      NOT NULL,
    [_AuditEventId]                   BIGINT          NOT NULL,
    CONSTRAINT [PK_Apprentice] PRIMARY KEY CLUSTERED ([ApprenticeId] ASC),
    CONSTRAINT [CHK_Apprentice_BirthDate] CHECK ([BirthDate]>='1900-01-01' AND [BirthDate]<='2079-06-06'),
    CONSTRAINT [CHK_Apprentice_DuplicateDate] CHECK ([DuplicateDate]>='1900-01-01' AND [DuplicateDate]<='2079-06-06'),
    CONSTRAINT [CHK_Apprentice_InActiveDate] CHECK ([InActiveDate]>='1900-01-01' AND [InActiveDate]<='2079-06-06'),
    CONSTRAINT [CHK_Apprentice_CreatedOn] CHECK ([CreatedOn]>='1753-01-01'),
    CONSTRAINT [CHK_Apprentice_UpdatedOn] CHECK ([UpdatedOn]>='1753-01-01'),
    CONSTRAINT [FK_Apprentice_Apprentice] FOREIGN KEY ([NewApprenticeId]) REFERENCES [dbo].[Apprentice] ([ApprenticeId])
);








GO
CREATE NONCLUSTERED INDEX [IX_Apprentice_ActiveFlag]
    ON [dbo].[Apprentice]([ActiveFlag] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Apprentice_BirthDate]
    ON [dbo].[Apprentice]([BirthDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Apprentice_CustomerReferenceNumber]
    ON [dbo].[Apprentice]([CustomerReferenceNumber] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Apprentice_DeceasedFlag]
    ON [dbo].[Apprentice]([DeceasedFlag] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Apprentice_FirstName]
    ON [dbo].[Apprentice]([FirstName] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Apprentice_NewApprenticeId]
    ON [dbo].[Apprentice]([NewApprenticeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Apprentice_Surname]
    ON [dbo].[Apprentice]([Surname] ASC);


GO

 
CREATE TRIGGER [dbo].[trg_Apprentice_FTMaintenance] ON [dbo].[Apprentice] 
AFTER UPDATE,INSERT 
AS  
BEGIN 
	SET NOCOUNT ON; 
 
/*-------------------------------------------------------------------------------- 
Based upon EMP.JobSeeker.[trg_JobSeeker_FTMaintenance] by Mick Vullo 
----------------------------------------------------------------------------------*/ 
 
		-- update Section 
		;WITH Deltas AS 
		( 
			SELECT  
				 i.ApprenticeId 
				,ProfileTypeCode		= i.ProfileTypeCode 
				,TermList		= REPLACE(CAST(ISNULL(i.FirstName,'') + ISNULL(' ' + i.OtherNames,'') + ISNULL(' ' + i.Surname,'')  + ISNULL(' ' + i.PreferredName,'') + CASE WHEN i.Surname LIKE '% %' THEN ISNULL(' ' + REPLACE(i.Surname,' ','') ,'') ELSE '' END AS VARCHAR(MAX)) COLLATE SQL_Latin1_General_CP1_CI_AS, CHAR(0), '') 
				,FilterTermList = CASE i.[GenderCode] WHEN 'M' THEN '0M' WHEN 'F' THEN '0F' ELSE '0M 0F' END + ' '  + RIGHT(CAST(YEAR(i.BirthDate) AS VARCHAR),2) +  ' ' + '0' + i.ProfileTypeCode 
			FROM  
				Inserted i 
				JOIN Deleted d ON i.ApprenticeId = d.ApprenticeId 
		) 
		UPDATE [FT].[Apprentice] 
		SET  
			 [ProfileTypeCode]		= Deltas.[ProfileTypeCode] 
			,[TermList]			= Deltas.[TermList] 
			,[FilterTermList]	= Deltas.FilterTermList 
			,[ProcessStatus]	= 0 
			,[ProcessDate]		= GETDATE() 
		FROM  
			Deltas  JOIN  
			[FT].[Apprentice] WITH (INDEX =[IX_FT_Apprentice_ApprenticeId]) 
				ON [FT].[Apprentice].ApprenticeId = Deltas.ApprenticeId  
		WHERE 
			[FT].[Apprentice].ApprenticeId = deltas.ApprenticeId 
			AND 
			( 
				Deltas.[TermList]			<> [FT].[Apprentice].TermList 
				OR Deltas.FilterTermList	<> [FT].[Apprentice].FilterTermList 
				OR Deltas.ProfileTypeCode		<> [FT].[Apprentice].ProfileTypeCode 
			) 
		OPTION ( FORCE ORDER ) 
 
 
		-- Insert Section 
		INSERT INTO [FT].[Apprentice] 
        ( 
			[ApprenticeId] 
           ,[ProfileTypeCode] 
           ,[TermList] 
           ,[FilterTermList] 
           ,[ProcessStatus] 
           ,[ProcessDate] 
		) 
		SELECT  
			i.ApprenticeId 
			,ProfileTypeCode			= i.ProfileTypeCode 
			,TermList			= REPLACE(CAST(ISNULL(i.FirstName,'') + ISNULL(' ' + i.OtherNames,'') + ISNULL(' ' + i.Surname,'')  + ISNULL(' ' + i.PreferredName,'') + CASE WHEN i.Surname LIKE '% %' THEN ISNULL(' ' + REPLACE(i.Surname,' ','') ,'') ELSE '' END AS VARCHAR(MAX)) COLLATE SQL_Latin1_General_CP1_CI_AS, CHAR(0), '') 
			,FilterTermList = CASE i.[GenderCode] WHEN 'M' THEN '0M' WHEN 'F' THEN '0F' ELSE '0M 0F' END + ' '  + RIGHT(CAST(YEAR(i.BirthDate) AS VARCHAR),2) +  ' ' + '0' + i.ProfileTypeCode 
			,[ProcessStatus]	= 0 
			,[ProcessDate]		= GETDATE() 
		FROM  
			Inserted i 
			LEFT JOIN FT.[Apprentice] ft ON i.ApprenticeId = ft.ApprenticeId 
		WHERE 
			ft.ApprenticeId IS NULL 
END 
