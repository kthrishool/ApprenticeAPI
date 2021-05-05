
 
CREATE PROC [dbo].[ApprenticeFuzzySearch]   
	 @Surname			[varchar](50) = ''   
	,@FirstName			[varchar](50) =''   
	,@BirthDateStartRange	[Date] = null   
	,@BirthDateEndRange		[Date] = null   
	,@GenderCode			[varchar](10) = null   
	,@MaxRows int = 100   
	,@QualityCode varchar(10) = 'M'   
	,@FuzzyFactor int = 4   
	,@FTCriteria varchar(1000) = NULL   
	,@UserId varchar(50) = null   
AS   
/*--------------------------------------------------------------------------------   
Based on EMP.[jobseeker].[JobSeekerFuzzySearch] by Mick Vullo 
----------------------------------------------------------------------------------*/   
   
   
	SET NOCOUNT ON;   
   
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
		,@LeastSignificantTerm varchar(50)   
		,@SearchTermCount int   
		,@FTFilter varchar(200)   
		,@SearchType int = 1   
		,@Increment tinyint = 1   
		,@Iteration tinyint = 1   
   
		,@NumericSearchString varchar(50)   
		,@ApprenticeId int   
		,@CRNNumber char(10)   
		   
		,@LongBirthYearFilterRange bit = 0    
		,@BirthYearFilter varchar(1000)   = ''   
		,@GenderCodeFilter	  varchar(10) = ''	   
		,@OtherNames varchar(30) = ''   
	   
	SET @SearchString = @FirstName + ' ' + @Surname    
	   
	IF @BirthDateStartRange IS NOT NULL   
	BEGIN    
		IF @BirthDateEndRange IS NULL SET @BirthDateEndRange = @BirthDateStartRange   
   
		IF DateDiff(Year, @BirthDateStartRange,@BirthDateEndRange)  <= 5   
		BEGIN    
			declare @i int = YEAR(@BirthDateStartRange)   
			WHILE @i <= YEAR(@BirthDateEndRange)    
			BEGIN   
				SET @BirthYearFilter = @BirthYearFilter + '"' + RIGHT(CAST (@i AS VARCHAR) , 2) +'"'   
				SET @i = @i + 1   
				IF (@i <= YEAR(@BirthDateEndRange)) SET @BirthYearFilter = @BirthYearFilter + ' OR '   
			END   
		END   
		ELSE   
			SET @LongBirthYearFilterRange = 1    
   
	--	select @BirthYearFilter   
	END   
	ELSE   
	BEGIN    
		SET @BirthYearFilter = ''   
		SET @BirthDateStartRange = '01 Jan 1900'   
		SET @BirthDateEndRange   = '31 DEC 9999'   
	END   
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
		,@FirstName = LTRIM(RTRIM(@FirstName))   
		,@Surname = LTRIM(RTRIM(@Surname))   
   
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
	IF CHARINDEX(' ', @FirstName, 0) > 1    
	BEGIN    
		SET @FirstName = @SearchTerm1   
		SET @OtherNames = @SearchTerm2   
	END   
   
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
   
	;WITH Results    
		AS   
		(   
			SELECT   
				A.[ApprenticeId]   
				,[CustomerReferenceNumber]   
--				,[TypeCode]   
				,A.[ProfileTypeCode]   
				,[TitleCode]   
				,[FirstName]   
				,[OtherNames]   
				,[Surname]   
				,[GenderCode]   
				,[BirthDate]   
				,[PreferredName]   
				,QualitySortOrder =    
						CASE    
							WHEN SearchType = 1 THEN 1   
							ELSE 2   
						END   
				,SequenceNo = ROW_NUMBER()    
				OVER   
				(   
					PARTITION BY    
						CASE    
							WHEN SearchType = 1 THEN 1   
							ELSE 2   
						END   
					ORDER BY    
						SearchType ASC,   
						CASE    
							WHEN (Surname		= @Surname)		AND ([FirstName] = @FirstName) THEN 100   
							WHEN (Surname		= @Surname)		AND ([OtherNames] = @FirstName) THEN 95   
							WHEN (Surname		= @Surname)		AND ([FirstName] LIKE @FirstName + '%') THEN 85   
							WHEN (Surname		= @FirstName)	AND ([FirstName] = @Surname) THEN 80   
							WHEN (Surname		= @Surname)		AND ([OtherNames] like @FirstName + '%') THEN 75   
   
							WHEN (Surname Like @Surname + '%' OR @Surname	Like Surname + '%') AND ([FirstName] = @FirstName) THEN 70   
							WHEN (Surname Like @Surname + '%' OR @Surname	Like Surname + '%') AND  ([OtherNames] = @FirstName) THEN 65   
							WHEN (Surname Like @Surname + '%' OR @Surname	Like Surname + '%') AND  ([FirstName] LIKE @FirstName + '%') THEN 60   
							WHEN (Surname Like @FirstName + '%' OR @FirstName LIKE Surname + '%' ) AND ([FirstName] = @Surname) THEN 55   
							WHEN (Surname Like @Surname + '%' OR @Surname	Like Surname + '%') AND  ([OtherNames] like @FirstName + '%') THEN 50   
							WHEN (Surname		= @Surname)		THEN 50   
							WHEN ([FirstName]  = @FirstName)	THEN 40   
							ELSE 20    
							END	* CASE WHEN BirthDate BETWEEN @BirthDateStartRange AND @BirthDateENDRange THEN 1.0 ELSE 0.8 END   
							DESC,   
						 Rank ASC   
				)   
				,SearchType   
				,Iteration   
				,ScoreValue =    
					CAST(   
						CASE    
							WHEN (Surname		= @Surname)		AND ([FirstName] = @FirstName) THEN 100   
							WHEN (Surname		= @Surname)		AND ([OtherNames] = @FirstName) THEN 95   
							WHEN (Surname		= @Surname)		AND ([FirstName] LIKE @FirstName + '%') THEN 85   
							WHEN (Surname		= @FirstName)	AND ([FirstName] = @Surname) THEN 80   
							WHEN (Surname		= @Surname)		AND ([OtherNames] like @FirstName + '%') THEN 75   
   
							WHEN (Surname Like @Surname + '%' OR @Surname	Like Surname + '%') AND ([FirstName] = @FirstName) THEN 70   
							WHEN (Surname Like @Surname + '%' OR @Surname	Like Surname + '%') AND  ([OtherNames] = @FirstName) THEN 65   
							WHEN (Surname Like @Surname + '%' OR @Surname	Like Surname + '%') AND  ([FirstName] LIKE @FirstName + '%') THEN 60   
							WHEN (Surname Like @FirstName + '%' OR @FirstName LIKE Surname + '%' ) AND ([FirstName] = @Surname) THEN 55   
							WHEN (Surname Like @Surname + '%' OR @Surname	Like Surname + '%') AND  ([OtherNames] like @FirstName + '%') THEN 50   
							WHEN (Surname		= @Surname)		THEN 50   
							WHEN ([FirstName]  = @FirstName)	THEN 40   
							ELSE 20    
							END * CASE WHEN BirthDate BETWEEN @BirthDateStartRange AND @BirthDateENDRange THEN 1.0 ELSE 0.8 END   
							AS INT)   
			FROM    
				@FTSearchResults FT   
				JOIN [FT].[Apprentice] FTA ON FT.[Key] = FTA.[Key]   
				JOIN [dbo].[Apprentice] A  WITH ( NOLOCK) ON FTA.ApprenticeId = A.ApprenticeId   
			WHERE    
				--j.TypeCode IN ('c','s')    
				--AND    
				(   
					YEAR(A.BirthDate) BETWEEN YEAR(@BirthDateStartRange) AND YEAR(@BirthDateEndRange)   
					OR @LongBirthYearFilterRange = 0   
				)   
   
		)   
		SELECT TOP 100   
			Results.[ApprenticeId]   
			,Results.[CustomerReferenceNumber] --= CASE WHEN ISNULL([SensitiveClientFlag],0) = 1 THEN '**********' ELSE [CustomerReferenceNumber] END  
			--,[TypeCode]   
			,Results.[ProfileTypeCode]   
			,Results.[TitleCode]   
			,Results.[FirstName]   
			,Results.[OtherNames]   
			,Results.[Surname]   
			,Results.[GenderCode]   
			,Results.[BirthDate]   
	--		,[PreferredName]   
	--		,Quality = CASE WHEN QualitySortOrder = 1 THEN 'H' WHEN (QualitySortOrder = 4 AND SearchType = 1) OR QualitySortOrder = 3 THEN 'M' ELSE 'L' END   
