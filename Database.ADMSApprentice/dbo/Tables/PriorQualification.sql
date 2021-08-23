CREATE TABLE [dbo].[PriorQualification] (
    [PriorQualificationId] INT             IDENTITY (1, 1) NOT NULL,
    [ApprenticeId]              INT             NOT NULL,
    [ApprenticeshipId]          INT             NULL,
    [QualificationCode]         VARCHAR (10)    NOT NULL,
    [QualificationDescription]  VARCHAR (200)   NULL,
    [QualificationLevel]        VARCHAR (10)    NULL,
    [QualificationANZSCOCode]   VARCHAR (10)    NULL,
    [NotOnTrainingGovAuFlag]    BIT             NOT NULL,
    [StartDate]                 DATE            NULL,
    [EndDate]                   DATE            NULL,
    [CreatedBy]                 NVARCHAR (1024) NOT NULL,
    [CreatedOn]                 DATETIME2 (7)   NOT NULL,
    [UpdatedBy]                 NVARCHAR (1024) NOT NULL,
    [UpdatedOn]                 DATETIME2 (7)   NOT NULL,
    [Version]                   ROWVERSION      NOT NULL,
    [_AuditEventId]             BIGINT          NOT NULL,
    CONSTRAINT [PK_PriorQualification] PRIMARY KEY CLUSTERED ([PriorQualificationId] ASC),
    CONSTRAINT [FK_PriorQualification_Apprentice] FOREIGN KEY ([ApprenticeId]) REFERENCES [dbo].[Apprentice] ([ApprenticeId])
);
GO
CREATE NONCLUSTERED INDEX [IX_PriorQualification_ApprenticeId]
    ON [dbo].[PriorQualification]([ApprenticeId] ASC) WITH (FILLFACTOR = 90);
