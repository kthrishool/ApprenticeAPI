CREATE TABLE [dbo].[ApprenticeComment] (
    [ApprenticeCommentId] INT             IDENTITY (1, 1) NOT NULL,
    [ApprenticeId]        INT             NOT NULL,
    [CommentType]         VARCHAR (10)    NOT NULL,
    [Comment]             VARCHAR (MAX)   NOT NULL,
    [PrivateCommentFlag]  BIT             NOT NULL,
    [CreatedBy]           NVARCHAR (1024) NOT NULL,
    [CreatedOn]           DATETIME2 (7)   NOT NULL,
    [UpdatedBy]           NVARCHAR (1024) NOT NULL,
    [UpdatedOn]           DATETIME2 (7)   NOT NULL,
    [Version]             ROWVERSION      NOT NULL,
    [_AuditEventId]       BIGINT          NOT NULL,
    CONSTRAINT [PK_ApprenticeComment] PRIMARY KEY CLUSTERED ([ApprenticeCommentId] ASC),
    CONSTRAINT [FK_ApprenticeComment_Apprentice] FOREIGN KEY ([ApprenticeId]) REFERENCES [dbo].[Apprentice] ([ApprenticeId])
);


GO
CREATE NONCLUSTERED INDEX [IX_ApprenticeComment_ApprenticeId]
    ON [dbo].[ApprenticeComment]([ApprenticeId] ASC) WITH (FILLFACTOR = 90);

