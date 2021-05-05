
/*--------------------------------------------------------------------------------   
     
  Object            : Based off [EMP].[dbo].[spJobseekerQuickFind]   
   

----------------------------------------------------------------------------------*/    
CREATE PROC [dbo].[ApprenticeQuickFind]   
	 @SearchString  varchar(200) = null   
AS   
 
BEGIN   
	SET NOCOUNT ON;   
	DECLARE   
		-- working variables   
		 @Name1 varchar(50)   
		,@Name2 varchar(50)   
		,@CommaPos tinyint   
		,@SpacePos tinyint   
		,@ApprenticeId INT   
   
		-- Error Variables   
		--,@ErrorMessage   varchar(1000)   
		--,@DatabaseName   varchar(128)   
		--,@ServerName     varchar(128)   
		--,@ErrorLogId     int   
		--,@ErrorSeverity  int   
		--,@ErrorState     int   
		--,@UpdatedOn      datetime   
		,@CustomerReferenceNumber	 char(10)   
	    ,@NumericSearchString varchar(200)   
   
	BEGIN TRY   
   
        SET @NumericSearchString = LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE( REPLACE(  
        @SearchString, ' ',''),'\', ''), '-', ''), ',', ''), '+', ''), '$', ''), '.', ''), CHAR(9), ''), CHAR(0), ''))) -- Replace everything except digits 0-9   
   
   
		IF LEN(@NumericSearchString) <= 10 AND ISNUMERIC(@NumericSearchString) = 1 AND CHARINDEX ('E',@NumericSearchString,0) = 0  -- ApprenticeId   
			SET @ApprenticeId = CAST(@NumericSearchString AS BIGINT)   
		ELSE   
			IF LEN(@NumericSearchString) = 10 AND ISNUMERIC(LEFT(@NumericSearchString, LEN(@NumericSearchString) - 1)) = 1   
				SET @CustomerReferenceNumber = CAST(@NumericSearchString AS CHAR(10))   
			ELSE   
			BEGIN    
				SET @SearchString = LTRIM(RTRIM(@SearchString))   
				SET @CommaPos = CHARINDEX(',', @SearchString, 0)   
				SET @SpacePos = CHARINDEX(' ', LTRIM(RTRIM(@SearchString)), 0)   
   
				IF @CommaPos > 0    
				BEGIN   
					SELECT   
						 @Name1   = LTRIM(RTRIM(LEFT (@SearchString, @CommaPos-1)))   
						,@Name2 = LTRIM(RTRIM(RIGHT(@SearchString, LEN(@SearchString) - @CommaPos)))   
				END   
				ELSE   
				BEGIN    
					SET @SpacePos = CHARINDEX(' ', LTRIM(RTRIM(@SearchString)), 0)   
					IF @SpacePos > 0    
					BEGIN   
						SELECT   
							 @Name1   = LTRIM(RTRIM(LEFT (@SearchString, @SpacePos-1)))   
							,@Name2 = LTRIM(RTRIM(RIGHT(@SearchString, LEN(@SearchString) - @SpacePos)))   
					END   
					ELSE   
						SET @Name1 = LTRIM(RTRIM(@SearchString)) -- no comma must be just the surname which is mandatory for a name search   
				END   
				   
			END   
   
		IF @ApprenticeId IS NOT NULL OR @CustomerReferenceNumber IS NOT NULL   
		BEGIN    
            --13 Aug 2015 : MS0479 - Split the search query into CRN/JSID to achieve improved query plan   
            IF @ApprenticeId IS NOT NULL   
				SELECT TOP 1   
					A.ApprenticeId
					--js.JobSeekerId    
					,A.BirthDate   
					,A.Surname    
					,A.[FirstName]   
					,AA.[Locality]  --= CASE WHEN [SensitiveClientFlag] = 1 THEN '*********' ELSE ISNULL(jsa.Locality,'') END   
					,AA.[PostCode]  --= CASE WHEN [SensitiveClientFlag] = 1 THEN '****' ELSE ISNULL(jsa.PostCode,'') END   
				FROM   
					[dbo].Apprentice A
					LEFT JOIN [dbo].[ApprenticeAddress] AA WITH (NOLOCK)   
						ON A.ApprenticeId = AA.ApprenticeId
						AND AA.AddressTypeCode IN ('RESD', 'POST')  
			    WHERE   
				    A.ApprenticeId =  @ApprenticeId 
			    ORDER BY    
				    AA.AddressTypeCode DESC   
			    OPTION (FORCE ORDER)   
   
            ELSE  --@CustomerReferenceNumber IS NOT NULL   
               
				SELECT TOP 1   
					A.ApprenticeId
					--js.JobSeekerId    
					,A.BirthDate   
					,A.Surname    
					,A.[FirstName]   
					,AA.[Locality]  --= CASE WHEN [SensitiveClientFlag] = 1 THEN '*********' ELSE ISNULL(jsa.Locality,'') END   
					,AA.[PostCode]  --= CASE WHEN [SensitiveClientFlag] = 1 THEN '****' ELSE ISNULL(jsa.PostCode,'') END    
				FROM   
					[dbo].Apprentice A   
					LEFT JOIN [dbo].[ApprenticeAddress] AA WITH (NOLOCK)   
					ON A.ApprenticeId = AA.ApprenticeId   
						AND AA.AddressTypeCode IN ('RESD', 'POST')                                          
					--	AND jsa.EndDate IS NULL   
					--LEFT JOIN [JobSeeker].[JskExtCharacteristic] e   
					--	ON js.JobSeekerId = e.JobSeekerId   
			    WHERE   
					A.CustomerReferenceNumber =  @CustomerReferenceNumber   
			    ORDER BY    
				    AA.AddressTypeCode DESC   
			    OPTION (FORCE ORDER)               
   
		END --IF @ApprenticeId IS NOT NULL OR @CustomerReferenceNumber IS NOT NULL   
		ELSE   
           
		BEGIN    
			WITH Candidates   
			AS   
			(   
				SELECT TOP 20    
					A.ApprenticeId    
					,BirthDate   
					,Surname    
					,[FirstName]   
					,AA.Locality   
					,AA.PostCode   
					,SequenceNo =    
						ROW_NUMBER() OVER   
						(   
							ORDER BY    
								CASE    
									WHEN    
										[Surname]  = @Name1 AND [FirstName] = @Name2    
										OR [Surname]  = @Name2 AND [FirstName] = @Name1 THEN 100    
									WHEN 
										[Surname]  = @Name1 AND [FirstName] LIKE @Name2 + '%'
										OR [Surname]  = @Name2 AND [FirstName] LIKE @Name1 + '%' THEN 95
									WHEN    
										[Surname]  like @Name1 + '%' AND [FirstName] like @Name2  + '%'   
										OR [Surname] like @Name2 + '%' AND [FirstName] like @Name1 + '%' THEN 90   
									WHEN [Surname]  = @Name1 THEN 80   
									WHEN [FirstName]  = @Name1 THEN 60   
									WHEN [Surname]  like @Name1 + '%' THEN 50   
									WHEN [FirstName]  like @Name1 + '%' THEN 40   
   
									ELSE   
										0   
								END DESC   
								, AA.AddressTypeCode DESC   
   
							)   
				FROM   
					[dbo].[Apprentice] A
					LEFT JOIN [dbo].[ApprenticeAddress] AA -- 99% will have a residential address so granb this here   
						ON A.ApprenticeId = AA.ApprenticeId   
						AND AA.AddressTypeCode IN ('RESD')                                          
				WHERE   
							( [FirstName] LIKE @Name1 + '%' AND ( [Surname] LIKE @Name2 + '%' OR @Name2 IS NULL))   
							OR    
							( [Surname] LIKE @Name1 + '%' AND ( [FirstName] LIKE @Name2 + '%' OR @Name2 IS NULL))   

			)   
			SELECT  DISTINCT  
					c.ApprenticeId    
					,c.BirthDate   
					,c.[Surname]    
					,c.[FirstName]   
					,[Locality] = COALESCE(c.Locality,AA.Locality, '') -- = CASE WHEN [SensitiveClientFlag] = 1 THEN '*********' ELSE COALESCE(c.Locality,jsa.Locality, '') END   
					,[PostCode] = COALESCE(c.PostCode,AA.PostCode, '')-- = CASE WHEN [SensitiveClientFlag] = 1 THEN '****'		ELSE COALESCE(c.PostCode,jsa.PostCode, '') END   
					,SequenceNo
			FROM    
				Candidates c   
				LEFT JOIN [dbo].[ApprenticeAddress] AA WITH (NOLOCK)   
					ON c.ApprenticeId = AA.ApprenticeId
					AND c.Locality IS NULL -- only if residential was not found   
					AND AA.AddressTypeCode IN ('POST') -- get the postal address                                       
			WHERE   
				SequenceNO <= 20   
			ORDER BY SequenceNo
			OPTION (FORCE ORDER)   
		END   
    END TRY   
   
/*-------------------- ERROR HANDLING -------------------------------------*/   
    BEGIN CATCH      
		THROW 
    END CATCH   
   
   
/*-------------------- FINALISATION -------------------------------------*/   
   
  -- Return 0 to indicate successful run of procedure   
  RETURN(0)    
   
END   
/*-------------------- END PROCEDURE --------------------------------------*/   
