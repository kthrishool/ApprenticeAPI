CREATE TABLE [SensitiveLogging].[RequestLog] (
    [RequestLogId]   BIGINT          IDENTITY (1, 1) NOT NULL,
    [LogDateTime]    DATETIME2 (7)   NOT NULL,
    [Username]       NVARCHAR (1024) NOT NULL,
    [AccessPath]     VARCHAR (MAX)   NULL,
    [AccessTypeCode] VARCHAR (10)    NOT NULL,
    [ProgramCode]    VARCHAR (10)    NOT NULL,
    [ProcessCode]    VARCHAR (10)    NOT NULL,
    [DataRequest]    VARCHAR (100)   NOT NULL,
    [DataResponse]   VARCHAR (100)   NULL,
    [AuthorisedFlag] BIT             NOT NULL,
    CONSTRAINT [PK__SensitiveLogging_RequestLog] PRIMARY KEY CLUSTERED ([RequestLogId] ASC)
);

