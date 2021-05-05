CREATE TABLE [dbo].[Guardian] (
    [GuardianId]      INT             IDENTITY (1, 1) NOT NULL,
    [ApprenticeId]    INT             NOT NULL,
    [Surname]         VARCHAR (50)    NOT NULL,
    [FirstName]       VARCHAR (50)    NOT NULL,
    [OtherNames]      VARCHAR (50)    NULL,
    [GenderCode]      VARCHAR (10)    NOT NULL,
    [EmailAddress]    VARCHAR (320)   NULL,
    [ContactTypeCode] VARCHAR (10)    NOT NULL,
    [PhoneNumber]     VARCHAR (15)    NOT NULL,
    [StreetAddress]   VARCHAR (200)   NOT NULL,
    [Locality]        VARCHAR (50)    NOT NULL,
    [StateCode]       VARCHAR (10)    NOT NULL,
    [Postcode]        VARCHAR (10)    NOT NULL,
    [CreatedBy]       NVARCHAR (1024) NOT NULL,
    [CreatedOn]       DATETIME2 (7)   NOT NULL,
    [UpdatedBy]       NVARCHAR (1024) NOT NULL,
    [UpdatedOn]       DATETIME2 (7)   NOT NULL,
    [Version]         ROWVERSION      NOT NULL,
    [_AuditEventId]   BIGINT          NOT NULL,
    CONSTRAINT [PK_Guardian] PRIMARY KEY CLUSTERED ([GuardianId] ASC),
    CONSTRAINT [FK_Guardian_Apprentice] FOREIGN KEY ([ApprenticeId]) REFERENCES [dbo].[Apprentice] ([ApprenticeId])
);


GO
CREATE NONCLUSTERED INDEX [IX_Guardian_ApprenticeId]
    ON [dbo].[Guardian]([ApprenticeId] ASC);

