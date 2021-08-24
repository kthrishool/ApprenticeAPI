CREATE TABLE [dbo].[ApprenticePhone] (
    [ApprenticePhoneId]  INT             IDENTITY (1, 1) NOT NULL,
    [ApprenticeId]       INT             NOT NULL,
    [PhoneTypeCode]      VARCHAR (10)    NOT NULL,
    [PhoneNumber]        VARCHAR (15)    NOT NULL,
    [CountryCode]       VARCHAR (10)    NOT NULL,
    [PreferredPhoneFlag] BIT             NULL,
    [CreatedBy]          NVARCHAR (1024) NOT NULL,
    [CreatedOn]          DATETIME2 (7)   NOT NULL,
    [UpdatedBy]          NVARCHAR (1024) NOT NULL,
    [UpdatedOn]          DATETIME2 (7)   NOT NULL,
    [Version]            ROWVERSION      NOT NULL,
    [_AuditEventId]      BIGINT          NOT NULL,
    CONSTRAINT [PK_ApprenticePhone] PRIMARY KEY CLUSTERED ([ApprenticePhoneId] ASC),
    CONSTRAINT [FK_ApprenticePhone_Apprentice] FOREIGN KEY ([ApprenticeId]) REFERENCES [dbo].[Apprentice] ([ApprenticeId])
);


GO
CREATE NONCLUSTERED INDEX [IX_ApprenticePhone_ApprenticeId]
    ON [dbo].[ApprenticePhone]([ApprenticeId] ASC) WITH (FILLFACTOR = 90);

GO
CREATE NONCLUSTERED INDEX [IX_ApprenticePhone_PhoneNumber]
    ON [dbo].[ApprenticePhone]([PhoneNumber] ASC) WITH (FILLFACTOR = 90);
