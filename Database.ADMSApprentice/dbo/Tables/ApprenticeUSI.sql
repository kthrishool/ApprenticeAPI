﻿CREATE TABLE [dbo].[ApprenticeUSI] (
    [ApprenticeUSIId]        INT             IDENTITY (1, 1) NOT NULL,
    [ApprenticeId]           INT             NOT NULL,
    [USI]                    VARCHAR (10)    NULL,
    [ActiveFlag]             BIT             NOT NULL,
    [USIChangeReason]        VARCHAR (150)   NULL,
    [USIVerifyFlag]          BIT             NULL,
    [FirstNameMatchedFlag]   BIT             NULL,
    [SurnameMatchedFlag]     BIT             NULL,
    [DateOfBirthMatchedFlag] BIT             NULL,
    [USIStatusCode]          VARCHAR (10)    NULL,
    [CreatedBy]              NVARCHAR (1024) NOT NULL,
    [CreatedOn]              DATETIME2 (7)   NOT NULL,
    [UpdatedBy]              NVARCHAR (1024) NOT NULL,
    [UpdatedOn]              DATETIME2 (7)   NOT NULL,
    [Version]                ROWVERSION      NOT NULL,
    [_AuditEventId]          BIGINT          NOT NULL,
    CONSTRAINT [PK_ApprenticeUSI] PRIMARY KEY CLUSTERED ([ApprenticeUSIId] ASC),
    CONSTRAINT [FK_ApprenticeUSI_Apprentice] FOREIGN KEY ([ApprenticeId]) REFERENCES [dbo].[Apprentice] ([ApprenticeId])
);


GO
CREATE NONCLUSTERED INDEX [IX_ApprenticeUSI_ApprenticeId]
    ON [dbo].[ApprenticeUSI]([ApprenticeId] ASC) WITH (FILLFACTOR = 90);
