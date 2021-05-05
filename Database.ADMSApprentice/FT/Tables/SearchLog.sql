CREATE TABLE [FT].[SearchLog] (
    [SearchLogId]     INT            IDENTITY (1, 1) NOT NULL,
    [RecordSource]    CHAR (3)       NOT NULL,
    [ExecutionDate]   DATETIME       NOT NULL,
    [ExecutionUserId] VARCHAR (50)   NULL,
    [Duration]        INT            NULL,
    [MaxRows]         INT            NULL,
    [RowCount]        INT            NULL,
    [SearchString]    VARCHAR (200)  NULL,
    [Filter]          VARCHAR (200)  NULL,
    [FTCriteria]      VARCHAR (1000) NULL,
    CONSTRAINT [PK_FT_SearchLog] PRIMARY KEY CLUSTERED ([SearchLogId] DESC) WITH (FILLFACTOR = 90)
);