--			,SequenceNo   
--			,SearchType   
			,ScoreValue    
			,PreviousApprenticeId = dup.ApprenticeId   
		FROM    
			Results	   
			LEFT JOIN [dbo].Apprentice dup on Results.ApprenticeId = dup.NewApprenticeId   
			--LEFT JOIN [JobSeeker].JskExtCharacteristic ec ON Results.JobseekerId = ec.JobSeekerId
		WHERE   
			(@QualityCode = 'H' AND QualitySortOrder = 1 AND SequenceNo <= 20)   
			OR ( @QualityCode = 'M' AND ((QualitySortOrder = 1 AND  SequenceNo <= 100) OR (QualitySortOrder > 1 AND  SequenceNo <= 20)))   
			OR ( @QualityCode = 'L')    
		ORDER BY    
			QualitySortOrder   
			,SequenceNo   
		OPTION(FORCE ORDER)   
		   
		SET @RowCount = @@RowCount   
	   
	-- Log the search for diagnostic purposes   
	INSERT INTO [FT].[SearchLog] ([RecordSource],[ExecutionDate],[ExecutionUserId], [Duration],[MaxRows],[RowCount],[Filter],[SearchString],[FTCriteria])   
    VALUES ( 'APP', GETDATE(), ISNULL(@UserId,SYSTEM_USER), DATEDIFF(MILLISECOND,@StartTime,GETDATE()), @MaxRows, @RowCount, NULL, @SearchString, @FTSearchStr)   
   
RETURN 0
