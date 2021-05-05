
  
CREATE PROC [FT].[FTApprenticeProcessFuzzyLogic]  
	 @BatchSize int = 1000  
	,@ProcessErrorFlag bit = 0  -- not implemented need to implement error processing for robustness  
	,@ApprenticeId int = null  
	,@ReprocessFlag bit = 0  
AS  
  
/*--------------------------------------------------------------------------------  
Based on EMP.[Batch].[FTJobSeekerProcessFuzzyLogic]  
----------------------------------------------------------------------------------*/  
  
  
	CREATE TABLE #ApprenticeTerm  
	(  
		[ApprenticeId] [int] NOT NULL,  
		[Term] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,  
		[TermType] char(1) NOT NULL,  
		[Position] [smallint] NOT NULL,  
	)  
  
	CREATE TABLE #ApprenticeWorkingList   
	(   
		ID int Identity(1,1) primary key,   
		ApprenticeId int   
	)   
	  
	CREATE TABLE #ApprenticeProcessingList   
	(  
		ApprenticeId int,  
		[FirstNameTerm] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,  
		[SurnameTerm] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,  
		[OtherNamesTerm] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL  
	)  
  
	CREATE CLUSTERED INDEX	CIX_ApprenticeProcessingList ON  #ApprenticeProcessingList ( [ApprenticeId] )   
	CREATE NONCLUSTERED INDEX IX_ApprenticeProcessingList_FirstNameTerm ON #ApprenticeProcessingList ( [FirstNameTerm] )  
	CREATE NONCLUSTERED INDEX IX_ApprenticeProcessingList_SurnameTerm ON #ApprenticeProcessingList ( [SurnameTerm] )  
	CREATE NONCLUSTERED INDEX IX_ApprenticeProcessingList_OtherNamesTerm ON #ApprenticeProcessingList ( [OtherNamesTerm] )  
	  
	IF @BatchSize IS NULL SET @BatchSize = 1000  
	  
	DECLARE @CurrentId int = 1  
  
	IF @ApprenticeId IS NOT NULL  
	BEGIN   
		INSERT INTO #ApprenticeWorkingList   
		(  
			ApprenticeId  
		)  
		VALUES  
		(  
			@ApprenticeId   
		)		  
	END  
	ELSE  
	BEGIN   
		INSERT INTO #ApprenticeWorkingList   
		(  
			ApprenticeId  
		)  
		SELECT   
			ApprenticeId   
		FROM   
			FT.Apprentice  
		WHERE   
			ProcessStatus = 0   
		ORDER BY ApprenticeId  
	END  
  
	-- Batch records for processing into batch size  
	INSERT INTO #ApprenticeProcessingList   
	(	ApprenticeId,  
		[FirstNameTerm],  
		[SurnameTerm],  
		[OtherNamesTerm]  
	)  
	SELECT   
		j.ApprenticeId  
		,[FirstNameTerm]	= REPLACE(CASE WHEN CHARINDEX(' ',j.[FirstName], 0 ) = 0 THEN j.[FirstName] ELSE LEFT (j.[FirstName], CHARINDEX(' ',j.[FirstName], 0 ) -1 ) END, '''','')  
		,[SurnameTerm]			= REPLACE(CASE WHEN CHARINDEX(' ',j.[Surname], 0 ) = 0 THEN j.[Surname] ELSE LEFT (j.[Surname], CHARINDEX(' ',j.[Surname], 0 ) -1 ) END, '''','')  
		,[OtherNamesTerm]   = REPLACE(CASE WHEN CHARINDEX(' ',j.[OtherNames], 0 ) = 0 THEN j.[OtherNames] ELSE LEFT (j.[OtherNames], CHARINDEX(' ',j.[OtherNames], 0 ) -1 ) END, '''','')  
	FROM   
		#ApprenticeWorkingList wl  
		JOIN [dbo].Apprentice j WITH (NOLOCK) ON wl.ApprenticeId = j.ApprenticeId  
	WHERE  
		wl.ID BETWEEN @CurrentId AND (@CurrentId + @BatchSize)  
	OPTION (FORCE ORDER)  
  
	WHILE EXISTS ( SELECT * FROM #ApprenticeProcessingList)  
	BEGIN   
		BEGIN TRY  -- just incase we get an error   
			-- Create fuzzy terms for new terms  
			;WITH Terms as  
			(  
				SELECT   
					Term = [FirstNameTerm]  
				FROM   
					#ApprenticeProcessingList  
				UNION   
				SELECT   
					Term = [SurnameTerm]  
				FROM   
					#ApprenticeProcessingList  
				UNION   
				SELECT   
					Term = [OtherNamesTerm]  
				FROM   
					#ApprenticeProcessingList  
				WHERE   
					LEN ([OtherNamesTerm]) > 0  
			)  
			INSERT INTO FT.Term (Term)  
			SELECT Terms.Term   
			FROM   
				Terms   
				LEFT JOIN FT.Term tl ON Terms.term = tl.Term   
			WHERE tl.Term IS NULL  
			OPTION (FORCE ORDER)  
			  
			UPDATE [FT].[Apprentice]  
			SET   
				[FirstNameFuzzySpecificTermList]	= ISNULL(t1.[SpecificTermList],'')  
				,[FirstNameFuzzyBroadTermList]		= ISNULL(t1.[BroadTermList],'')  
				,[SurnameFuzzySpecificTermList]			= ISNULL(t2.[SpecificTermList],'')  
				,[SurnameFuzzyBroadTermList]			= ISNULL(t2.[BroadTermList],'')  
				,[OtherNamesFuzzySpecificTermList]	= ISNULL(t3.[SpecificTermList],'')  
				,[OtherNamesFuzzyBroadTermList]		= ISNULL(t3.[BroadTermList],'')  
				,[FirstNameAliasTermList]			= ISNULL(t1.[AliasTermList],'')  
				,[FirstNameAliasFuzzySpecificTermList]	=  ISNULL(t1.[AliasSpecificTermList],'') --+ ISNULL(' ' + t1.[AliasBroadTermList],'')  
				,[ProcessStatus] = 1  
				,[ProcessDate] = GETDATE()  
			FROM   
				#ApprenticeProcessingList pl  
				JOIN [FT].[Apprentice] ON [FT].[Apprentice].ApprenticeId = pl.ApprenticeId  
				JOIN [FT].[Term] t1 ON pl.[FirstNameTerm] = t1.Term  
				JOIN [FT].[Term] t2 ON pl.[SurnameTerm] = t2.Term  
				LEFT JOIN [FT].[Term] t3 ON pl.[OtherNamesTerm] = t3.Term  
			OPTION ( FORCE ORDER )  
  
			SET @CurrentId = @CurrentId + @BatchSize  
  
		END TRY   
		BEGIN CATCH   
			IF @BatchSize > 1   
				SET @BatchSize = 1  
			ELSE -- means we have found the error   
			BEGIN   
				UPDATE [FT].[Apprentice]  
				SET   
					ProcessStatus = 99  
					,ProcessDate = GETDATE()  
				FROM #ApprenticeProcessingList pl  
				WHERE  
					[FT].[Apprentice].ApprenticeId = pl.ApprenticeId  
			  
				SET @CurrentId = @CurrentId + @BatchSize  
				SET @BatchSize = 1000	-- increase bacth size after reprocessing error  
			END  
			-- next batch  
		END CATCH  
  
		TRUNCATE TABLE #ApprenticeProcessingList  
		INSERT INTO #ApprenticeProcessingList   
		(	ApprenticeId,  
			[FirstNameTerm],  
			[SurnameTerm],  
			[OtherNamesTerm]  
		)  
		SELECT   
			j.ApprenticeId  
			,[FirstNameTerm]	= REPLACE(CASE WHEN CHARINDEX(' ',j.[FirstName], 0 ) = 0 THEN j.[FirstName] ELSE LEFT (j.[FirstName], CHARINDEX(' ',j.[FirstName], 0 ) -1 ) END, '''','')  
			,[SurnameTerm]			= REPLACE(CASE WHEN CHARINDEX(' ',j.[Surname], 0 ) = 0 THEN j.[Surname] ELSE LEFT (j.[Surname], CHARINDEX(' ',j.[Surname], 0 ) -1 ) END, '''','')  
			,[OtherNamesTerm]   = REPLACE(CASE WHEN CHARINDEX(' ',j.[OtherNames], 0 ) = 0 THEN j.[OtherNames] ELSE LEFT (j.[OtherNames], CHARINDEX(' ',j.[OtherNames], 0 ) -1 ) END, '''','')  
		FROM   
			#ApprenticeWorkingList wl  
			JOIN [dbo].Apprentice j WITH (NOLOCK) ON wl.ApprenticeId = j.ApprenticeId  
		WHERE  
			wl.ID BETWEEN @CurrentId AND (@CurrentId + @BatchSize)  
		OPTION (FORCE ORDER)  
	END --WHILE  
