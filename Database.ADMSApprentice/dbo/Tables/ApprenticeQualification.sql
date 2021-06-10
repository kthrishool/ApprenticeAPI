CREATE TABLE [dbo].[ApprenticeQualification] (
    [ApprenticeQualificationId] INT             IDENTITY (1, 1) NOT NULL,
    [ApprenticeId]              INT             NOT NULL,
    [QualificationCode]         VARCHAR (10)    NOT NULL,
    [QualificationDescription]  VARCHAR (50)    NULL,
    [QualificationLevel]        VARCHAR (10)    NULL,
    [QualificationANZSCOCode]   VARCHAR (10)    NULL,
    [StartDate]                 DATE            NULL,
    [EndDate]                   DATE            NULL,
    [ApprenticeshipId]          INT             NULL,
    [CreatedBy]                 NVARCHAR (1024) NOT NULL,
    [CreatedOn]                 DATETIME2 (7)   NOT NULL,
    [UpdatedBy]                 NVARCHAR (1024) NOT NULL,
    [UpdatedOn]                 DATETIME2 (7)   NOT NULL,
    [Version]                   ROWVERSION      NOT NULL,
    [_AuditEventId]             BIGINT          NOT NULL,
    CONSTRAINT [PK_ApprenticeQualification] PRIMARY KEY CLUSTERED ([ApprenticeQualificationId] ASC),
    CONSTRAINT [FK_ApprenticeQualification_Apprentice] FOREIGN KEY ([ApprenticeId]) REFERENCES [dbo].[Apprentice] ([ApprenticeId])
);

