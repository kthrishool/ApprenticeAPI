CREATE PROC [Infrastructure].[upAuditEventInsert]
	 @EventName varchar(200)
	,@ApplicationUsername varchar(1024) = NULL
	,@CorrelationID uniqueidentifier = NULL
	,@ReferenceNumber varchar(50) = NULL
	,@AuditEventId bigint = NULL OUTPUT

AS
/*--------------------------------------------------------------------------------
  Author			: Mick Vullo
  Version			: 1.00
  CreatedDate		: 02 Apr 2015
  
  Object            : [Infrastructure].[upAuditEventInsert]

  Description		: 
		Sets up an AuditEvent Record which can then be associated with audit records via the CONTEXT_INFO

  Parameters

	 @EventName	- Name to identify the unit of work (all DML statements that run under the current SPID)
	,@ApplicationUsername - User name for the unit of work
	,@CorrelationID - application CorrelationID used for debugging
	,@ReferenceNumber - Change request or defect number linked to the event usually as a resuult of manual intervention from prod support
	,@AuditEventId bigint - output id

  Example Usage   

	DECLARE @AuditEventId bigint
	EXEC [Infrastructure].[upAuditEventInsert]
		@EventName = 'Defect #9697 - ManualVerifyFlag Fix'
		,@ApplicationUsername = 'Admin'
		,@AuditEventId = @AuditEventId OUTPUT

	SELECT @AuditEventId

  Ammendment History:
  
----------------------------------------------------------------------------------*/
	SELECT
		@AuditEventId = AuditEventId
	FROM 
		[Infrastructure].[AuditEvent]
	WHERE
		AuditEventId = CAST(CAST(CONTEXT_INFO() AS Binary(8)) AS bigint)
		AND EventName = @EventName
		AND ApplicationUsername = @ApplicationUsername
		AND (ReferenceNumber = @ReferenceNumber  OR (@ReferenceNumber IS NULL AND ReferenceNumber IS NULL))

	IF @AuditEventId IS NULL
	BEGIN 
		SELECT 
			@CorrelationID = ISNULL(@CorrelationID, NEWID())
			,@ApplicationUsername = ISNULL(@ApplicationUsername, SYSTEM_USER)

		INSERT INTO [Infrastructure].[AuditEvent]
		(
			[CorrelationId]
			,[EventName]
			,[ApplicationUsername]
			,CreatedOn
			,ProcessStatus
			,ReferenceNumber
		)
		VALUES 
		(
			@CorrelationID
			,@EventName
			,@ApplicationUsername
			,GETDATE()
			,0
			,@ReferenceNumber
		)

		SET @AuditEventId = SCOPE_IDENTITY()

		-- mimic the code which does an insert followed by an update to force the update trigger to fire and lock  the AuditEventId into the ContextInfo 
		UPDATE [Infrastructure].[AuditEvent]
		SET ProcessStatus = 1
		WHERE AuditEventId = @AuditEventId
	END
	RETURN 0 

