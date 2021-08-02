

CREATE PROCEDURE [Batch].[InfrastructureServiceBusEventArchivePurge]
(
	@RetentionHours		int			= 96
	,@BatchSize			int			= 100000
	,@TerminateTime		char(5)		= '05:00'
)
AS
/*--------------------------------------------------------------------------------
  Author			: Marcus Chadwick
  Version			: 1.00
  CreatedDate		: 02-02-2018
  
  Object            :  [Batch].[InfrastructureServiceBusEventArchivePurge]

  Description		: 
		Purges Service Bus Events which are only required for audit,  hence are CDC'd to DW 
		Should only be run after hours during periods of low web api activity

  Parameters
	  @RetentionHours - hours of data to retain - defaults to 96 hours
	  @BatchSize      - Records to delete in one hit so as to minimise blocking
	  @TerminateTime  - Last time to run purge so as not to interfere with online day

JL0758	03/09/2018	Modified to delete ServiceBusEventArchive instead of ServiceBusEvent
----------------------------------------------------------------------------------*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @PurgeEndDate		datetime		= DATEADD(HOUR, ABS(@RetentionHours) * -1, GETDATE())
			,@RetentionLastId	bigint


	SELECT	@RetentionLastId					= MAX(Id)
	FROM	Infrastructure.ServiceBusEventArchive							WITH (NOLOCK)
	WHERE	Created								< @PurgeEndDate

	BEGIN TRY	
		WHILE COALESCE(@RetentionLastId,0) > 0 AND CAST(GETDATE() AS TIME) < @TerminateTime
		BEGIN
			RAISERROR('Deleting next (%d) batch of rows from [Infrastructure].[ServiceBusEventArchive]...',10,1,@BatchSize) WITH NOWAIT;

			DELETE	TOP (@BatchSize) x
			FROM	[Infrastructure].[ServiceBusEventArchive]	x
			WHERE	x.Id <= @RetentionLastId
			AND		x.Status		= 2 --has been successfully processed

			IF @@ROWCOUNT=0
				BREAK;
				
			WAITFOR DELAY '00:00:01'
		END

		RAISERROR('No rows eligible for data purge ([Infrastructure].[ServiceBusEventArchive]) or current time later than defined end time (%s), aborting',10,1,@TerminateTime) WITH NOWAIT;
	END TRY
	BEGIN CATCH
		THROW;
		RETURN ERROR_NUMBER()
	END CATCH
	
	RETURN(0) 
END