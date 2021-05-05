CREATE TYPE [Batch].[ApprenticeTFNMatchStatusListType] AS TABLE (
    [MessageQueueCorrelationId] UNIQUEIDENTIFIER NOT NULL,
    [StatusCode]                VARCHAR (10)     NOT NULL,
    [StatusReasonCode]          VARCHAR (10)     NULL);

