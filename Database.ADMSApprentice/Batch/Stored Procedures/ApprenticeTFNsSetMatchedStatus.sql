
/**************************************************************************
[Batch].[ApprenticeTFNsSetMatchedStatus]
Description:	Sets the StatusCode to matched / unmatched for the list of TFNs provided

Usage:			[Batch].[ApprenticeTFNsSetMatchedStatus]
					@CurrentDateTime = NULL

Author:			JD3044
Created:		19/03/2021

Modification History:
UserId	Date		    Description
=======	==========		====================================================
PS1508	14/04/2021		Return list of records thet are successfully updated
***************************************************************************/
CREATE PROC [Batch].[ApprenticeTFNsSetMatchedStatus]
    @CurrentDateTime DATETIME2 = NULL
	,@UpdatedBy NVARCHAR(1024) = 'tbc'
	,@ApprenticeTFNMatchStatusListType [Batch].[ApprenticeTFNMatchStatusListType] READONLY
	,@AuditEventId BIGINT
	,@OutputLog VARCHAR(MAX) OUTPUT
AS
BEGIN
	SET NOCOUNT ON
	BEGIN TRY
  
		BEGIN TRANSACTION

		SET @OutputLog ='##############################################################################################################################'+CHAR(13)+CHAR(10);
		SET @OutputLog = @OutputLog + 'Stored Procedure [ADMSApprentice].[Batch].[ApprenticeTFNsSetMatchedStatus] commencing at '+CONVERT(VARCHAR(28),SYSDATETIME(),121)+CHAR(13)+CHAR(10);
		SET @OutputLog = @OutputLog + '##############################################################################################################################'+CHAR(13)+CHAR(10);

		-- If no date is passed then default to now
		SELECT  @CurrentDateTime = ISNULL(@CurrentDateTime, GETDATE());

		IF NOT EXISTS (SELECT TOP 1 1 FROM [Infrastructure].[AuditEvent] WHERE AuditEventId = @AuditEventId)
			RAISERROR('Invalid @AuditEventId provided.', 16, 1);

		DECLARE @RowCount INT = 0;

		SELECT @RowCount = COUNT(*) FROM @ApprenticeTFNMatchStatusListType;

		SET @OutputLog = @OutputLog + CONVERT(VARCHAR(10),@RowCount)+' ApprenticeTFN record(s) requested to be updated.'+CHAR(13)+CHAR(10);

		UPDATE X
		SET StatusCode = S.StatusCode
			,StatusReasonCode = S.StatusReasonCode
			,StatusDate = @CurrentDateTime
			,UpdatedBy = @UpdatedBy
			,UpdatedOn = @CurrentDateTime
			,_AuditEventId = @AuditEventId
		FROM [dbo].[ApprenticeTFN] X
		INNER JOIN @ApprenticeTFNMatchStatusListType S
			ON S.[MessageQueueCorrelationId] = X.[MessageQueueCorrelationId];

		SET @RowCount = @@ROWCOUNT

		SET @OutputLog = @OutputLog + CONVERT(VARCHAR(10),@RowCount)+' ApprenticeTFN record(s) were updated.'+CHAR(13)+CHAR(10);

		SELECT 
			 X.[MessageQueueCorrelationId]
			,X.StatusCode
			,X.StatusReasonCode
			,X.StatusDate 
		FROM [dbo].[ApprenticeTFN] X
		INNER JOIN @ApprenticeTFNMatchStatusListType S
			ON S.[MessageQueueCorrelationId] = X.[MessageQueueCorrelationId];

	END TRY

	BEGIN CATCH
		IF XACT_STATE() = -1 OR @@TRANCOUNT >= 1
			ROLLBACK TRANSACTION

		DECLARE @ErrorMessage  nvarchar(1000),
				@ErrorSeverity int,
				@ErrorState    int;
				      
		-- raise user defined error message the error again as it has been consumed by the catch block
		SELECT @ErrorSeverity = ERROR_SEVERITY(),
				@ErrorState    = ERROR_STATE(),
				@ErrorMessage  = ERROR_MESSAGE();

		RAISERROR(@ErrorMessage,@ErrorSeverity,@ErrorState);

		SET @OutputLog = @OutputLog + CHAR(13)+CHAR(10);
		SET @OutputLog = @OutputLog + '##############################################################################################################################'+CHAR(13)+CHAR(10);
		SET @OutputLog = @OutputLog + 'Error encountered: '+@ErrorMessage + CHAR(13)+CHAR(10);
		SET @OutputLog = @OutputLog + 'Stored Procedure [ADMSApprentice].[Batch].[ApprenticeTFNsSetMatchedStatus] completed at '+CONVERT(VARCHAR(28),SYSDATETIME(),121)+CHAR(13)+CHAR(10);
		SET @OutputLog = @OutputLog + 'BATCH FAILURE'+CHAR(13)+CHAR(10);
		SET @OutputLog = @OutputLog + '##############################################################################################################################'+CHAR(13)+CHAR(10);

		RETURN ERROR_NUMBER();
	END CATCH

/*-------------------- FINALISATION -------------------------------------*/
	-- Normal Exit Processing
	IF @@TRANCOUNT > 0 
		BEGIN
			COMMIT TRANSACTION
		END
	-- Normal Exit Processing
	SET @OutputLog = @OutputLog + CHAR(13)+CHAR(10);
	SET @OutputLog = @OutputLog + '##############################################################################################################################'+CHAR(13)+CHAR(10);
	SET @OutputLog = @OutputLog + 'Stored Procedure [ADMSApprentice].[Batch].[ApprenticeTFNsSetMatchedStatus] completed at '+CONVERT(VARCHAR(28),SYSDATETIME(),121)+CHAR(13)+CHAR(10);
	SET @OutputLog = @OutputLog + '##############################################################################################################################'+CHAR(13)+CHAR(10);

	-- Return 0 to indicate successful run of procedure
	RETURN(0);
END

