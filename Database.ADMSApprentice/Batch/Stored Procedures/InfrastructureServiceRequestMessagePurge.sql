

CREATE PROC [Batch].[InfrastructureServiceRequestMessagePurge]
	 @RetentionHours int = 96
	,@BatchSize int = 100000
	,@TerminateTime char(8) = '05:00:00'
AS

/*--------------------------------------------------------------------------------
  Author			: Mick Vullo
  Version			: 1.00
  CreatedDate		: 17 Nov 2015
  
  Object            :  [Batch].[InfrastructureServiceRequestMessagePurge]

  Description		: 
		Purges Service Request Messages which are only required to prevent duplicate web api calls
		Should only be run after hours during periods of low web api activity

  Parameters
	  @RetentionHours - hours of data to retain - defaults to 96 hours
	  @BatchSize      - Records to delete in one hit so as to minimise blocking

  Example Usage   

	EXEC [Batch].[InfrastructureServiceRequestMessagePurge]

  Ammendment History:
  
	28 Apr 2016 - Added the time constraint in the loop to ensure the purge does not continue on the online day
----------------------------------------------------------------------------------*/
DECLARE 

	 @LastId bigint
	,@FirstId bigint
	,@PurgeStartDate Datetime
	,@PurgeEndDate  Datetime

BEGIN
	SET NOCOUNT ON;
	BEGIN TRY	

		SET @PurgeEndDate = DATEADD(hour, @RetentionHours * -1, GETDATE())

		SELECT 
			 @FirstId = MIN ([ServiceRequestMessageId])
			,@LastId = MAX([ServiceRequestMessageId])
		FROM 
			[Infrastructure].[ServiceRequestMessage] WITH (NOLOCK)
		WHERE 
			[RequestReceivedDateTime] < @PurgeEndDate

		WHILE @FirstId < @LastId AND CAST (GETDATE() AS TIME ) < @TerminateTime		-- Use the Id's to do the purge for efficiency
		BEGIN 
	
			DELETE FROM [Infrastructure].[ServiceRequestMessage] 
			WHERE [ServiceRequestMessageId] BETWEEN @FirstId AND CASE WHEN (@FirstId + @BatchSize) > @LastId THEN @LastId ELSE (@FirstId + @BatchSize) END

			SET @FirstId = @FirstId + @BatchSize
			WAITFOR DELAY '00:00:01'

		END
	END TRY

	/*-------------------- ERROR HANDLING -------------------------------------*/
	--  DO NOT MODIFY THIS SECTION
	BEGIN CATCH

		/*-------------------- VARIABLE DECLARATION -------------------------------*/
		DECLARE @ErrorMessage  nvarchar(1000),
				@DatabaseName  varchar(128),
				@ServerName    varchar(128),
				@ErrorLogId    int = 0,
				@ErrorSeverity int,
				@ErrorState    int,
				@SystemDate    datetime = GetDate()
      
		SELECT @DatabaseName = DB_NAME(), @ServerName = @@SERVERNAME

		-- raise user defined error message the error again as it has been consumed by the catch block
		SELECT @ErrorSeverity = ERROR_SEVERITY(),
				@ErrorState    = ERROR_STATE(),
				@ErrorMessage  = ERROR_MESSAGE()
		RAISERROR(@ErrorMessage,@ErrorSeverity,@ErrorState)

		RETURN ERROR_NUMBER()
	END CATCH
/*-------------------- FINALISATION -------------------------------------*/

-- Return 0 to indicate successful run of procedure
	RETURN(0) 

 END
/*-------------------- END PROCEDURE --------------------------------------*/