
/****************************************************************************
 
Object:			[dbo].[ApprenticeAdvancedSearch]

Description:	Returns apprentices that meet the search criteria.  A ranking is provided based on most
				likely match.

				Name fuzzy searching logic based on EMP.[jobseeker].[JobSeekerFuzzySearch] by Mick Vullo

				If @Names is provided it will return records which match in any of FirstName, OtherNames, PreferredName or Surname.  A
					higher rating will be calculated based on more name matches.  @Names will also be used to perform fuzzy matching 
					though these will recieve a lower score.  Where there is no match on @Names, records will still be returned if there
					is an exact match by USI, ApprenticeId, Email or PhoneNumber.
				If @BirthDate is provided, only records which match are returned.
				If @GenderCode is provided, it is only used to aid fuzzy search logic.
				If @EmailAddress is provided it will return an exact match.  If only a partial match it will exclude non matches
					unless there is an exact match by USI, ApprenticeId or PhoneNumber.
				If @PhoneNumber is provided it will return matches to the rightmost 8 digits.
				If @AddressString is provided it will be used to add weight to matches.  Records that don't have a match
					on address will be excluded unless it is an exact match on USI, ApprenticeId, Email or PhoneNumber.
				If @USI is provided, only records which match are returned.
				If @ApprenticeId is provided, only records which match are returned.

Usage:       
				USE ADMSApprentice

				EXEC [dbo].[ApprenticeAdvancedSearch]
					@Names	= 'Trot'   
					,@EmailAddress = 'arcb@btac.nsw.edu.au'
					,@AddressString = 'thornton'


Author:			JD3044
  
Created:		2021-05-26

Modification History:
USERID	Date		      Description
=======	==========		====================================================
***************************************************************************/
CREATE PROC [dbo].[ApprenticeAdvancedSearch]   
	@Names					[varchar](150) = '' 
	,@BirthDate				[Date] = null
	,@GenderCode			[varchar](10) = null
	,@EmailAddress			[varchar](320) = ''
	,@PhoneNumber			[varchar](15) = ''
	,@AddressString			[varchar](100) = ''
	,@USI					[varchar](10) = ''
	,@ApprenticeId			[int] = null
	,@MaxRows				[int] = 100   
	,@QualityCode			[varchar](10) = 'M'   
	,@FuzzyFactor			[int] = 4   
	,@FTCriteria			[varchar](1000) = NULL   
	,@UserId				[varchar](50) = null   
