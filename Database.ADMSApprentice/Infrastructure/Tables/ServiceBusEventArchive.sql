CREATE TABLE [Infrastructure].[ServiceBusEventArchive] (
    [Id]            INT              NOT NULL,
    [CorrelationId] UNIQUEIDENTIFIER NOT NULL,
    [MessageId]     UNIQUEIDENTIFIER NOT NULL,
    [EventType]     NVARCHAR (50)    NOT NULL,
    [Message]       NVARCHAR (MAX)   NOT NULL,
    [Status]        INT              NOT NULL,
    [Created]       DATETIME         NOT NULL,
    [LastUpdated]   DATETIME         NOT NULL,
    [CustomHeaders] NVARCHAR (1024)  NULL,
    [ChainId]       NVARCHAR (50)    NULL,
    [ParentChainId] NVARCHAR (50)    NULL,
    [PublishAfter]  DATETIME         NULL,
    CONSTRAINT [PK_ServiceBusEventArchive] PRIMARY KEY CLUSTERED ([Id] ASC)
);



