CREATE TABLE [SensitiveLogging].[APIRequestLog] (
    [APIRequestLogId]    BIGINT           IDENTITY (1, 1) NOT NULL,
    [LogDateTime]        DATETIME2 (7)    NOT NULL,
    [UserIdentity]       NVARCHAR (1024)  NOT NULL,
    [UserRole]           VARCHAR (200)    NOT NULL,
    [AccessTypeCode]     VARCHAR (10)     NOT NULL,
    [CorrelationId]      UNIQUEIDENTIFIER NOT NULL,
    [ProgramCode]        VARCHAR (10)     NOT NULL,
    [SensitiveDataPairs] VARCHAR (MAX)    NULL,
    [ResponseCode]       VARCHAR (10)     NULL,
    [RequestURL]         VARCHAR (500)    NOT NULL,
    [OrgCode]            VARCHAR (10)     NOT NULL,
    [SiteCode]           VARCHAR (10)     NOT NULL,
    [Contracts]          VARCHAR (100)    NOT NULL,
    CONSTRAINT [PK__SensitiveLogging_APIRequestLog] PRIMARY KEY CLUSTERED ([APIRequestLogId] ASC)
);

