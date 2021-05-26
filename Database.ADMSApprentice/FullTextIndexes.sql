CREATE FULLTEXT INDEX ON [FT].[Apprentice]
    ([SearchTermList] LANGUAGE 1033, [SearchFuzzyTermList] LANGUAGE 1033)
    KEY INDEX [PK_FT_Apprentice]
    ON [FT_Apprentice];


GO
CREATE FULLTEXT INDEX ON [dbo].[ApprenticeAddress]
    ([StreetAddress1] LANGUAGE 1033, [StreetAddress2] LANGUAGE 1033, [StreetAddress3] LANGUAGE 1033, [Locality] LANGUAGE 1033, [Postcode] LANGUAGE 1033)
    KEY INDEX [PK_ApprenticeAddress]
    ON [FT_Apprentice];

