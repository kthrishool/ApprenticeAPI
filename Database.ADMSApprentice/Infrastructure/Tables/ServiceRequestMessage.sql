CREATE TABLE [Infrastructure].[ServiceRequestMessage] (
    [ServiceRequestMessageId]         BIGINT           IDENTITY (1, 1) NOT NULL,
    [ServiceExternalRequestMessageId] NVARCHAR (100)   NOT NULL,
    [RequestUsername]                 NVARCHAR (1024)  NOT NULL,
    [RequestUsernameHash]             AS               (CONVERT([binary](32),hashbytes('SHA2_256',[RequestUsername]),(0))) PERSISTED,
    [ServiceMethod]                   NVARCHAR (500)   NOT NULL,
    [CorrelationId]                   UNIQUEIDENTIFIER NOT NULL,
    [RequestReceivedDateTime]         DATETIME2 (7)    NOT NULL,
    [RequestProcessedDateTime]        DATETIME2 (7)    NULL,
    [RequestMessageData]              NVARCHAR (MAX)   NOT NULL,
    [ResponseMessageData]             NVARCHAR (MAX)   NULL,
    [ServiceMethodHash]               AS               (CONVERT([binary](32),hashbytes('SHA2_256',[ServiceMethod]),(0))) PERSISTED,
    CONSTRAINT [PK_infrastructure.ServiceRequestMessage] PRIMARY KEY CLUSTERED ([ServiceRequestMessageId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Infrastructure_ServiceRequestMessage_CorrelationId]
    ON [Infrastructure].[ServiceRequestMessage]([CorrelationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ServiceRequestMessage_RequestReceivedDateTime]
    ON [Infrastructure].[ServiceRequestMessage]([RequestReceivedDateTime] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UIX_ServiceRequestMessageExternalRequestMessageIdRequestUsernameServiceMethod]
    ON [Infrastructure].[ServiceRequestMessage]([ServiceExternalRequestMessageId] ASC, [RequestUsernameHash] ASC, [ServiceMethodHash] ASC)
    INCLUDE([RequestUsername], [ServiceMethod]);

