CREATE TABLE [FT].[Term] (
    [Term]                  VARCHAR (50)   NOT NULL,
    [ConsonantTerm]         VARCHAR (5)    NULL,
    [ConsonantBroadTerm]    VARCHAR (4)    NULL,
    [SoundexTerm1]          CHAR (4)       NULL,
    [SoundexTerm2]          CHAR (4)       NULL,
    [SoundexTerm3]          CHAR (4)       NULL,
    [MetaphoneTerm]         VARCHAR (5)    NULL,
    [MetaphoneTermBroad]    VARCHAR (4)    NULL,
    [MetaphoneTermAlt]      VARCHAR (5)    NULL,
    [ProcessedFlag]         BIT            CONSTRAINT [DF_FT_Term_ProcessedFlag] DEFAULT ((0)) NULL,
    [TermChar1]             AS             (left([Term],(1))) PERSISTED,
    [TermChar2]             AS             (case when len([TERM])>(2) then left([Term],(2)) else '' end) PERSISTED,
    [TermChar3]             AS             (case when len([TERM])>(3) then left([Term],(3)) else '' end) PERSISTED,
    [TermChar4]             AS             (case when len([TERM])>(4) then left([Term],(4)) else '' end) PERSISTED,
    [TermChar5]             AS             (case when len([TERM])>(5) then left([Term],(5)) else '' end) PERSISTED,
    [TermChar6]             AS             (case when len([TERM])>(6) then left([Term],(6)) else '' end) PERSISTED,
    [TermChar7]             AS             (case when len([TERM])>(7) then left([Term],(7)) else '' end) PERSISTED,
    [TermChar8]             AS             (case when len([TERM])>(8) then left([Term],(8)) else '' end) PERSISTED,
    [Frequency]             INT            NULL,
    [PrefixFrequency]       INT            CONSTRAINT [DF_FT_Term_PrefixFrequency] DEFAULT ((0)) NULL,
    [AliasTermList]         VARCHAR (8000) NULL,
    [PrefixTermList]        VARCHAR (8000) NULL,
    [SpecificTermList]      VARCHAR (8000) NULL,
    [BroadTermList]         VARCHAR (8000) NULL,
    [AliasSpecificTermList] VARCHAR (8000) NULL,
    [AliasBroadTermList]    VARCHAR (8000) NULL,
    [CreatedOn]             DATETIME2 (7)  CONSTRAINT [DF_FT_Term_CreatedOn] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_FT_Term] PRIMARY KEY CLUSTERED ([Term] ASC)
);


GO


 
 
 
CREATE TRIGGER [FT].[trg_FT_Apprentice_Term_InsteadOf] ON [FT].[Term] 
INSTEAD OF INSERT 
AS 
 
