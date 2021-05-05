CREATE TABLE [dbo].[ApprenticeTFN] (
    [ApprenticeTFNId]           INT              IDENTITY (1, 1) NOT NULL,
    [ApprenticeId]              INT              NOT NULL,
    [TaxFileNumber]             NVARCHAR (20)    NOT NULL,
    [StatusCode]                VARCHAR (10)     NOT NULL,
    [StatusDate]                DATETIME2 (7)    NOT NULL,
    [StatusReasonCode]          VARCHAR (10)     NULL,
    [MessageQueueCorrelationId] UNIQUEIDENTIFIER NULL,
    [CreatedBy]                 NVARCHAR (1024)  NOT NULL,
    [CreatedOn]                 DATETIME2 (7)    NOT NULL,
    [UpdatedBy]                 NVARCHAR (1024)  NOT NULL,
    [UpdatedOn]                 DATETIME2 (7)    NOT NULL,
    [Version]                   ROWVERSION       NOT NULL,
    [_AuditEventId]             BIGINT           NOT NULL,
    CONSTRAINT [PK_ApprenticeTFN] PRIMARY KEY CLUSTERED ([ApprenticeTFNId] ASC),
    CONSTRAINT [FK_ApprenticeTFN_Apprentice] FOREIGN KEY ([ApprenticeId]) REFERENCES [dbo].[Apprentice] ([ApprenticeId])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UIX_ApprenticeTFN_ApprenticeId]
    ON [dbo].[ApprenticeTFN]([ApprenticeId] ASC) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [IX_ApprenticeTFN_MessageQueueCorrelationId]
    ON [dbo].[ApprenticeTFN]([MessageQueueCorrelationId] ASC) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [IX_ApprenticeTFN_MessageQueueCorrelationIdStatusCodeStatusDate]
    ON [dbo].[ApprenticeTFN]([MessageQueueCorrelationId] ASC, [StatusCode] ASC, [StatusDate] ASC) WITH (FILLFACTOR = 90);

