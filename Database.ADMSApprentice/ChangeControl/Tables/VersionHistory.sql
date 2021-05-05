CREATE TABLE [ChangeControl].[VersionHistory] (
    [VersionNumber] AS             ((CONVERT([varchar](10),[Major])+'.')+CONVERT([varchar](10),[Minor])),
    [Major]         TINYINT        NOT NULL,
    [Minor]         TINYINT        NOT NULL,
    [Reference]     NVARCHAR (200) CONSTRAINT [DF__ChangeControl_VersionHistory_Reference] DEFAULT (N'') NULL,
    [DeployedOn]    DATETIME2 (7)  CONSTRAINT [DF__ChangeControl_VersionHistory_DeployedOn] DEFAULT (SYSDATETIME()) NOT NULL,
    [DeployedBy]    [sysname]      CONSTRAINT [DF__ChangeControl_VersionHistory_DeployedBy] DEFAULT (suser_sname()) NOT NULL,
    CONSTRAINT [PK__ChangeControl__VersionNumber] PRIMARY KEY CLUSTERED ([Major] ASC, [Minor] ASC, [DeployedOn] ASC)
);

