

/****************************************************************************
 
Object:			[dbo].[ApprenticeBasicSearch]

Description:	Returns apprentices that meet the search criteria.  A ranking is provided in order of USI,
				PhoneNumber, Email and then best Named match.

				Name fuzzy searching logic based on EMP.[jobseeker].[JobSeekerFuzzySearch] by Mick Vullo

				If @FirstName or @Surname is provided @BirthDate must also be provided.
				If @EmailAddress is provided it will return only an exact match.
				If @PhoneNumber is provided it will return only an exact match.
				If @USI is provided, only records which match are returned.

Usage:       
				USE ADMSApprentice

				EXEC [dbo].[ApprenticeBasicSearch]
					@Surname	= 'Trot'   
					,@EmailAddress = 'arcb@btac.nsw.edu.au'

Author:			JD3044
  
Created:		2021-07-23

Modification History:
USERID	Date		      Description
=======	==========		====================================================
***************************************************************************/
CREATE PROC [dbo].[ApprenticeBasicSearch]   
	@FirstName				[varchar](50) = '' 
	,@Surname				[varchar](50) = '' 
	,@BirthDate				[Date] = null
	,@USI					[varchar](10) = ''
	,@EmailAddress			[varchar](320) = ''
	,@PhoneNumber			[varchar](15) = ''
	,@MaxRows				[int] = 100   
	,@QualityCode			[varchar](10) = 'M'   
	,@FuzzyFactor			[int] = 4   
	,@FTCriteria			[varchar](1000) = NULL   
 
