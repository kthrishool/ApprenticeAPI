CREATE TABLE [dbo].[Guardian] (
    [GuardianId]        INT             IDENTITY (1, 1) NOT NULL,
    [ApprenticeId]      INT             NOT NULL,
    [Surname]           VARCHAR (50)    NULL,
    [FirstName]         VARCHAR (50)    NULL,
    [EmailAddress]      VARCHAR (320)   NULL,
    [HomePhoneNumber]   VARCHAR (15)    NULL,
    [Mobile]            VARCHAR (15)    NULL,
    [WorkPhoneNumber]   VARCHAR (15)    NULL,
    [StreetAddress1]    VARCHAR (100)   NULL,
    [StreetAddress2]    VARCHAR (100)   NULL,
    [StreetAddress3]    VARCHAR (100)   NULL,
    [Locality]          VARCHAR (50)    NULL,
    [StateCode]         VARCHAR (10)    NULL,
    [Postcode]          VARCHAR (10)    NULL,
    [SingleLineAddress] VARCHAR (375)   NULL,
    [GeocodeType]       VARCHAR (4)     NULL,
    [Latitude]          DECIMAL (10, 8) NULL,
    [Longitude]         DECIMAL (11, 8) NULL,
    [Confidence]        SMALLINT        NULL,
    [CreatedBy]         NVARCHAR (1024) NOT NULL,
    [CreatedOn]         DATETIME2 (7)   NOT NULL,
    [UpdatedBy]         NVARCHAR (1024) NOT NULL,
    [UpdatedOn]         DATETIME2 (7)   NOT NULL,
    [Version]           ROWVERSION      NOT NULL,
    [_AuditEventId]     BIGINT          NOT NULL,
    CONSTRAINT [PK_Guardian] PRIMARY KEY CLUSTERED ([GuardianId] ASC),
    CONSTRAINT [FK_Guardian_Apprentice] FOREIGN KEY ([ApprenticeId]) REFERENCES [dbo].[Apprentice] ([ApprenticeId])
);




GO
CREATE NONCLUSTERED INDEX [IX_Guardian_ApprenticeId]
    ON [dbo].[Guardian]([ApprenticeId] ASC);

