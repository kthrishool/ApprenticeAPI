CREATE TABLE [dbo].[ApprenticeAddress] (
    [ApprenticeAddressId] INT             IDENTITY (1, 1) NOT NULL,
    [ApprenticeId]        INT             NOT NULL,
    [AddressTypeCode]     VARCHAR (10)    NOT NULL,
    [StreetAddress1]      VARCHAR (100)   NOT NULL,
    [StreetAddress2]      VARCHAR (100)   NULL,
    [StreetAddress3]      VARCHAR (100)   NULL,
    [Locality]            VARCHAR (50)    NOT NULL,
    [StateCode]           VARCHAR (10)    NOT NULL,
    [Postcode]            VARCHAR (10)    NOT NULL,
    [SingleLineAddress]   VARCHAR (375)   NULL,
    [GeocodeType]         VARCHAR (4)     NULL,
    [Latitude]            DECIMAL (10, 8) NULL,
    [Longitude]           DECIMAL (11, 8) NULL,
    [Confidence]          SMALLINT        NULL,
    [CreatedBy]           NVARCHAR (1024) NOT NULL,
    [CreatedOn]           DATETIME2 (7)   NOT NULL,
    [UpdatedBy]           NVARCHAR (1024) NOT NULL,
    [UpdatedOn]           DATETIME2 (7)   NOT NULL,
    [Version]             ROWVERSION      NOT NULL,
    [_AuditEventId]       BIGINT          NOT NULL,
    CONSTRAINT [PK_ApprenticeAddress] PRIMARY KEY CLUSTERED ([ApprenticeAddressId] ASC),
    CONSTRAINT [FK_ApprenticeAddress_Apprentice] FOREIGN KEY ([ApprenticeId]) REFERENCES [dbo].[Apprentice] ([ApprenticeId])
);






GO
CREATE NONCLUSTERED INDEX [IX_ApprenticeAddress_ApprenticeId]
    ON [dbo].[ApprenticeAddress]([ApprenticeId] ASC) WITH (FILLFACTOR = 90);
GO
CREATE UNIQUE NONCLUSTERED INDEX [UIX_ApprenticeAddress_ApprenticeIdAddressTypeCode]
    ON [dbo].[ApprenticeAddress]([ApprenticeId] ASC, [AddressTypeCode] ASC) WITH (FILLFACTOR = 90);