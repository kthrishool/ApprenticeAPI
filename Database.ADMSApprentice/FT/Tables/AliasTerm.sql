CREATE TABLE [FT].[AliasTerm] (
    [Term]                    VARCHAR (50)  NOT NULL,
    [AliasTerm]               VARCHAR (50)  NOT NULL,
    [IgnoreFlag]              BIT           CONSTRAINT [DF_FT_AliasTerm_IgnoreFlag] DEFAULT ((0)) NULL,
    [AliasConsonantTerm]      VARCHAR (5)   NULL,
    [AliasConsonantBroadTerm] VARCHAR (4)   NULL,
    [AliasMetaphoneTerm]      VARCHAR (5)   NULL,
    [AliasMetaphoneBroadTerm] VARCHAR (4)   NULL,
    [ReprocessFlag]           BIT           CONSTRAINT [DF_FT_AliasTerm_ReprocessFlag] DEFAULT ((0)) NULL,
    [UpdatedOn]               DATETIME2 (7) NULL,
    CONSTRAINT [PK_FT_AliasTerm] PRIMARY KEY CLUSTERED ([Term] ASC, [AliasTerm] ASC)
);