AS   
   
	SET NOCOUNT ON;

	BEGIN TRY

		IF @Names IS NULL
			SET @Names = ''
		IF @EmailAddress IS NULL
			SET @EmailAddress = ''
		IF @PhoneNumber IS NULL
			SET @PhoneNumber = ''
		IF @USI IS NULL
			SET @USI = ''
		IF @AddressString IS NULL
			SET @AddressString = ''

		SET @AddressString = LTRIM(RTRIM(@AddressString))
		SET @AddressString = REPLACE(REPLACE(@AddressString,'<',''),'>','')
		SET @AddressString  = REPLACE(REPLACE(REPLACE(@AddressString,' ','<>'),'><',''),'<>',' ')

		IF @Names = '' AND @USI = '' AND @ApprenticeId IS NULL AND @AddressString = '' AND @EmailAddress = '' AND @PhoneNumber = '' AND @BirthDate IS NULL
			RAISERROR ('At least one of Name, USI, ApprenticeId, EmailAddres, PhoneNumber, BirthDate or AddressString must be provided.',16,1)

		IF @Names = '' AND @USI = '' AND @ApprenticeId IS NULL AND @EmailAddress = '' AND @PhoneNumber = '' AND @BirthDate IS NULL AND LTRIM(RTRIM(@AddressString)) IN ('NSW','VIC','SA','WA','TAS','ACT','QLD','NT')
			RAISERROR ('At least one of Name, USI, EmailAddres, PhoneNumber, BirthDate or ApprenticeId must be provided if AddressString is just a state.',16,1)

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
			,[PhoneNumber] VARCHAR(100)
			,[USI] VARCHAR(10)
			,[AddressMatchFlag] BIT
			,[ResidentialAddress] VARCHAR(350)
			,[ResidentialStateCode] VARCHAR(10)
			,[PostalAddress] VARCHAR(350)
			,[PostalStateCode] VARCHAR(10)
			,[NamesMatchFlag] BIT
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

		DECLARE @ResultNamesTable TABLE
			([ApprenticeId] INT
			,[Name] VARCHAR(50))

		INSERT INTO @NamesTable ([Name])
		SELECT value FROM STRING_SPLIT(@Names, ' ');

		DECLARE @AddressTable TABLE
			([Address] VARCHAR(350))

		DECLARE @ResultAddressTable TABLE
			([ApprenticeId] INT
			,[Address] VARCHAR(350))

		INSERT INTO @AddressTable ([Address])
		SELECT value FROM STRING_SPLIT(@AddressString, ' ');

		IF (SELECT COUNT(*) FROM @AddressTable WHERE Address IN ('NSW','VIC','SA','WA','TAS','ACT','QLD','NT')) > 1
			RAISERROR ('No more that 1 state code may be provided in AddressString.',16,1)

  
		DECLARE    
			 @SearchString  varchar(200) = null   
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
			,@AddressStateOnly bit = 0
			,@StateCode varchar(10)

		--IF (SELECT COUNT(*) FROM @AddressTable WHERE Address NOT IN ('NSW','VIC','SA','WA','TAS','ACT','QLD','NT')) = 0

		SELECT @StateCode = [Address] FROM @AddressTable WHERE Address IN ('NSW','VIC','SA','WA','TAS','ACT','QLD','NT')

		IF LTRIM(RTRIM(@AddressString)) IN ('NSW','VIC','SA','WA','TAS','ACT','QLD','NT')
			SET @AddressString = ''

		IF @ApprenticeId IS NOT NULL
		BEGIN
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
			,[PhoneNumber]
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
			WHERE A.ApprenticeId = @ApprenticeId
				AND A.ActiveFlag = 1
				AND A.DeceasedFlag = 0
		END

		IF LEN(@USI) > 0
		BEGIN
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
			,[PhoneNumber]
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
				,AU.[USI]
				,1
				,1
				,1
				,1
				,100
			FROM dbo.Apprentice A
			INNER JOIN dbo.ApprenticeUSI AU
				ON A.ApprenticeId = AU.ApprenticeId
			WHERE AU.ActiveFlag = 1
				AND AU.USI = @USI
				AND A.ActiveFlag = 1
				AND A.DeceasedFlag = 0
				AND A.ApprenticeId NOT IN (SELECT ApprenticeId FROM #InterimResults)
		END

		IF LEN(@EmailAddress) > 4
		BEGIN
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
			,[PhoneNumber]
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
			WHERE A.EmailAddress = @EmailAddress
				AND A.ActiveFlag = 1
				AND A.DeceasedFlag = 0
				AND A.ApprenticeId NOT IN (SELECT ApprenticeId FROM #InterimResults)


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
			,[PhoneNumber]
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
				,AU.[USI]
				,1
				,1
				,1
				,1
				,30
			FROM dbo.Apprentice A
			LEFT JOIN dbo.ApprenticeUSI AU
				ON A.ApprenticeId = AU.ApprenticeId
				AND AU.ActiveFlag = 1
			WHERE A.EmailAddress LIKE @EmailAddress+'%'
				AND A.ActiveFlag = 1
				AND A.DeceasedFlag = 0
				AND A.ApprenticeId NOT IN (SELECT ApprenticeId FROM #InterimResults)

		END

		IF LEN(@PhoneNumber) > 7
		BEGIN
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
			,[PhoneNumber]
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
			WHERE A.ActiveFlag = 1
				AND A.DeceasedFlag = 0
				AND A.ApprenticeId NOT IN (SELECT ApprenticeId FROM #InterimResults)
				AND A.ApprenticeId IN (SELECT ApprenticeId FROM dbo.ApprenticePhone AP WHERE AP.PhoneNumber = @PhoneNumber)

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
			,[PhoneNumber]
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
				,AU.[USI]
				,1
				,1
				,1
				,1
				,95
			FROM dbo.Apprentice A
			LEFT JOIN dbo.ApprenticeUSI AU
				ON A.ApprenticeId = AU.ApprenticeId
				AND AU.ActiveFlag = 1
			WHERE A.ActiveFlag = 1
				AND A.DeceasedFlag = 0
				AND A.ApprenticeId NOT IN (SELECT ApprenticeId FROM #InterimResults)
				AND A.ApprenticeId IN (SELECT ApprenticeId FROM dbo.ApprenticePhone AP WHERE AP.PhoneNumber LIKE '%'+@PhoneNumber)
		END

		IF @BirthDate IS NOT NULL
		BEGIN
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
			,[PhoneNumber]
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
				,AU.[USI]
				,1
				,1
				,1
				,1
				,30
			FROM dbo.Apprentice A
			LEFT JOIN dbo.ApprenticeUSI AU
				ON A.ApprenticeId = AU.ApprenticeId
				AND AU.ActiveFlag = 1
			WHERE A.[BirthDate] = @BirthDate
				AND A.ActiveFlag = 1
				AND A.DeceasedFlag = 0
				AND A.ApprenticeId NOT IN (SELECT ApprenticeId FROM #InterimResults)
		END

		IF LEN(@AddressString) > 0
		BEGIN
			IF OBJECT_ID ('TEMPDB..#AddressResults') IS NOT NULL
				DROP TABLE #AddressResults; 

			CREATE TABLE #AddressResults
				(ApprenticeAddressId INT,
				ApprenticeId INT,
				[StreetAddress1] [varchar](100),
				[StreetAddress2] [varchar](100),
				[StreetAddress3] [varchar](100),
				[Locality] [varchar](50),
				[StateCode] [varchar](10),
				[Postcode] [varchar](10),
				[Rank] INT
				)

			IF OBJECT_ID ('TEMPDB..#AddressRank') IS NOT NULL
				DROP TABLE #AddressRank; 

			CREATE TABLE #AddressRank
				(ApprenticeAddressId INT,
				[Rank] INT
				)

			SET @AddressString = REPLACE(@AddressString,' ',' OR ')

			INSERT INTO #AddressRank (ApprenticeAddressId, [Rank])
			SELECT [KEY], [Rank]
			FROM CONTAINSTABLE(
				[dbo].[ApprenticeAddress],
				--([Locality],[StreetAddress1],[StreetAddress2],[StreetAddress2],[Postcode],[StateCode]),
				([Locality],[StreetAddress1],[StreetAddress2],[StreetAddress2],[Postcode]),
				@AddressString
			)

			INSERT INTO #AddressResults (ApprenticeAddressId,
				ApprenticeId,
				[StreetAddress1],
				[StreetAddress2],
				[StreetAddress3],
				[Locality],
				[StateCode],
				[Postcode],
				[Rank])
			SELECT AA.[ApprenticeAddressId],
				[ApprenticeId],
				[StreetAddress1],
				[StreetAddress2],
				[StreetAddress3],
				[Locality],
				[StateCode],
				[Postcode],
				[Rank]
			FROM dbo.ApprenticeAddress AA
			INNER JOIN #AddressRank AR
				ON AA.ApprenticeAddressId = AR.ApprenticeAddressId

			UPDATE X
			SET [AddressMatchFlag] = 1
			FROM #InterimResults X
			INNER JOIN dbo.Apprentice A
				ON X.ApprenticeId = A.ApprenticeId
			INNER JOIN #AddressResults AR
				ON A.ApprenticeId = AR.ApprenticeId

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
			,[PhoneNumber]
			,[USI]
			,[AddressMatchFlag]
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
				,AU.[USI]
				,1
				,1
				,1
				,1
				,1
				,30
			FROM dbo.Apprentice A
			INNER JOIN #AddressResults AR
				ON A.ApprenticeId = AR.ApprenticeId
			LEFT JOIN dbo.ApprenticeUSI AU
				ON A.ApprenticeId = AU.ApprenticeId
				AND AU.ActiveFlag = 1
			WHERE A.ActiveFlag = 1
				AND A.DeceasedFlag = 0
				AND A.ApprenticeId NOT IN (SELECT ApprenticeId FROM #InterimResults)

			INSERT INTO @ResultAddressTable (ApprenticeId, Address)
			select R.ApprenticeId, Value
			from #InterimResults R
			INNER JOIN dbo.ApprenticeAddress AA
				ON R.ApprenticeId = AA.ApprenticeId
				AND AA.AddressTypeCode = 'RESD'
				--CROSS APPLY STRING_SPLIT( CONCAT_WS(' ',AA.StreetAddress1,AA.StreetAddress2,AA.StreetAddress3,AA.Locality,AA.Postcode,AA.StateCode), ' ')
			CROSS APPLY STRING_SPLIT( CONCAT_WS(' ',AA.StreetAddress1,AA.StreetAddress2,AA.StreetAddress3,AA.Locality,AA.Postcode), ' ')

			UPDATE X
			SET ScoreValue = ScoreValue + (5*AddressMatches)
			FROM #InterimResults X
			INNER JOIN (
					SELECT ApprenticeId, CASE WHEN EXISTS (SELECT Address, count(*) FROM @AddressTable group by Address having count(*) > 1) THEN COUNT(*) ELSE COUNT(DISTINCT RNT.Address) END AS AddressMatches
					--SELECT ApprenticeId, COUNT(*) AddressMatches
					--SELECT ApprenticeId, COUNT(DISTINCT RNT.Address) AddressMatches
					FROM @ResultAddressTable RNT
					INNER JOIN @AddressTable NT
						ON RNT.Address = NT.Address
						AND ISNUMERIC(NT.Address) = 0
					GROUP BY ApprenticeId
					) Y
						ON X.ApprenticeId = Y.ApprenticeId
			WHERE [AddressMatchFlag] = 1

			UPDATE X
			SET ScoreValue = ScoreValue + (3*AddressMatches)
			FROM #InterimResults X
			INNER JOIN (
					SELECT ApprenticeId, CASE WHEN EXISTS (SELECT Address, count(*) FROM @AddressTable group by Address having count(*) > 1) THEN COUNT(*) ELSE COUNT(DISTINCT RNT.Address) END AS AddressMatches
					--SELECT ApprenticeId, COUNT(*) AddressMatches
					--SELECT ApprenticeId, COUNT(DISTINCT RNT.Address) AddressMatches
					FROM @ResultAddressTable RNT
					INNER JOIN @AddressTable NT
						ON RNT.Address = NT.Address
						AND ISNUMERIC(NT.Address) = 1
					GROUP BY ApprenticeId
					) Y
						ON X.ApprenticeId = Y.ApprenticeId
			WHERE [AddressMatchFlag] = 1

		END

--End Address


--Names

		IF LEN (@Names) > 0
		BEGIN

			SET @SearchString = @Names    
	   
			IF @GenderCode IS NOT NULL SET @GenderCodeFilter = '0' + @GenderCode --Note prefix at the start as some metaphone terms can end with a '0' which will end up with false positive results	   
	   
	   
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
   
			--SELECT @FTFilter   
			DECLARE @FTSearchResults TABLE ( [Key] int primary Key, [Rank] int, [SearchType] tinyint, Iteration tinyint )   
   
			DECLARE    
				@CandidateRows int    
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
   
			SET @CandidateRows = @MaxRows * 100   
	   
			-- Search on word matches first   
			INSERT INTO @FTSearchResults   
			SELECT    
				[Key]   
				,Rank   
				,SearchType = @SearchType   
				,Iteration = @Iteration   
			FROM    
				Containstable (FT.Apprentice,[SearchTermList], @TermCriteria ,@CandidateRows) FT1   
			   
			IF @@ROWCOUNT > 0 SELECT @TotalRows = @TotalRows + @@ROWCOUNT, @SearchType = @SearchType + 1   
   
			-- if not enough results then search again increaseing fuzzy level   
			IF @TotalRows <= (@MaxRows) AND @FuzzyFactor >= 2    
			BEGIN    
				INSERT INTO @FTSearchResults   
				SELECT    
					[Key]   
					,Rank   
					,SearchType = @SearchType   
					,Iteration	= @Iteration   
				FROM    
					Containstable (FT.Apprentice,[SearchFuzzyTermList], @FuzzyConsonantCriteria, @CandidateRows) FT1	   
				WHERE    
					[Key] NOT IN ( SELECT [Key] FROM @FTSearchResults)   
   
				SELECT @TotalRows = @TotalRows + @@ROWCOUNT   
			END   
   
			SET @SearchType = @SearchType + @Increment   
   
			-- if not enough results then search again increaseing fuzzy level   
			IF @TotalRows <= @MaxRows AND @FuzzyFactor >= 3 AND LEN(@FuzzySpecificCriteria) > 0 AND @QualityCode <> 'H'   
			BEGIN    
				INSERT INTO @FTSearchResults   
				SELECT    
					[Key]   
					,Rank   
					,SearchType = @SearchType   
					,Iteration = @Iteration   
				FROM    
					Containstable (FT.Apprentice,[SearchFuzzyTermList], @FuzzySpecificCriteria, @CandidateRows) FT1	   
				WHERE    
					[Key] NOT IN ( SELECT [Key] FROM @FTSearchResults)   
	   
				SELECT @TotalRows = @TotalRows + @@ROWCOUNT   
			END   
   
			SET @SearchType = @SearchType + @Increment   
   
			-- if not enough results then search again increaseing fuzzy level   
			IF @TotalRows <= @MaxRows AND @FuzzyFactor >= 4 AND LEN(@FuzzyBroadCriteria) > 0  AND @QualityCode <> 'H'   
			BEGIN    
				INSERT INTO @FTSearchResults   
				SELECT    
						[Key]   
					,Rank   
					,SearchType = @SearchType   
					,Iteration = @Iteration   
				FROM    
					Containstable (FT.Apprentice,[SearchFuzzyTermList], @FuzzyBroadCriteria, @CandidateRows) FT1	   
				WHERE    
					[Key] NOT IN ( SELECT [Key] FROM @FTSearchResults)   
					   
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
						@FTSearchResults FT   
						JOIN [FT].[Apprentice] FTA ON FT.[Key] = FTA.[Key]   
						JOIN [dbo].[Apprentice] A  WITH ( NOLOCK) ON FTA.ApprenticeId = A.ApprenticeId   
					WHERE    
						A.ActiveFlag = 1
						AND A.DeceasedFlag = 0

			
			UPDATE X
			SET [NamesMatchFlag] = 1
			FROM #InterimResults X
			INNER JOIN #NamesResults NR
				ON X.ApprenticeId = NR.ApprenticeId

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
			,[PhoneNumber]
			,[USI]
			,[NamesMatchFlag]
			,QualitySortOrder
			,SequenceNo
			,SearchType
			,Iteration
			,ScoreValue
			)
			SELECT NR.ApprenticeId
				,NR.[ProfileTypeCode]
				,NR.[FirstName]
				,NR.[OtherNames]
				,NR.[Surname]
				,NR.[GenderCode]
				,NR.[BirthDate]
				,NR.[PreferredName]
				,NR.[EmailAddress]
				,NULL
				,AU.[USI]
				,1
				,NR.QualitySortOrder
				,1
				,NR.SearchType
				,1
				,40
			FROM #NamesResults NR
			LEFT JOIN dbo.ApprenticeUSI AU
				ON NR.ApprenticeId = AU.ApprenticeId
				AND AU.ActiveFlag = 1
			WHERE NR.ApprenticeId NOT IN (SELECT ApprenticeId FROM #InterimResults)

			INSERT INTO @ResultNamesTable (ApprenticeId, Name)
			select ApprenticeId, Value
			from #NamesResults
			CROSS APPLY STRING_SPLIT( CONCAT_WS( ' ', FirstName, OtherNames, Surname, PreferredName), ' ')

			UPDATE X
			SET ScoreValue = ScoreValue * (1+NameMatches)
			FROM #InterimResults X
			INNER JOIN (SELECT ApprenticeId, COUNT(*) NameMatches
						FROM @ResultNamesTable RNT
						INNER JOIN @NamesTable NT
							ON RNT.Name = NT.Name
						GROUP BY ApprenticeId
						) Y
				ON X.ApprenticeId = Y.ApprenticeId
			WHERE [NamesMatchFlag] = 1

			UPDATE X
			SET ScoreValue = ScoreValue +1
			FROM #InterimResults X
			WHERE [NamesMatchFlag] = 1
				AND REPLACE([Surname],' ','') LIKE '%'+REPLACE(@Names,' ','')+'%'
		END

--End Names


		UPDATE X
		SET [ResidentialAddress] = CONCAT_WS(' ',AA.StreetAddress1,AA.StreetAddress2,AA.StreetAddress3,AA.Locality,AA.Postcode,AA.StateCode)
			,[ResidentialStateCode] = AA.StateCode
		FROM #InterimResults X
		INNER JOIN dbo.ApprenticeAddress AA
			ON X.ApprenticeId = AA.ApprenticeId
			AND AA.AddressTypeCode = 'RESD' 

		UPDATE X
		SET [PostalAddress] = CONCAT_WS(' ',AA.StreetAddress1,AA.StreetAddress2,AA.StreetAddress3,AA.Locality,AA.Postcode,AA.StateCode)
			,[PostalStateCode] = AA.StateCode
		FROM #InterimResults X
		INNER JOIN dbo.ApprenticeAddress AA
			ON X.ApprenticeId = AA.ApprenticeId
			AND AA.AddressTypeCode = 'POST'

		UPDATE X
		SET PhoneNumber = (SELECT LEFT( STRING_AGG (PhoneTypeCode+':'+PhoneNumber,', ') WITHIN GROUP (ORDER BY ApprenticePhoneId DESC),100)
							FROM dbo.ApprenticePhone AP
							WHERE ApprenticePhoneId IN (SELECT TOP 4 ApprenticePhoneId FROM dbo.ApprenticePhone WHERE ApprenticeId = X.ApprenticeId ORDER BY ApprenticePhoneId DESC)
							GROUP BY ApprenticeId)
		FROM #InterimResults X

		DELETE X
		FROM #InterimResults X
		WHERE (@ApprenticeId IS NOT NULL AND X.ApprenticeId <> @ApprenticeId)
			OR (LEN(@USI) > 0 AND ISNULL(X.USI,'') <> @USI)
			OR (LEN(@EmailAddress) > 4 AND ISNULL(X.EmailAddress,'') NOT LIKE '%'+@EmailAddress+'%')
			OR (LEN(@PhoneNumber) > 7 AND ISNULL(X.PhoneNumber,'') NOT LIKE '%'+@PhoneNumber+'%')
			OR (@BirthDate IS NOT NULL AND X.BirthDate <> @BirthDate)
			OR (LEN(@AddressString) > 0 AND ISNULL([AddressMatchFlag],0) <> 1)
			OR (LEN(@Names) > 0 AND ISNULL([NamesMatchFlag],0) <> 1)
			OR (@StateCode IS NOT NULL AND ISNULL(X.ResidentialStateCode,'') <> @StateCode AND ISNULL(X.PostalStateCode,'') <> @StateCode)


			SELECT TOP (@MaxRows)   
				Results.[ApprenticeId]   
				,Results.[ProfileTypeCode]   
				,Results.[FirstName]   
				,Results.[Surname]   
				,Results.[OtherNames] 
				,Results.[BirthDate]
				,Results.[EmailAddress]
				,Results.USI
				,Results.PhoneNumber
				,Results.ResidentialAddress
				,Results.PostalAddress
				,ScoreValue    
			FROM    
				#InterimResults Results
			ORDER BY    
				QualitySortOrder   
				,ScoreValue desc   
			OPTION(FORCE ORDER)   
		   
			SET @RowCount = @@RowCount   
	   
		-- Log the search for diagnostic purposes   
		INSERT INTO [FT].[SearchLog] ([RecordSource],[ExecutionDate],[ExecutionUserId], [Duration],[MaxRows],[RowCount],[Filter],[SearchString],[FTCriteria])   
		VALUES ( 'APP', GETDATE(), ISNULL(@UserId,SYSTEM_USER), DATEDIFF(MILLISECOND,@StartTime,GETDATE()), @MaxRows, @RowCount, NULL, @SearchString, @FTSearchStr)   
   
	END TRY

	BEGIN CATCH
		THROW;
	END CATCH