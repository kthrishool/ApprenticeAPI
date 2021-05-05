CREATE TABLE [Infrastructure].[ServiceBusEvent] (
    [Id]            INT              IDENTITY (1, 1) NOT NULL,
    [CorrelationId] UNIQUEIDENTIFIER NULL,
    [MessageId]     UNIQUEIDENTIFIER NOT NULL,
    [EventType]     NVARCHAR (50)    NOT NULL,
    [Message]       NVARCHAR (MAX)   NOT NULL,
    [Status]        INT              NOT NULL,
    [Created]       DATETIME         NOT NULL,
    [LastUpdated]   DATETIME         NOT NULL,
    [CustomHeaders] NVARCHAR (1024)  NULL,
    [ChainId]       NVARCHAR (50)    NULL,
    [ParentChainId] NVARCHAR (50)    NULL,
    CONSTRAINT [PK_Infrastructure.ServiceBusEvent] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [FUIX_ServiceBusEvent_IdFiltered_status_filteredStatus]
    ON [Infrastructure].[ServiceBusEvent]([Id] ASC, [Status] ASC) WHERE ([STATUS] IN ((0), (1))) WITH (STATISTICS_NORECOMPUTE = ON);


GO
CREATE NONCLUSTERED INDEX [IX_ServiceBusEvent_CreatedStatusLastUpdated]
    ON [Infrastructure].[ServiceBusEvent]([Status] ASC, [LastUpdated] ASC)
    INCLUDE([CorrelationId], [Created], [EventType], [Message], [MessageId]) WITH (STATISTICS_NORECOMPUTE = ON);