BEGIN 
	-- SET NOCOUNT ON added to prevent extra result sets from 
	-- interfering with SELECT statements. 
	SET NOCOUNT ON; 
 
	DECLARE @MetaphoneTermSuffix char(2) = 'mZ' 
 
	;With NewTerms AS 
	( 
		SELECT  
			i.Term 
			,TermChar1 = LEFT (i.Term,1) 
			,SoundexTerm   = CASE WHEN LEN(i.Term) > 2 AND ISNUMERIC(i.Term) = 0 THEN RTRIM(LTRIM(Soundex(i.Term))) ELSE '' END 
			,ConsonantTerm = CASE WHEN ISNUMERIC(i.Term) = 1 THEN '' ELSE [FT].ConsonantTerm(i.Term) END 
			,DoubleMetaphoneTerm = CASE WHEN IsNumeric(i.Term) = 1 OR LEN(i.Term) <=  1 THEN '' ELSE FT.DoubleMetaPhone(i.Term)  END 
		FROM  
			Inserted i 
			LEFT JOIN FT.Term t ON i.Term = t.Term 
		WHERE 
			t.Term IS NULL 
	),  
	Term AS 
	( 
	SELECT  
		Term  
		,ConsonantTerm  
		,ConsonantBroadTerm = CASE WHEN LEN(ConsonantTerm) > 2 THEN LEFT(ConsonantTerm,LEN(ConsonantTerm)-1) ELSE '' END 
		,SoundexTerm1 = CASE WHEN RIGHT(SoundexTerm,3) = '000' THEN CASE WHEN LEN(ConsonantTerm) <= 2 OR RIGHT(SOUNDEX(ConsonantTerm),3) = '000' THEN '' ELSE SOUNDEX(ConsonantTerm) END ELSE SoundexTerm END 
		,SoundexTerm2  
			= CASE  
				WHEN LTRIM(SOundexTerm) IN('' ,'0000') THEN ''  
				WHEN RIGHT(SoundexTerm,1) <> '0'  THEN LEFT(SoundexTerm,3) + '0'  
				WHEN RIGHT(SoundexTerm,2) <> '00' THEN LEFT(SoundexTerm,2) + '00' 
				ELSE ''  
			END 
		,SoundexTerm3  
			= CASE  
				WHEN SOundexTerm IN('    ' ,'0000') THEN ''  
				WHEN RIGHT(SoundexTerm,1) <> '0'  AND RIGHT(SoundexTerm,2) <> '00'   
				THEN LEFT(SoundexTerm,2) + '00'  
				ELSE ''  
		END 
		,[MetaphoneTerm]     = CASE WHEN LEN(RTRIM(LEFT(DoubleMetaphoneTerm, 5))) > 2 --AND LEFT(DoubleMetaphoneTerm, 1) = TermChar1  
			THEN RTRIM(LEFT(DoubleMetaphoneTerm, 5))  ELSE '' END 
		,[MetaphoneTermBroad]= CASE WHEN LEN(RTRIM(LEFT(DoubleMetaphoneTerm, 5))) > 2 --AND LEFT(DoubleMetaphoneTerm, 1) = TermChar1  
			THEN LEFT (RTRIM(LEFT(DoubleMetaphoneTerm, 5)), LEN(RTRIM(LEFT(DoubleMetaphoneTerm, 5)))-1) ELSE '' END 
		,[MetaphoneTermALT]  = CASE WHEN LEFT(DoubleMetaphoneTerm, 5) <> RIGHT(DoubleMetaphoneTerm, 5) AND LEN(RTRIM(RIGHT(DoubleMetaphoneTerm, 5)))> 2 AND SUBSTRING(DoubleMetaphoneTerm, 6, 1) = TermChar1 THEN RTRIM(RIGHT(DoubleMetaphoneTerm, 5)) ELSE '' END 
	FROM  
		NewTerms 
	), HasAlias 
	AS 
	( 
		SELECT  
			nt.Term 
		FROM  
			NewTerms nt 
			LEFT JOIN FT.AliasTerm at on nt.Term = at.Term 
		GROUP BY  
			nt.Term 
	) 
	INSERT INTO [FT].[Term] 
    ( 
		 [Term] 
		,[ConsonantTerm] 
		,[ConsonantBroadTerm] 
		,[SoundexTerm1] 
		,[SoundexTerm2] 
		,[SoundexTerm3] 
		,[MetaphoneTerm] 
		,[MetaphoneTermBroad] 
		,[MetaphoneTermALT] 
		,SpecificTermList 
		,BroadTermList 
		,AliasTermList 
		,AliasSpecificTermList 
		,AliasBroadTermList 
	) 
	SELECT  
		Term.Term  
		,ConsonantTerm  
		,ConsonantBroadTerm  
		,SoundexTerm1 
		,SoundexTerm2  
		,SoundexTerm3  
		,[MetaphoneTerm] 
		,[MetaphoneTermBroad] 
		,[MetaphoneTermALT]  
		,SpecificTermList  
		=  
			CASE  
				WHEN LEN(ConsonantTerm) >= 4 THEN  
					CASE  
						WHEN Right(SoundexTerm1, 1) <> '0' AND LEN(MetaphoneTerm) >= 4 THEN ConsonantTerm + CASE WHEN ConsonantTerm = MetaphoneTerm + @MetaphoneTermSuffix THEN '' ELSE ' ' + MetaphoneTerm + @MetaphoneTermSuffix END + ' ' + SoundexTerm1 
						WHEN Right(SoundexTerm1, 1) =  '0' AND LEN(MetaphoneTerm) >= 4  AND ConsonantTerm <> MetaphoneTerm  THEN ConsonantTerm + ' ' + MetaphoneTerm + @MetaphoneTermSuffix 
						WHEN Right(SoundexTerm1, 1) <> '0' AND LEN(MetaphoneTerm) < 4 OR ConsonantTerm <> MetaphoneTerm  THEN ConsonantTerm + ' ' + SoundexTerm1 
					END 
				WHEN LEN(ConsonantTerm) = 3 THEN  
					CASE  
						WHEN Right(SoundexTerm1, 2) <> '00' AND LEN(MetaphoneTerm) >= 3 THEN ConsonantTerm + CASE WHEN ConsonantTerm = MetaphoneTerm THEN '' ELSE ' ' + MetaphoneTerm + @MetaphoneTermSuffix END + ' ' + SoundexTerm1 
						WHEN Right(SoundexTerm1, 2) =  '00' AND LEN(MetaphoneTerm) >= 3  AND ConsonantTerm <> MetaphoneTerm  THEN ConsonantTerm + ' ' + MetaphoneTerm + @MetaphoneTermSuffix 
						WHEN Right(SoundexTerm1, 2) <> '00' AND LEN(MetaphoneTerm) < 3 OR ConsonantTerm <> MetaphoneTerm  THEN ConsonantTerm + ' ' + SoundexTerm1 
					END 
				WHEN LEN(ConsonantTerm) = 2 THEN  
					CASE  
						WHEN Right(SoundexTerm1, 3) <> '000' AND LEN(MetaphoneTerm) >= 2 THEN ConsonantTerm + CASE WHEN ConsonantTerm = MetaphoneTerm THEN '' ELSE ' ' + MetaphoneTerm + @MetaphoneTermSuffix END + ' ' + SoundexTerm1 
						WHEN Right(SoundexTerm1, 3) =  '000' AND LEN(MetaphoneTerm) >= 2  AND ConsonantTerm <> MetaphoneTerm  THEN ConsonantTerm + ' ' + MetaphoneTerm + @MetaphoneTermSuffix 
						WHEN Right(SoundexTerm1, 3) <> '000' AND LEN(MetaphoneTerm) < 2 OR ConsonantTerm <> MetaphoneTerm  THEN ConsonantTerm + ' ' + SoundexTerm1 
					END 
 
			END 
		,BroadTermList  
		=  
			LTRIM( 
				CASE  
				WHEN LEN(ConsonantBroadTerm) >= 3 AND LEN(MetaphoneTerm) = 3  THEN ConsonantBroadTerm + CASE WHEN ConsonantBroadTerm = MetaphoneTerm OR MetaphoneTerm = '' THEN '' ELSE ' ' + MetaphoneTerm + @MetaphoneTermSuffix END + ' ' + CASE WHEN LEN(SoundexTerm2) = 0 THEN SoundexTerm1 ELSE SoundexTerm2 END 
				WHEN LEN(ConsonantBroadTerm) >= 3 AND LEN(MetaphoneTerm) >= 3 THEN ConsonantBroadTerm + CASE WHEN ConsonantBroadTerm = MetaphoneTermBroad OR MetaphoneTermBroad = '' THEN '' ELSE ' ' + MetaphoneTermBroad + @MetaphoneTermSuffix END + ' ' + CASE WHEN LEN(SoundexTerm2) = 0 THEN SoundexTerm1 ELSE SoundexTerm2 END 
				WHEN LEN(ConsonantBroadTerm) =  2 AND (ConsonantBroadTerm = MetaphoneTermBroad  OR LEN(MetaphoneTermBroad) = 0) THEN  ConsonantBroadTerm + ' ' + CASE WHEN LEN(SoundexTerm2) = 0 THEN SoundexTerm1 ELSE SoundexTerm2 END 
				WHEN LEN(ConsonantBroadTerm) =  2 AND ConsonantBroadTerm <> MetaphoneTermBroad THEN  ConsonantBroadTerm + ' ' + MetaphoneTermBroad + @MetaphoneTermSuffix + ' ' + CASE WHEN LEN(SoundexTerm2) = 0 THEN SoundexTerm1 ELSE SoundexTerm2 END 
				ELSE			 
					LEFT(Term.Term, 2)  + ' ' + CASE WHEN LEN(SoundexTerm2) = 0 THEN SoundexTerm1 ELSE SoundexTerm2 END 
			END 
			) 
		,AliasTermList   
			= CASE WHEN ha.Term is null  
				THEN '' 
				ELSE 
					LTRIM 
					( 
						STUFF 
							( 
								( 
									SELECT DISTINCT ' ' + [AliasTerm]  
									FROM  
										[FT].[AliasTerm] alt 
									WHERE Term.Term = alt.Term 
									FOR XML PATH(''), type 
								).value('.', 'varchar(max)'), 1, 1, '' 
							)  
					) 
				END 
		,AliasSpecificTermList  
			= CASE WHEN ha.Term is null  
				THEN '' 
				ELSE 
					LTRIM 
					( 
						STUFF 
							( 
								( 
									SELECT DISTINCT ' ' + [AliasConsonantTerm]  
									FROM  
										[FT].[AliasTerm] alt 
									WHERE Term.Term = alt.Term 
										And [AliasConsonantTerm] NOT IN ( ConsonantTerm, ConsonantBroadTerm, MetaphoneTerm, MetaphoneTermBroad) 
									FOR XML PATH(''), type 
								).value('.', 'varchar(max)'), 1, 1, '' 
							)  + ' ' +  
						STUFF 
							( 
								( 
									SELECT DISTINCT ' ' + AliasMetaphoneTerm 
									FROM  
										[FT].[AliasTerm] alt 
									WHERE Term.Term = alt.Term 
									And AliasMetaphoneTerm NOT IN ( [AliasConsonantTerm], ConsonantTerm, ConsonantBroadTerm, MetaphoneTerm, MetaphoneTermBroad) 
									FOR XML PATH(''), type 
								).value('.', 'varchar(max)'), 1, 1, '' 
							)  
					) 
				END 
	,AliasBroadTermList =  
		CASE WHEN ha.Term is null  
			THEN '' 
			ELSE 
				LTRIM 
				( 
					STUFF 
						( 
							( 
								SELECT DISTINCT ' ' + alt.AliasConsonantBroadTerm  
								FROM  
									[FT].[AliasTerm] alt 
								WHERE Term.Term = alt.AliasTerm 
								AND alt.AliasConsonantBroadTerm NOT IN ( AliasMetaphoneTerm, [AliasConsonantTerm], ConsonantTerm, ConsonantBroadTerm, MetaphoneTerm, MetaphoneTermBroad) 
								FOR XML PATH(''), type 
							).value('.', 'varchar(max)'), 1, 1, '' 
						)  + ' ' + 
					STUFF 
						( 
							( 
								SELECT DISTINCT ' ' + alt.AliasMetaphoneBroadTerm 
								FROM  
									[FT].[AliasTerm] alt 
								WHERE Term.Term = alt.AliasTerm 
								AND alt.AliasMetaphoneBroadTerm NOT IN ( alt.AliasConsonantBroadTerm, AliasMetaphoneTerm, [AliasConsonantTerm], ConsonantTerm, ConsonantBroadTerm, MetaphoneTerm, MetaphoneTermBroad) 
								FOR XML PATH(''), type 
							).value('.', 'varchar(max)'), 1, 1, '' 
						)  
				) 
		END 
 
	FROM  
		Term 
		LEFT JOIN HasAlias ha on Term.Term = ha.Term 
	OPTION ( FORCE ORDER)  
END 

