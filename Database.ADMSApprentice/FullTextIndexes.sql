CREATE FULLTEXT INDEX ON [FT].[Apprentice]
    ([SearchTermList] LANGUAGE 1033, [SearchFuzzyTermList] LANGUAGE 1033)
    KEY INDEX [PK_FT_Apprentice]
    ON [FT_Apprentice];