AS   
   
	SET NOCOUNT ON;

	BEGIN TRY

		IF @FirstName IS NULL
			SET @FirstName = ''
		IF @Surname IS NULL
			SET @Surname = ''
		IF @EmailAddress IS NULL
			SET @EmailAddress = ''
		IF @PhoneNumber IS NULL
			SET @PhoneNumber = ''
		IF @USI IS NULL
			SET @USI = ''

		DECLARE    
			 @Names  varchar(150) = ''
			,@SearchString  varchar(200) = null   
			,@SearchMode varchar(50)   
			,@StartTime datetime =GETDATE()   
			,@RowCount int   
			,@CriteriaXML XML   
			,@FTSearchStr varchar(1000)   
			,@TermCriteria varchar(1000)   
			,@FuzzyConsonantCriteria varchar(1000)   
			,@FuzzySpecificCriteria varchar(1000)   
			,@FuzzyBroadCriteria varchar(1000)   
			,@SearchStringLen int   
			,@SearchTerm1 varchar(50)   
			,@SearchTerm2 varchar(50)   
			,@SearchTerm3 varchar(50)   
			,@SearchTerm4 varchar(50)   
			,@SearchTerm5 varchar(50)   
			,@SearchTerm6 varchar(50) 
			,@LeastSignificantTerm varchar(50)   
			,@SearchTermCount int   
			,@FTFilter varchar(200)   
			,@SearchType int = 1   
			,@Increment tinyint = 1   
			,@Iteration tinyint = 1   
			,@GenderCodeFilter	  varchar(10) = ''
			,@BirthYearFilter varchar(1000)   = ''

		IF @USI = '' AND @EmailAddress = '' AND @PhoneNumber = '' AND (@BirthDate IS NULL OR (@Surname = '' AND @FirstName = ''))
			RAISERROR ('If USI, EmailAddres and PhoneNumber is not provided then BirthDate and a name must be provided.',16,1)

		IF @BirthDate IS NULL AND (@Surname <> '' OR @FirstName <> '')
			RAISERROR ('BirthDate must be provided if a name is also provided.',16,1)

		IF OBJECT_ID ('TEMPDB..#ApprenticeMatches') IS NOT NULL
			DROP TABLE #ApprenticeMatches; 

		CREATE TABLE #ApprenticeMatches
			([ApprenticeId]   INT
			,[USIMatch] TINYINT
			,[PhoneNumberMatch] TINYINT
			,[EmailMatch] TINYINT
			,[BirthDateMatch] TINYINT
			,[FirstNameMatch] TINYINT
			,[SurnameMatch] TINYINT)

		IF OBJECT_ID ('TEMPDB..#InterimResults') IS NOT NULL
			DROP TABLE #InterimResults; 

		CREATE TABLE #InterimResults
			([ApprenticeId]   INT
			,[ProfileTypeCode] VARCHAR(10)
			,[FirstName] VARCHAR(50)  
			,[OtherNames] VARCHAR(50)
			,[Surname] VARCHAR(50)  
			,[GenderCode] VARCHAR(10)
			,[BirthDate] DATE
			,[PreferredName] VARCHAR(50)
			,[EmailAddress] VARCHAR(320)
			,[Phone1InternationalPrefix] VARCHAR(10)
			,[Phone1] VARCHAR(15)
			,[Phone2InternationalPrefix] VARCHAR(10)
			,[Phone2] VARCHAR(15)
			,[USI] VARCHAR(10)
			,[AddressMatchFlag] BIT
			,[ResidentialAddress] VARCHAR(350)
			,[ResidentialStateCode] VARCHAR(10)
			,[PostalAddress] VARCHAR(350)
			,[PostalStateCode] VARCHAR(10)
			,[NamesMatchFlag] BIT
			,[FirstNameMatch] BIT
			,[SurnameMatch] BIT
			,QualitySortOrder INT
			,SequenceNo INT
			,SearchType VARCHAR(10)
			,Iteration INT
			,ScoreValue INT
			)



		IF OBJECT_ID ('TEMPDB..#NamesResults') IS NOT NULL
			DROP TABLE #NamesResults; 

		CREATE TABLE #NamesResults
			([ApprenticeId]   INT
			,[ProfileTypeCode] VARCHAR(10)
			,[FirstName] VARCHAR(50)  
			,[OtherNames] VARCHAR(50)
			,[Surname] VARCHAR(50)  
			,[GenderCode] VARCHAR(10)
			,[BirthDate] DATE
			,[PreferredName] VARCHAR(50)
			,[EmailAddress] VARCHAR(320)
			,[PhoneNumber] VARCHAR(15)
			,QualitySortOrder INT
			,SequenceNo INT
			,SearchType VARCHAR(10)
			,Iteration INT
			,ScoreValue INT
			)

		DECLARE @NamesTable TABLE
			([Name] VARCHAR(50))

		IF OBJECT_ID ('TEMPDB..#ResultNamesTable') IS NOT NULL
			DROP TABLE #ResultNamesTable; 

		CREATE TABLE #ResultNamesTable
			([ApprenticeId]   INT
			,[Name] VARCHAR(50))

		INSERT INTO @NamesTable ([Name])
		SELECT value FROM STRING_SPLIT(@Names, ' ');

		SET @Names = LTRIM(RTRIM(LTRIM(RTRIM(@FirstName)) + ' ' + LTRIM(RTRIM(@Surname))))

		IF LEN(@USI) > 0
		BEGIN

			INSERT INTO #ApprenticeMatches
				([ApprenticeId]
				,[USIMatch])
			SELECT A.ApprenticeId
				,1
			FROM dbo.Apprentice A
			INNER JOIN dbo.ApprenticeUSI AU
				ON A.ApprenticeId = AU.ApprenticeId
			WHERE AU.ActiveFlag = 1
				AND AU.USI = @USI
				AND A.ActiveFlag = 1
				AND A.DeceasedFlag = 0

		END

		IF LEN(@EmailAddress) > 4
		BEGIN

			INSERT INTO #ApprenticeMatches
				([ApprenticeId]
				,[EmailMatch])
			SELECT A.ApprenticeId
				,1
			FROM dbo.Apprentice A
			WHERE A.EmailAddress = @EmailAddress
				AND A.ActiveFlag = 1
				AND A.DeceasedFlag = 0

		END

		IF LEN(@PhoneNumber) > 7
		BEGIN

			INSERT INTO #ApprenticeMatches
				([ApprenticeId]
				,[PhoneNumberMatch])
			SELECT A.ApprenticeId
				,1
			FROM dbo.Apprentice A
			WHERE A.ActiveFlag = 1
				AND A.DeceasedFlag = 0
				AND A.ApprenticeId IN (SELECT ApprenticeId FROM dbo.ApprenticePhone AP WHERE AP.PhoneNumber = @PhoneNumber)

		END

		IF @BirthDate IS NOT NULL
		BEGIN

			INSERT INTO #ApprenticeMatches
				([ApprenticeId]
				,[BirthDateMatch])
			SELECT A.ApprenticeId
				,1
			FROM dbo.Apprentice A
			WHERE A.[BirthDate] = @BirthDate
				AND A.ActiveFlag = 1
				AND A.DeceasedFlag = 0

		END

		INSERT INTO #InterimResults
		([ApprenticeId]
		,[ProfileTypeCode]
		,[FirstName]
		,[OtherNames]
		,[Surname]
		,[GenderCode]
		,[BirthDate]
		,[PreferredName]
		,[EmailAddress]
		,[Phone1InternationalPrefix]
		,[Phone1]
		,[Phone2InternationalPrefix]
		,[Phone2]
		,[USI]
		,QualitySortOrder
		,SequenceNo
		,SearchType
		,Iteration
		,ScoreValue
		)
		SELECT A.ApprenticeId
			,A.[ProfileTypeCode]
			,A.[FirstName]
			,A.[OtherNames]
			,A.[Surname]
			,A.[GenderCode]
			,A.[BirthDate]
			,A.[PreferredName]
			,A.[EmailAddress]
			,NULL
			,NULL
			,NULL
			,NULL
			,AU.[USI]
			,1
			,1
			,1
			,1
			,100
		FROM dbo.Apprentice A
		LEFT JOIN dbo.ApprenticeUSI AU
			ON A.ApprenticeId = AU.ApprenticeId
			AND AU.ActiveFlag = 1
		WHERE A.ApprenticeId IN (SELECT ApprenticeId FROM #ApprenticeMatches)

		UPDATE X
		SET ScoreValue = ScoreValue + 100*ISNULL([USIMatch],0) + 90*ISNULL([PhoneNumberMatch],0) + 80*ISNULL([EmailMatch],0)
		FROM #InterimResults X
		INNER JOIN (SELECT ApprenticeId
						,MAX([USIMatch]) AS [USIMatch]
						,MAX([PhoneNumberMatch]) AS [PhoneNumberMatch]
						,MAX([EmailMatch]) AS [EmailMatch]
						,MAX([BirthDateMatch]) AS [BirthDateMatch]
						,MAX([FirstNameMatch]) AS [FirstNameMatch]
						,MAX([SurnameMatch]) AS [SurnameMatch]
					FROM #ApprenticeMatches
					GROUP BY ApprenticeId) Y
			ON X.ApprenticeId = Y.ApprenticeId;


--Names

		IF LEN (@Names) > 0
		BEGIN

			SET @SearchString = @Names    
	   
			SELECT    
				@RowCount = 0    
				,@FTFilter =    
					CASE    
						WHEN LEN(@BirthYearFilter) > 0    
							THEN    
								CASE WHEN LEN (@GenderCodeFilter) > 0  THEN '(' + @BirthYearFilter + ') AND ' + @GenderCodeFilter ELSE '(' + @BirthYearFilter + ')' END   
						WHEN LEN(@GenderCodeFilter) > 0 		   
							THEN    
								@GenderCodeFilter   
						ELSE ''   
					END   
				,@Names = LTRIM(RTRIM(@Names))
   
			IF OBJECT_ID ('TEMPDB..#FTSearchResults') IS NOT NULL
				DROP TABLE #FTSearchResults; 

			CREATE TABLE #FTSearchResults
				([Key] int primary Key,
				[Rank] int, 
				[SearchType] tinyint, 
				Iteration tinyint)
   
			DECLARE    
				@CandidateRows int = 1000000
				,@TotalRows int = 0	   
   
			IF @FTCriteria IS NULL   
			BEGIN   
			   
				SET @CriteriaXML = [FT].[QuickSearchCriteria](@SearchString, @FTFilter, @FuzzyFactor, 1)   
				SELECT    
					@FTSearchStr  = @CriteriaXML.value('(/Criteria)[1]','varchar(8000)')   
					,@TermCriteria  = @CriteriaXML.value('(/TermCriteria)[1]','varchar(8000)')   
					,@FuzzyConsonantCriteria  = @CriteriaXML.value('(/FuzzyConsonantCriteria)[1]','varchar(8000)')   
					,@FuzzySpecificCriteria  = @CriteriaXML.value('(/FuzzySpecificCriteria)[1]','varchar(8000)')   
					,@FuzzyBroadCriteria  = @CriteriaXML.value('(/FuzzyBroadCriteria)[1]','varchar(8000)')   
					,@SearchTerm1 = @CriteriaXML.value('(/SearchTerm1)[1]','varchar(50)')   
					,@SearchTerm2 = @CriteriaXML.value('(/SearchTerm2)[1]','varchar(50)')   
					,@SearchTerm3 = @CriteriaXML.value('(/SearchTerm3)[1]','varchar(50)')   
					,@SearchTerm4 = @CriteriaXML.value('(/SearchTerm4)[1]','varchar(50)')   
					,@SearchTerm5 = @CriteriaXML.value('(/SearchTerm5)[1]','varchar(50)')   
					,@SearchTerm6 = @CriteriaXML.value('(/SearchTerm6)[1]','varchar(50)')  
					,@SearchTermCount = @CriteriaXML.value('(/SearchTermCount)[1]','varchar(50)')   
   
				END   
			ELSE   
			BEGIN   
				SELECT    
						@TermCriteria = @FTCriteria   
						,@FuzzyConsonantCriteria = @FTCriteria   
						,@FuzzySpecificCriteria = @FTCriteria   
						,@FuzzyBroadCriteria = @FTCriteria   
			END   
   
			IF LEN(@FuzzyConsonantCriteria) = 0 SET @FuzzyConsonantCriteria = @TermCriteria   
   
			-- Search on word matches first   
			INSERT INTO #FTSearchResults   
			SELECT    
				FT1.[Key]   
				,Rank   
				,SearchType = @SearchType   
				,Iteration = @Iteration   
			FROM    
				Containstable (FT.Apprentice,[SearchTermList], @TermCriteria ,@CandidateRows) FT1   
				INNER JOIN [FT].[Apprentice] FA
					ON FA.[Key] = FT1.[Key]				
				INNER JOIN dbo.Apprentice A
					ON FA.[ApprenticeId] = A.ApprenticeId
			WHERE 
				A.BirthDate = @BirthDate
			   
			IF @@ROWCOUNT > 0 SELECT @TotalRows = @TotalRows + @@ROWCOUNT, @SearchType = @SearchType + 1   
   
			-- if not enough results then search again increaseing fuzzy level   
			IF @TotalRows <= (@MaxRows) AND @FuzzyFactor >= 2    
			BEGIN    
				INSERT INTO #FTSearchResults   
				SELECT    
					FT1.[Key]   
					,Rank   
					,SearchType = @SearchType   
					,Iteration	= @Iteration   
				FROM    
					Containstable (FT.Apprentice,[SearchFuzzyTermList], @FuzzyConsonantCriteria, @CandidateRows) FT1	   
					INNER JOIN [FT].[Apprentice] FA
						ON FA.[Key] = FT1.[Key]				
					INNER JOIN dbo.Apprentice A
						ON FA.[ApprenticeId] = A.ApprenticeId
				WHERE    
					A.BirthDate = @BirthDate
					AND FT1.[Key] NOT IN ( SELECT [Key] FROM #FTSearchResults)   
   
				SELECT @TotalRows = @TotalRows + @@ROWCOUNT   
			END   
   
			SET @SearchType = @SearchType + @Increment   
   
			-- if not enough results then search again increaseing fuzzy level   
			IF @TotalRows <= @MaxRows AND @FuzzyFactor >= 3 AND LEN(@FuzzySpecificCriteria) > 0 AND @QualityCode <> 'H'   
			BEGIN    
				INSERT INTO #FTSearchResults   
				SELECT    
					FT1.[Key]   
					,Rank   
					,SearchType = @SearchType   
					,Iteration = @Iteration   
				FROM    
					Containstable (FT.Apprentice,[SearchFuzzyTermList], @FuzzySpecificCriteria, @CandidateRows) FT1	 
					INNER JOIN [FT].[Apprentice] FA
						ON FA.[Key] = FT1.[Key]				
					INNER JOIN dbo.Apprentice A
						ON FA.[ApprenticeId] = A.ApprenticeId
				WHERE    
					A.BirthDate = @BirthDate
					AND FT1.[Key] NOT IN ( SELECT [Key] FROM #FTSearchResults)   
	   
				SELECT @TotalRows = @TotalRows + @@ROWCOUNT   
			END   
   
			SET @SearchType = @SearchType + @Increment   
   
			-- if not enough results then search again increaseing fuzzy level   
			IF @TotalRows <= @MaxRows AND @FuzzyFactor >= 4 AND LEN(@FuzzyBroadCriteria) > 0  AND @QualityCode <> 'H'   
			BEGIN    
				INSERT INTO #FTSearchResults   
				SELECT    
					FT1.[Key]   
					,Rank   
					,SearchType = @SearchType   
					,Iteration = @Iteration   
				FROM    
					Containstable (FT.Apprentice,[SearchFuzzyTermList], @FuzzyBroadCriteria, @CandidateRows) FT1
					INNER JOIN [FT].[Apprentice] FA
						ON FA.[Key] = FT1.[Key]				
					INNER JOIN dbo.Apprentice A
						ON FA.[ApprenticeId] = A.ApprenticeId
				WHERE  
					A.BirthDate = @BirthDate
					AND FT1.[Key] NOT IN ( SELECT [Key] FROM #FTSearchResults)   
					   
				SELECT @TotalRows = @TotalRows + @@ROWCOUNT   
   
			END   
			SET @SearchType = @SearchType + @Increment   
 
			INSERT INTO #NamesResults
				([ApprenticeId]
				,[ProfileTypeCode]
				,[FirstName]
				,[OtherNames]
				,[Surname]
				,[GenderCode]
				,[BirthDate]
				,[PreferredName]
				,[EmailAddress]
				,[PhoneNumber]
				,QualitySortOrder
				,SearchType
				,Iteration
				,ScoreValue
				)
					SELECT   
						A.[ApprenticeId]   
						,A.[ProfileTypeCode]   
						,[FirstName]   
						,[OtherNames]   
						,[Surname]   
						,[GenderCode]   
						,[BirthDate]   
						,[PreferredName]
						,A.[EmailAddress]
						,NULL
						,QualitySortOrder =    
								CASE    
									WHEN SearchType = 1 THEN 1   
									ELSE 2   
								END   
						,SearchType   
						,Iteration   
						,ScoreValue =    20
					FROM    
						#FTSearchResults FT   
						JOIN [FT].[Apprentice] FTA ON FT.[Key] = FTA.[Key]   
						JOIN [dbo].[Apprentice] A  WITH ( NOLOCK) ON FTA.ApprenticeId = A.ApprenticeId   
					WHERE    
						A.ActiveFlag = 1
						AND A.DeceasedFlag = 0

			
			UPDATE X
			SET [NamesMatchFlag] = 1
				,ScoreValue = X.ScoreValue + 1
			FROM #InterimResults X
			INNER JOIN #NamesResults NR
				ON X.ApprenticeId = NR.ApprenticeId

			INSERT INTO #ResultNamesTable (ApprenticeId, Name)
			select ApprenticeId, Value
			from #NamesResults
			CROSS APPLY STRING_SPLIT( CONCAT_WS( ' ', FirstName, OtherNames, Surname, PreferredName), ' ')

			UPDATE X
			SET ScoreValue = ScoreValue + (5 * (1+NameMatches))
			FROM #InterimResults X
			INNER JOIN (SELECT ApprenticeId, COUNT(*) NameMatches
						FROM #ResultNamesTable RNT
						INNER JOIN @NamesTable NT
							ON RNT.Name = NT.Name COLLATE LATIN1_GENERAL_CI_AS
						GROUP BY ApprenticeId
						) Y
				ON X.ApprenticeId = Y.ApprenticeId
			WHERE [NamesMatchFlag] = 1

			UPDATE X
			SET ScoreValue = ScoreValue +3
			FROM #InterimResults X
			WHERE [NamesMatchFlag] = 1
				AND REPLACE([Surname],' ','') LIKE '%'+REPLACE(@Names,' ','')+'%'

			UPDATE X
			SET ScoreValue = ScoreValue +3
			FROM #InterimResults X
			WHERE [NamesMatchFlag] = 1
				AND REPLACE([FirstName],' ','') LIKE '%'+REPLACE(@Names,' ','')+'%'

			UPDATE X
			SET ScoreValue = ScoreValue +10
				,[FirstNameMatch] = 1
			FROM #InterimResults X
			WHERE [NamesMatchFlag] = 1
				AND REPLACE([FirstName],' ','') LIKE REPLACE(@FirstName,' ','')+'%'
				AND @FirstName <> ''

			UPDATE X
			SET ScoreValue = ScoreValue +10
				,[SurnameMatch] = 1
			FROM #InterimResults X
			WHERE [NamesMatchFlag] = 1
				AND REPLACE([Surname],' ','') LIKE REPLACE(@Surname,' ','')+'%'
				AND @Surname <> ''

			UPDATE X
			SET ScoreValue = ScoreValue +10
				,[FirstNameMatch] = 1
			FROM #InterimResults X
			WHERE [NamesMatchFlag] = 1
				AND REPLACE([FirstName],' ','') LIKE REPLACE(@FirstName,' ','')
				AND @FirstName <> ''

			UPDATE X
			SET ScoreValue = ScoreValue +10
				,[SurnameMatch] = 1
			FROM #InterimResults X
			WHERE [NamesMatchFlag] = 1
				AND REPLACE([Surname],' ','') LIKE REPLACE(@Surname,' ','')
				AND @Surname <> ''
		END

----End Names

		UPDATE X
		SET [ResidentialAddress] = CONCAT_WS(' ',AA.StreetAddress1,AA.StreetAddress2,AA.StreetAddress3,AA.Locality,AA.Postcode,AA.StateCode)
			,[ResidentialStateCode] = AA.StateCode
		FROM #InterimResults X
		INNER JOIN dbo.ApprenticeAddress AA
			ON X.ApprenticeId = AA.ApprenticeId
			AND AA.AddressTypeCode = 'RESD' 

		UPDATE X
		SET [Phone1InternationalPrefix] = AP.[InternationalPrefix]
			,[Phone1] = AP.[PhoneNumber]
		FROM #InterimResults X
		INNER JOIN dbo.ApprenticePhone AP
			ON X.ApprenticeId = AP.ApprenticeId
			AND AP.PhoneTypeCode = 'PHONE1';

		UPDATE X
		SET [Phone2InternationalPrefix] = AP.[InternationalPrefix]
			,[Phone2] = AP.[PhoneNumber]
		FROM #InterimResults X
		INNER JOIN dbo.ApprenticePhone AP
			ON X.ApprenticeId = AP.ApprenticeId
			AND AP.PhoneTypeCode = 'PHONE2';

		SELECT TOP (@MaxRows)   
			Results.[ApprenticeId]   
			,Results.[FirstName]   
			,Results.[OtherNames] 
			,Results.[Surname]   
			,Results.[BirthDate]
			,Results.[EmailAddress]
			,Results.USI
			,Results.Phone1InternationalPrefix
			,Results.Phone1
			,Results.Phone2InternationalPrefix
			,Results.Phone2
			,Results.ResidentialAddress
			,ScoreValue
			,CAST(ISNULL([USIMatch],0) AS BIT) AS [USIMatch]
			,CAST(ISNULL([PhoneNumberMatch],0) AS BIT) AS [PhoneNumberMatch]
			,CAST(ISNULL([EmailMatch],0) AS BIT) AS [EmailMatch]
			,CAST(ISNULL([BirthDateMatch],0) AS BIT) AS [BirthDateMatch]
			,CAST(ISNULL([FirstNameMatch],0) AS BIT) AS [FirstNameMatch]
			,CAST(ISNULL([SurnameMatch],0) AS BIT) AS [SurnameMatch]
		FROM    
			#InterimResults Results
		INNER JOIN (SELECT ApprenticeId
						,MAX([USIMatch]) AS [USIMatch]
						,MAX([PhoneNumberMatch]) AS [PhoneNumberMatch]
						,MAX([EmailMatch]) AS [EmailMatch]
						,MAX([BirthDateMatch]) AS [BirthDateMatch]
					FROM #ApprenticeMatches
					GROUP BY ApprenticeId) Y
			ON Results.ApprenticeId = Y.ApprenticeId
		WHERE ScoreValue > 100
		ORDER BY    
			QualitySortOrder   
			,ScoreValue desc   
		OPTION(FORCE ORDER)   
		   
	END TRY

	BEGIN CATCH
		THROW;
	END CATCH