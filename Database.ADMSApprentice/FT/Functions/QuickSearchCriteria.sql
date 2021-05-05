

CREATE  FUNCTION [FT].[QuickSearchCriteria] (@SearchTerms varchar(200), @FilterTerms varchar(200), @FuzzyFactor smallint, @Mode tinyint) 
RETURNS XML  
AS  		 
BEGIN 
		DECLARE @SearchTerm TABLE (RowNumber int identity(1,1), [Term] varchar(50), SoundexTerm char(4),SoundexBroadTerm char(4),SoundexBroadTerm2 char(4), SoundexFuzzyLevel tinyint, MetaphoneTerm varchar(5), MetaphoneTermALt varchar(5), MetaphoneBroadTerm varchar(4), MetaphoneFuzzyLevel tinyint, Frequency int,ConsonantTerm varchar(5), ConsonantBroadTerm varchar(4),ConsonantFuzzyLevel tinyint) 
 
		DECLARE  
			@SearchTermsXML XML 
			,@FilterTermsXML XML 
			,@CriteriaXML XML 
			,@Delimiter char(1) = ' ' 
			,@RowCount tinyint 
			,@Row int = 1 
			,@Operator varchar(10) 
			,@MetaphoneSuffix char(2) = 'mZ' -- used in the catalog so as not to confuse terms with meatphone terms 
 
			-- Max 6 Terms 
			,@Term1 varchar(100) 
			,@Term2 varchar(100) 
			,@Term3 varchar(100) 
			,@Term4 varchar(100) 
			,@Term5 varchar(100) 
			,@Term6 varchar(100) 
 
			,@Term1Frequency int 
			,@Term2Frequency int 
			,@Term3Frequency int 
			,@Term4Frequency int 
			,@Term5Frequency int 
			,@Term6Frequency int 
 
			,@SoundexTerm1 char(4) 
			,@SoundexTerm2 char(4) 
			,@SoundexTerm3 char(4) 
			,@SoundexTerm4 char(4) 
			,@SoundexTerm5 char(4) 
			,@SoundexTerm6 char(4) 
 
			,@IgnoreConstonant bit = 1 
			,@IgnoreFuzzy bit = 0 
			,@FilterCriteria varchar(8000) = '' 
 
		SET @SearchTerms = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(@SearchTerms, '&',  ' '), '"', ' '),'''',''),'<',''),'>',''),'"',' '), ')',''),'(',''), '-',' '), '.',' '), ',',' '),CHAR(8),'') 
		SET @SearchTermsXML = CONVERT(xml,' <root> <s>' + REPLACE(@SearchTerms, @Delimiter,'</s> <s>') + '</s>   </root> ') 
		SET @OPerator  
			= CASE 
				@Mode  
					WHEN 1 THEN 'NEAR' 
					WHEN 2 THEN 'AND'  
					WHEN 3 THEN 'OR' 
					ELSE 'NEAR' 
				END 
		SET @FilterCriteria = @FilterTerms 
		IF LEN(@FilterTerms) > 0  
		BEGIN 
			SET @FilterCriteria = @FilterTerms 
 
			--SET @FilterTermsXML = [FT].[QuickSearchCriteria] ( @FilterTerms, '', 1, 2 )  -- 2 = operator AND  
			--SET @FilterCriteria = @FilterTermsXML.value('(/FuzzyLevel1Criteria)[1]','varchar(8000)') 
			--IF  LEN(@FilterCriteria) = 0 SET @FilterCriteria = @FilterTermsXML.value('(/FuzzyLevel0Criteria)[1]','varchar(8000)') 
		END 
		ELSE 
			SET @FilterCriteria = '' 
 
		;WITH SearchTerms 
		AS 
		( 
			SELECT  
				[Term] = LTRIM(T.c.value('.','varchar(100)')) 
				,[SoundexTerm]   = CASE WHEN @FuzzyFactor > 1 AND ISNUMERIC ( T.c.value('.','varchar(100)')) = 0 THEN SOUNDEX(LTRIM(T.c.value('.','varchar(100)'))) ELSE '' END 
				,[MetaphoneTerm] = CASE WHEN @FuzzyFactor > 1 AND LEN(LTRIM(T.c.value('.','varchar(100)'))) > 1 THEN FT.DoubleMetaPhone(LTRIM(T.c.value('.','varchar(100)'))) ELSE '' END 
				,[ConsonantTerm] = CASE WHEN @FuzzyFactor > 1 AND LEN(LTRIM(T.c.value('.','varchar(100)'))) > 1 THEN FT.ConsonantTerm(LTRIM(T.c.value('.','varchar(100)'))) ELSE '' END 
			FROM  
				@SearchTermsXML.nodes('/root/s') T(c) 
		), Term AS 
		( 
			SELECT  
				st.[Term] 
				,SoundexTerm =  
					CASE  
						WHEN ISNUMERIC(st.[Term]) = 1 THEN ''  
						WHEN RIGHT([SoundexTerm], 3) = '000' THEN ''  
						ELSE [SoundexTerm] 
					END 
				,SoundexBroadTerm =   
					CASE  
						WHEN ISNUMERIC(st.[Term]) = 1 THEN ''  
						WHEN RIGHT(SoundexTerm,1) <> '0'   THEN LEFT(SoundexTerm,3) + '0'  
						WHEN RIGHT(SoundexTerm,2) <> '00'  THEN LEFT(SoundexTerm,2) + '00'  
						ELSE ''  
					END 
				,SoundexBroadTerm2 =  
					CASE  
						WHEN ISNUMERIC(st.[Term]) = 1 THEN ''  
						WHEN RIGHT(SoundexTerm,1) <> '0'   THEN LEFT(SoundexTerm,1) + RIGHT(SoundexTerm,2) + '0'  
						ELSE ''  
					END 
				,SoundexFuzzyLevel =  
					CASE  
						WHEN ISNUMERIC(st.[Term]) = 1 THEN 0 
						WHEN RIGHT(SoundexTerm,1) <> '0'   THEN 4  
						WHEN RIGHT(SoundexTerm,2) <> '00'  THEN 3 
						ELSE 1 
					END 
				,MetaphoneTerm    = CASE WHEN LEN(st.MetaphoneTerm) >5 AND LEFT(st.MetaphoneTerm,1) = LEFT(st.Term,1) THEN  RTRIM(LEFT(st.MetaphoneTerm,5))  ELSE '' END 
				,MetaphoneTermALT = CASE WHEN LEN(st.MetaphoneTerm) >5 AND LEFT(RTRIM(RIGHT(st.MetaphoneTerm,5)),1) = LEFT(st.Term,1) THEN  RTRIM(RIGHT(st.MetaphoneTerm,5)) ELSE '' END 
				,MetaphoneTermBroad = CASE WHEN LEN(st.MetaphoneTerm) >5 AND LEFT(st.MetaphoneTerm,1) = LEFT(st.Term,1)  THEN LEFT(st.MetaphoneTerm, LEN(RTRIM(LEFT(st.MetaphoneTerm,5))) -1) ELSE '' END 
				,[ConsonantTerm]  = CASE WHEN st.[ConsonantTerm] = st.Term THEN '' ELSE st.[ConsonantTerm] END 
				,ConsonantBroadTerm= CASE WHEN LEN(st.[ConsonantTerm]) > 1 THEN LEFT (st.[ConsonantTerm], LEN(st.[ConsonantTerm])-1) ELSE '' END 
				,Frequency = ISNULL(t.Frequency,1) 
			FROM  
				SearchTerms st 
				LEFT JOIN FT.Term t ON st.Term = t.Term  
			WHERE 
				st.Term <> '' 
		)  
		INSERT INTO @SearchTerm 
		( 
			Term 
			,SoundexTerm 
			,SoundexBroadTerm 
			,SoundexBroadTerm2  
			,SoundexFuzzyLevel 
			,MetaphoneTerm 
			,MetaphoneTermALt 
			,MetaphoneBroadTerm  
			,MetaphoneFuzzyLevel 
			,Frequency 
			,ConsonantTerm 
			,ConsonantBroadTerm 
			,ConsonantFuzzyLevel 
		) 
		SELECT  
			[Term] = RTRIM(Term) 
			,SoundexTerm 
			,SoundexBroadTerm  
			,SoundexBroadTerm2  
			,SoundexFuzzyLevel 
			,MetaphoneTerm 
			,MetaphoneTermALT 
			,MetaphoneTermBroad 
			,MetaphoneFuzzyLevel = CASE WHEN LEN(MetaphoneTerm) = 0 OR  LEFT(MetaphoneTerm,1) <> LEFT(Term,1)  THEN 0 ELSE LEN(MetaphoneTerm) END  
			,Frequency 
			,ConsonantTerm 
			,ConsonantBroadTerm 
			,ConsonantFuzzyLevel = CASE WHEN LEN(ConsonantTerm) = 0 THEN 0 ELSE LEN(ConsonantTerm) END  
		FROM  
			Term 
		SET @RowCount = @@RowCount 
 
		DECLARE  
			@Term varchar(50) 
			,@SoundexTerm char(4) 
			,@SoundexBroadTerm char(4) 
			,@SoundexBroadTerm2 char(4) 
			,@SoundexFuzzyLevel tinyint 
			,@MetaphoneTerm varchar(5) 
			,@MetaphoneTermALt varchar(4) 
			,@MetaphoneBroadTerm varchar(4) 
			,@MetaphoneFuzzyLevel tinyint 
			,@Frequency int 
 
			,@DeltaCount int = 0 
 
		DECLARE  
			 @FuzzyLevel0Criteria varchar(200) = '' 
			,@FuzzyLevel1Criteria varchar(200) = '' 
			,@FuzzyLevel2Criteria varchar(200) = '' 
			,@FuzzyLevel2CriteriaOdd varchar(200) = '' 
			,@FuzzyLevel2CriteriaEven varchar(200) = '' 
			,@FuzzyLevel3CriteriaOdd varchar(200) = '' 
			,@FuzzyLevel3CriteriaEven varchar(200) = '' 
			,@FuzzyLevel4Criteria varchar(200) = '' 
			,@FuzzyLevel5Criteria varchar(200) = '' 
 
			,@SpecificFuzzyTerm varchar(50) = '' 
			,@BroadFuzzyTerm varchar(50) = '' 
 
			,@DuplicateTerm varchar(50) = '' 
			,@ConsonantTerm varchar(5) ='' 
			,@ConsonantBroadTerm varchar(4) = '' 
			,@ConsonantFuzzyLevel tinyint 
 
 
		WHILE @Row <= @Rowcount 
		BEGIN 
			IF @Term <> @ConsonantTerm AND @ConsonantTerm <> '' AND @IgnoreConstonant = 1 SET @IgnoreConstonant = 0 
			IF @Row = 1 
				SELECT  
					@Term1 = Term, @Term = Term, @SoundexTerm1 = SoundexTerm, @SoundexTerm = SoundexTerm, @SoundexBroadTerm = SoundexBroadTerm, @SoundexBroadTerm2 = SoundexBroadTerm2, @SoundexFuzzyLevel = SoundexFuzzyLevel, @MetaphoneTerm = MetaphoneTerm, @MetaphoneTermALt = MetaphoneTermALt, @MetaphoneBroadTerm = MetaphoneBroadTerm, @MetaphoneFuzzyLevel = MetaphoneFuzzyLevel, @Term1Frequency = Frequency 
					,@ConsonantTerm = ConsonantTerm, @ConsonantFuzzyLevel = ConsonantFuzzyLevel, @ConsonantBroadTerm = ConsonantBroadTerm 
				 FROM @SearchTerm WHERE RowNumber = 1 
			IF @Row = 2  
			BEGIN 
				SELECT  
					@Term2 = Term, @Term = Term, @SoundexTerm2 = SoundexTerm, @SoundexTerm = CASE WHEN SoundexTerm IN (@SoundexTerm1) THEN '' ELSE SoundexTerm END, @SoundexBroadTerm = SoundexBroadTerm, @SoundexBroadTerm2 = SoundexBroadTerm2, @SoundexFuzzyLevel = CASE WHEN SoundexTerm IN (@SoundexTerm1) THEN 0 ELSE SoundexFuzzyLevel END, @MetaphoneTerm = MetaphoneTerm, @MetaphoneTermALt = MetaphoneTermALt, @MetaphoneBroadTerm = MetaphoneBroadTerm, @MetaphoneFuzzyLevel = MetaphoneFuzzyLevel, @Term2Frequency = Frequency 
					,@ConsonantTerm = ConsonantTerm, @ConsonantFuzzyLevel = ConsonantFuzzyLevel, @ConsonantBroadTerm = ConsonantBroadTerm 
				FROM @SearchTerm WHERE RowNumber = 2		 
 
				IF @Rowcount < 4 
				BEGIN 
					IF (@Term1 <> @Term2 AND @Term2 LIKE  @Term1 + '%')  
						SET @DuplicateTerm  = @Term1  
					ELSE IF (@Term1 <> @Term2 AND @Term1 LIKE  @Term2 + '%')  
						SET @DuplicateTerm  = @Term2 
					ELSE  
						SET @DuplicateTerm  = '' 
				END 
			END 
			IF @Row = 3  
			BEGIN 
				SELECT  
					@Term3 = Term, @Term = Term, @SoundexTerm3 = SoundexTerm, @SoundexTerm = CASE WHEN SoundexTerm IN (@SoundexTerm1,@SoundexTerm2 ) THEN '' ELSE SoundexTerm END, @SoundexBroadTerm = SoundexBroadTerm, @SoundexBroadTerm2 = SoundexBroadTerm2, @SoundexFuzzyLevel = CASE WHEN SoundexTerm IN (@SoundexTerm1,@SoundexTerm2 ) THEN 0 ELSE SoundexFuzzyLevel END, @MetaphoneTerm = MetaphoneTerm, @MetaphoneTermALt = MetaphoneTermALt, @MetaphoneBroadTerm = MetaphoneBroadTerm, @MetaphoneFuzzyLevel = MetaphoneFuzzyLevel, @Term3Frequency = Frequency 
					,@ConsonantTerm = ConsonantTerm, @ConsonantFuzzyLevel = ConsonantFuzzyLevel, @ConsonantBroadTerm = ConsonantBroadTerm 
				FROM @SearchTerm WHERE RowNumber = 3	 
 
 
				IF @Rowcount < 4 
				BEGIN 
					IF (@Term1 <> @Term3 AND @Term3 LIKE  @Term1 + '%')  
						SET @DuplicateTerm  = @Term1  
					ELSE IF (@Term3 <> @Term2 AND  @Term3 LIKE  @Term2 + '%')  
						SET @DuplicateTerm  = @Term2 
					ELSE IF ((@Term1 <> @Term3 AND @Term1 LIKE  @Term3 + '%') OR ( @Term3 <> @Term2 AND @Term2 LIKE  @Term3 + '%' )) 
						SET @DuplicateTerm  = @Term3 
					ELSE  
						SET @DuplicateTerm  = '' 
				END 
			END 
 
			IF @Row = 4  
			BEGIN 
				SELECT  
					@Term4 = Term, @Term = Term, @SoundexTerm4 = SoundexTerm, @SoundexTerm = CASE WHEN SoundexTerm IN (@SoundexTerm1,@SoundexTerm2,@SoundexTerm3 ) THEN '' ELSE SoundexTerm END, @SoundexBroadTerm = SoundexBroadTerm, @SoundexBroadTerm2 = SoundexBroadTerm2, @SoundexFuzzyLevel = CASE WHEN SoundexTerm IN (@SoundexTerm1,@SoundexTerm2,@SoundexTerm3 ) THEN 0 ELSE SoundexFuzzyLevel END, @MetaphoneTerm = MetaphoneTerm, @MetaphoneTermALt = MetaphoneTermALt, @MetaphoneBroadTerm = MetaphoneBroadTerm, @MetaphoneFuzzyLevel = MetaphoneFuzzyLevel, @Term4Frequency = Frequency 
					,@ConsonantTerm = ConsonantTerm, @ConsonantFuzzyLevel = ConsonantFuzzyLevel, @ConsonantBroadTerm = ConsonantBroadTerm 
				FROM @SearchTerm WHERE RowNumber = 4 
			END 
 
			IF @Row = 5 
			BEGIN 
				SELECT  
					@Term5 = Term, @Term = Term, @SoundexTerm5 = SoundexTerm, @SoundexTerm = CASE WHEN SoundexTerm IN (@SoundexTerm1,@SoundexTerm2,@SoundexTerm3,@SOundexTerm4 ) THEN '' ELSE SoundexTerm END, @SoundexBroadTerm = SoundexBroadTerm, @SoundexBroadTerm2 = SoundexBroadTerm2, @SoundexFuzzyLevel =  CASE WHEN SoundexTerm IN (@SoundexTerm1,@SoundexTerm2,@SoundexTerm3,@SOundexTerm4 ) THEN 0 ELSE SoundexFuzzyLevel END, @MetaphoneTerm = MetaphoneTerm, @MetaphoneTermALt = MetaphoneTermALt, @MetaphoneBroadTerm = MetaphoneBroadTerm, @MetaphoneFuzzyLevel = MetaphoneFuzzyLevel, @Term5Frequency = Frequency 
					,@ConsonantTerm = ConsonantTerm, @ConsonantFuzzyLevel = ConsonantFuzzyLevel, @ConsonantBroadTerm = ConsonantBroadTerm 
				FROM @SearchTerm WHERE RowNumber = 5 
 
			END 
 
			IF @Row = 6 
			BEGIN 
				SELECT  
					@Term6 = Term, @Term = Term, @SoundexTerm6 = SoundexTerm, @SoundexTerm = CASE WHEN SoundexTerm IN (@SoundexTerm1,@SoundexTerm2,@SoundexTerm3,@SOundexTerm4,@SoundexTerm5) THEN '' ELSE SoundexTerm END, @SoundexBroadTerm = SoundexBroadTerm, @SoundexBroadTerm2 = SoundexBroadTerm2, @SoundexFuzzyLevel = SoundexFuzzyLevel, @MetaphoneTerm = MetaphoneTerm, @MetaphoneTermALt = MetaphoneTermALt, @MetaphoneBroadTerm = MetaphoneBroadTerm, @MetaphoneFuzzyLevel = MetaphoneFuzzyLevel, @Term6Frequency = Frequency 
					,@ConsonantTerm = ConsonantTerm, @ConsonantFuzzyLevel = ConsonantFuzzyLevel, @ConsonantBroadTerm = ConsonantBroadTerm 
				FROM @SearchTerm WHERE RowNumber = 6 
			END 
 
			SELECT  
				 @SpecificFuzzyTerm =  
					CASE  
						WHEN @SoundexFuzzyLevel > @MetaphoneFuzzyLevel OR @MetaphoneTerm = @ConsonantTerm THEN @SoundexTerm 
						ELSE  
							CASE WHEN LEN(@MetaphoneTerm) <= 1  
								THEN  
									CASE WHEN LEN(@ConsonantBroadTerm) > 1 THEN @ConsonantBroadTerm ELSE @Term END  
								ELSE  
									@MetaphoneTerm + 'mZ'  
							END 
 
					END 
				,@BroadFuzzyTerm =  
					CASE  
						WHEN @SoundexFuzzyLevel > @MetaphoneFuzzyLevel THEN @SoundexBroadTerm 
							ELSE  
 
							CASE WHEN LEN(@MetaphoneBroadTerm) <= 1 THEN  
								CASE  
									WHEN LEN(@ConsonantBroadTerm) > 1 THEN @ConsonantBroadTerm  
									ELSE @Term  
								END  
								ELSE @MetaphoneBroadTerm + 'mZ'   
							END  
					END 
 
 
			IF @Term <> @ConsonantTerm AND LEN(@ConsonantTerm) > 0 SET @DeltaCount = @DeltaCount + 1 
 
			SELECT  
				 @FuzzyLevel1Criteria = @FuzzyLevel1Criteria + ' ' + @Operator + ' "' + @Term + '"' + CASE WHEN LEN(@DuplicateTerm) > 0 THEN ' ' + @Operator + ' "' + @DuplicateTerm + '"'  ELSE '' END 
 
				,@FuzzyLevel2Criteria     = @FuzzyLevel2Criteria     + ' ' + @Operator + ' "' + CASE WHEN LEN (@ConsonantTerm) = 0 THEN @Term ELSE @ConsonantTerm  END + '"' + CASE WHEN LEN(@DuplicateTerm) > 0 THEN ' ' + @Operator + ' "' + @DuplicateTerm + '"'  ELSE '' END 
				,@FuzzyLevel2CriteriaOdd  = @FuzzyLevel2CriteriaOdd  + ' ' + @Operator + ' "' + CASE WHEN @DeltaCount % 2 = 0 AND LEN(@ConsonantTerm) > 0 THEN @ConsonantTerm ELSE @Term END + '"' + CASE WHEN LEN(@DuplicateTerm) > 0 THEN ' ' + @Operator + ' "' + @DuplicateTerm + '"'  ELSE '' END 
				,@FuzzyLevel2CriteriaEven = @FuzzyLevel2CriteriaEven + ' ' + @Operator + ' "' + CASE WHEN @DeltaCount % 2 = 1 AND LEN(@ConsonantTerm) > 0 THEN @ConsonantTerm ELSE @Term END + '"' + CASE WHEN LEN(@DuplicateTerm) > 0 THEN ' ' + @Operator + ' "' + @DuplicateTerm + '"'  ELSE '' END 
 
 
				,@FuzzyLevel3CriteriaOdd  = @FuzzyLevel3CriteriaOdd  + ' ' + @Operator + ' "' + CASE WHEN @DeltaCount % 2 = 0 AND LEN(@SpecificFuzzyTerm) > 0 THEN @SpecificFuzzyTerm ELSE @Term END + '"' + CASE WHEN LEN(@DuplicateTerm) > 0 THEN ' ' + @Operator + ' "' + @DuplicateTerm + '"'  ELSE '' END 
				,@FuzzyLevel3CriteriaEven = @FuzzyLevel3CriteriaEven + ' ' + @Operator + ' "' + CASE WHEN @DeltaCount % 2 = 1 AND LEN(@SpecificFuzzyTerm) > 0 THEN @SpecificFuzzyTerm ELSE @Term END + '"' + CASE WHEN LEN(@DuplicateTerm) > 0 THEN ' ' + @Operator + ' "' + @DuplicateTerm + '"'  ELSE '' END 
 
 
				,@FuzzyLevel4Criteria = @FuzzyLevel4Criteria + ' ' + @Operator + ' "' + CASE WHEN LEN(@SpecificFuzzyTerm) > 0 THEN @SpecificFuzzyTerm ELSE CASE WHEN LEN(@ConsonantTerm) > 0 THEN @ConsonantTerm ELSE @Term END END+ '"' 
				,@FuzzyLevel5Criteria = @FuzzyLevel5Criteria + ' ' + @Operator + ' "' + CASE WHEN LEN(@BroadFuzzyTerm) > 0 THEN @BroadFuzzyTerm ELSE CASE WHEN LEN(@SpecificFuzzyTerm) > 0 THEN @SpecificFuzzyTerm ELSE  CASE WHEN LEN(@ConsonantTerm) > 0 THEN @ConsonantTerm ELSE @Term END END END + '"' 
 
 
				SET @Row= @Row + 1 
		END 
 
	IF @Rowcount = 1 and @Term1Frequency > 1000 SET @IgnoreFuzzy = 1 
 
	SELECT  
		 @FuzzyLevel0Criteria = '"' + @SearchTerms + '"'  
		,@FuzzyLevel1Criteria = RTRIM(CASE WHEN LEN(@FuzzyLevel1Criteria) > 0 THEN RIGHT(@FuzzyLevel1Criteria, LEN(@FuzzyLevel1Criteria) - (LEN(@Operator) + 2)) ELSE NULL END) 
		,@FuzzyLevel2Criteria = RTRIM(CASE WHEN LEN(@FuzzyLevel2Criteria) > 0 THEN RIGHT(@FuzzyLevel2Criteria, LEN(@FuzzyLevel2Criteria) - (LEN(@Operator) + 2)) ELSE NULL END) 
		,@FuzzyLevel2CriteriaOdd = RTRIM(CASE WHEN LEN(@FuzzyLevel2CriteriaOdd) > 0 THEN RIGHT(@FuzzyLevel2CriteriaOdd, LEN(@FuzzyLevel2CriteriaOdd) - (LEN(@Operator) + 2)) ELSE NULL END) 
		,@FuzzyLevel2CriteriaEven = RTRIM(CASE WHEN LEN(@FuzzyLevel2CriteriaEven) > 0 THEN RIGHT(@FuzzyLevel2CriteriaEven, LEN(@FuzzyLevel2CriteriaEven) - (LEN(@Operator) + 2)) ELSE NULL END) 
		,@FuzzyLevel3CriteriaOdd = RTRIM(CASE WHEN @IgnoreFuzzy = 0 AND LEN(@FuzzyLevel3CriteriaOdd) > 0 THEN RIGHT(@FuzzyLevel3CriteriaOdd, LEN(@FuzzyLevel3CriteriaOdd) - (LEN(@Operator) + 2)) ELSE NULL END) 
		,@FuzzyLevel3CriteriaEven = RTRIM(CASE WHEN @IgnoreFuzzy = 0 AND LEN(@FuzzyLevel3CriteriaEven) > 0 THEN RIGHT(@FuzzyLevel3CriteriaEven, LEN(@FuzzyLevel3CriteriaEven) - (LEN(@Operator) + 2)) ELSE NULL END) 
		,@FuzzyLevel4Criteria = RTRIM(CASE WHEN @IgnoreFuzzy = 0 AND LEN(@FuzzyLevel4Criteria) > 0 THEN RIGHT(@FuzzyLevel4Criteria, LEN(@FuzzyLevel4Criteria) - (LEN(@Operator) + 2)) ELSE NULL END) 
		,@FuzzyLevel5Criteria = RTRIM(CASE WHEN @IgnoreFuzzy = 0 AND LEN(@FuzzyLevel5Criteria) > 0 THEN RIGHT(@FuzzyLevel5Criteria, LEN(@FuzzyLevel5Criteria) - (LEN(@Operator) + 2)) ELSE NULL END) 
 
	SELECT   
		-- Search Term List 
		@FuzzyLevel1Criteria = CASE WHEN @FuzzyLevel1Criteria = @FuzzyLevel0Criteria THEN  NULL ELSE @FuzzyLevel1Criteria END 
 
		,@FuzzyLevel2Criteria     = CASE WHEN @FuzzyLevel2Criteria IN(@FuzzyLevel0Criteria, @FuzzyLevel1Criteria) THEN  NULL ELSE @FuzzyLevel2Criteria END 
		,@FuzzyLevel2CriteriaOdd  = CASE WHEN @FuzzyLevel2CriteriaOdd  IN(@FuzzyLevel0Criteria, @FuzzyLevel1Criteria, @FuzzyLevel2Criteria) THEN  NULL ELSE @FuzzyLevel2CriteriaOdd END 
		,@FuzzyLevel2CriteriaEven = CASE WHEN @FuzzyLevel2CriteriaEven IN(@FuzzyLevel0Criteria, @FuzzyLevel1Criteria, @FuzzyLevel2Criteria,@FuzzyLevel2CriteriaOdd) THEN  NULL ELSE @FuzzyLevel2CriteriaEven END 
		,@FuzzyLevel3CriteriaOdd  = CASE WHEN @FuzzyLevel3CriteriaOdd  IN(@FuzzyLevel0Criteria, @FuzzyLevel1Criteria, @FuzzyLevel2Criteria,@FuzzyLevel2CriteriaOdd, @FuzzyLevel2CriteriaEven) THEN  NULL ELSE @FuzzyLevel3CriteriaOdd END 
		,@FuzzyLevel3CriteriaEven = CASE WHEN @FuzzyLevel3CriteriaEven IN(@FuzzyLevel0Criteria, @FuzzyLevel1Criteria, @FuzzyLevel2Criteria,@FuzzyLevel2CriteriaOdd, @FuzzyLevel2CriteriaEven,@FuzzyLevel3CriteriaOdd) THEN  NULL ELSE @FuzzyLevel3CriteriaEven END 
 
		,@FuzzyLevel4Criteria = CASE WHEN @FuzzyLevel4Criteria IN(@FuzzyLevel0Criteria, @FuzzyLevel1Criteria, @FuzzyLevel2Criteria,@FuzzyLevel2CriteriaOdd, @FuzzyLevel2CriteriaEven,@FuzzyLevel3CriteriaOdd,@FuzzyLevel3CriteriaEven) THEN  NULL ELSE @FuzzyLevel4Criteria END 
		,@FuzzyLevel5Criteria = CASE WHEN @FuzzyLevel5Criteria = @FuzzyLevel4Criteria THEN  NULL ELSE @FuzzyLevel5Criteria END 
 
 
	SELECT @CriteriaXML =  
	( 
		SELECT  
			Criteria =   
 
				CASE WHEN LEN(@FilterCRiteria) > 0 THEN @FilterCriteria + ' AND ISABOUT( ' ELSE 'ISABOUT( ' END 
				+ ISNULL( @FuzzyLevel0Criteria                      + ' WEIGHT(1.00)','')  
				+ CASE WHEN @FuzzyFactor >= 1 THEN  ISNULL( ', ' + @FuzzyLevel1Criteria               + ' WEIGHT(0.90)','') ELSE '' END 
				+ CASE WHEN @FuzzyFactor >= 2 THEN  ISNULL( ', ' + @FuzzyLevel2CriteriaOdd    		+ ' WEIGHT(0.6)','')  
					+ ISNULL( ', ' + @FuzzyLevel2CriteriaEven    		+ ' WEIGHT(0.6)','') ELSE '' END 
 
 
				+ CASE WHEN @FuzzyFactor >= 3 THEN  ISNULL( ', ' + @FuzzyLevel3CriteriaOdd    		+ ' WEIGHT(0.25)','')  
					+ ISNULL( ', ' + @FuzzyLevel3CriteriaEven    		+ ' WEIGHT(0.25)','') ELSE '' END 
				+ CASE WHEN @FuzzyFactor >= 4 THEN  ISNULL( ', ' + @FuzzyLevel4Criteria	    		+ ' WEIGHT(0.08)','') ELSE '' END 
				+ CASE WHEN @FuzzyFactor >= 5 THEN  ISNULL( ', ' + @FuzzyLevel5Criteria	    		+ ' WEIGHT(0.01)','') ELSE '' END 
			+')' 
 
			,TermCriteria =  
				CASE WHEN LEN(@FilterCRiteria) > 0 THEN @FilterCriteria + ' AND ISABOUT( ' ELSE 'ISABOUT( ' END 
					+ ISNULL( @FuzzyLevel0Criteria                      + ' WEIGHT(1.00)','')  
					+ CASE WHEN @FuzzyFactor >= 1 THEN  ISNULL( ', ' + @FuzzyLevel1Criteria               + ' WEIGHT(0.10)','') ELSE '' END 
					+')' 
 
			,FuzzyConsonantCriteria =  
				CASE WHEN  @FuzzyFactor >= 2  THEN 
					CASE WHEN LEN(@FuzzyLevel2Criteria) > 0 THEN 
						CASE WHEN LEN(@FilterCriteria) > 0  THEN @FilterCriteria + ' AND ISABOUT( ' ELSE 'ISABOUT( ' END 
						+ @FuzzyLevel2Criteria    		+ ' WEIGHT(0.1)' 
						+')' 
						ELSE '' END 
					ELSE '' END 
 
 
			,FuzzySpecificCriteria =  
				CASE  
					WHEN LEN(@FuzzyLevel3CriteriaOdd) > 0 THEN 
						CASE WHEN LEN(@FilterCriteria) > 0  THEN @FilterCriteria + ' AND ISABOUT( ' ELSE 'ISABOUT( ' END 
						+ ISNULL( + @FuzzyLevel3CriteriaOdd    		+ ' WEIGHT(0.5)','')  
						+ ISNULL( ', ' + @FuzzyLevel3CriteriaEven    		+ ' WEIGHT(0.5)','')  
						+')' 
					ELSE  
						CASE WHEN LEN(@FuzzyLevel3CriteriaEven) > 0 THEN 
							CASE WHEN LEN(@FilterCriteria) > 0  THEN @FilterCriteria + ' AND ISABOUT( ' ELSE 'ISABOUT( ' END 
							+ ISNULL( @FuzzyLevel3CriteriaEven    		+ ' WEIGHT(0.5)','')  
							+')' 
						ELSE '' END 
					END 
			,FuzzyBroadCriteria =  
				CASE  
					WHEN LEN(@FuzzyLevel4Criteria) > 0 THEN  
						CASE WHEN LEN(@FilterCRiteria) > 0 THEN @FilterCriteria + ' AND ISABOUT( ' ELSE 'ISABOUT( ' END 
						+ ISNULL( @FuzzyLevel4Criteria + ' WEIGHT(1.0)','')  
						+ CASE WHEN @FuzzyFactor >= 5 THEN  ISNULL( ', ' + @FuzzyLevel5Criteria	    		+ ' WEIGHT(0.01)','')  
						+')' 
						ELSE ')' END 
					ELSE  
						CASE WHEN LEN(@FuzzyLevel5Criteria) > 0 THEN 
							CASE WHEN LEN(@FilterCRiteria) > 0  THEN @FilterCriteria + ' AND ISABOUT( ' ELSE 'ISABOUT( ' END 
							+ ISNULL( @FuzzyLevel5Criteria	    		+ ' WEIGHT(0.01)','')  
							+')' 
						ELSE '' END 
					END 
			,FuzzyLevel0Criteria        = ISNULL( @FuzzyLevel0Criteria,'')  
			,FuzzyLevel1Criteria        = ISNULL( @FuzzyLevel1Criteria,'')  
			,FuzzyLevel2Criteria        = ISNULL( @FuzzyLevel2Criteria,'')  
			,FuzzyLevel2CriteriaOdd     = ISNULL( @FuzzyLevel2CriteriaOdd,'')  
			,FuzzyLevel2CriteriaEven    = ISNULL( @FuzzyLevel2CriteriaEven,'')  
			,FuzzyLevel3CriteriaOdd     = ISNULL( @FuzzyLevel3CriteriaOdd ,'')  
			,FuzzyLevel3CriteriaEven    = ISNULL( @FuzzyLevel3CriteriaEven ,'')  
			,FuzzyLevel4Criteria        = ISNULL( @FuzzyLevel4Criteria,'') 		  
			,FuzzyLevel5Criteria        = ISNULL( @FuzzyLevel5Criteria,'')  
			,FilterCriteria            = ISNULL( @FilterCriteria,'') 
			,FuzzyFactor = @FuzzyFactor 
			,SearchTerm1 = @Term1 
			,SearchTerm1Frequency = CASE WHEN @Term1 IS NULL THEN '' ELSE CAST(@Term1Frequency AS VARCHAR) END 
			,SearchTerm2 = ISNULL(@Term2,'') 
			,SearchTerm2Frequency = CASE WHEN @Term2 IS NULL THEN '' ELSE CAST(@Term2Frequency AS VARCHAR) END 
			,SearchTerm3 = ISNULL(@Term3,'') 
			,SearchTerm3Frequency = CASE WHEN @Term3 IS NULL THEN '' ELSE CAST(@Term3Frequency AS VARCHAR) END 
			,SearchTerm4 = ISNULL(@Term4,'') 
			,SearchTerm4Frequency = CASE WHEN @Term4 IS NULL THEN '' ELSE CAST(@Term4Frequency AS VARCHAR) END 
			,SearchTerm5 = ISNULL(@Term5,'') 
			,SearchTerm6Frequency = CASE WHEN @Term5 IS NULL THEN '' ELSE CAST(@Term5Frequency AS VARCHAR) END 
			,SearchTerm6 = ISNULL(@Term6,'') 
			,SearchTerm6Frequency = CASE WHEN @Term6 IS NULL THEN '' ELSE CAST(@Term6Frequency AS VARCHAR) END 
			,SearchTermCount = @RowCount 
		FOR XML PATH('') 
	) 
	RETURN @CriteriaXML 
END 

