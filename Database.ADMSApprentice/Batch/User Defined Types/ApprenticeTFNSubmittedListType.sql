CREATE TYPE [Batch].[ApprenticeTFNSubmittedListType] AS TABLE (
    [MessageQueueCorrelationId] UNIQUEIDENTIFIER NOT NULL,
    [Version]                   BINARY (8)       NOT NULL);

