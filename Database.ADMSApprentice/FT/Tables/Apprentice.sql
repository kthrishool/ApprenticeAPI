CREATE TABLE [FT].[Apprentice] (
    [Key]                                 INT            IDENTITY (1, 1) NOT NULL,
    [ApprenticeId]                        INT            NOT NULL,
    [ProfileTypeCode]                     VARCHAR (10)   NULL,
    [TermList]                            VARCHAR (8000) NULL,
    [FirstNameFuzzySpecificTermList]      VARCHAR (8000) NULL,
    [FirstNameFuzzyBroadTermList]         VARCHAR (8000) NULL,
    [FirstNameAliasTermList]              VARCHAR (8000) NULL,
    [FirstNameAliasFuzzySpecificTermList] VARCHAR (8000) NULL,
    [SurnameFuzzySpecificTermList]        VARCHAR (8000) NULL,
    [SurnameFuzzyBroadTermList]           VARCHAR (8000) NULL,
    [OtherNamesFuzzySpecificTermList]     VARCHAR (8000) NULL,
    [OtherNamesFuzzyBroadTermList]        VARCHAR (8000) NULL,
    [FilterTermList]                      VARCHAR (1000) NULL,
    [ProcessStatus]                       TINYINT        NULL,
    [ProcessDate]                         DATETIME       NULL,
    [ApprenticeVersion]                   BIGINT         NULL,
    [Version]                             ROWVERSION     NULL,
    [SearchTermList]                      AS             ((((([TermList]+' ')+[FilterTermList])+' ')+' ')+[FirstNameAliasTermList]),
    [SearchFuzzyTermList]                 AS             (((((((((((([FilterTermList]+' ')+[TermList])+' ')+[FirstNameAliasTermList])+' ')+[FirstNameFuzzySpecificTermList])+' ')+[SurnameFuzzySpecificTermList])+' ')+[OtherNamesFuzzySpecificTermList])+' ')+[FirstNameFuzzyBroadTermList]),
    CONSTRAINT [PK_FT_Apprentice] PRIMARY KEY CLUSTERED ([Key] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_FT_Apprentice_ProcessStatusApprenticeId]
    ON [FT].[Apprentice]([ProcessStatus] ASC, [ApprenticeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FT_Apprentice_ApprenticeId]
    ON [FT].[Apprentice]([ApprenticeId] ASC);

