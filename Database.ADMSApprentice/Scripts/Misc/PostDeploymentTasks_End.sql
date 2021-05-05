--[PostDeploymentTasks_End.sql|2.0]
RAISERROR('STARTING PostDeploymentTasks_End.sql...',10,1) WITH NOWAIT;
SET NOCOUNT ON;

DECLARE @PostDeploymentTasks_End_Success				BIT
		,@PostDeploymentTasks_End_DeploymentSequence	VARCHAR(50)		= 'POSTDEPLOYMENT_END'
		,@PostDeploymentTasks_End_DatabaseName			SYSNAME			= DB_NAME()
		,@PostDeploymentTasks_End_ErrorHeaderString		NVARCHAR(50)	= [DeploymentAdmin].[ChangeControl].[GetOutputHeader]('red')
		,@PostDeploymentTasks_End_NewVersionNumber		VARCHAR(21)		= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'NewVersionNumber')	
		,@PostDeploymentTasks_End_ServerName			NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@PostDeploymentTasks_End_AuditMessage			VARCHAR(MAX)
		,@PostDeploymentTasks_End_ScriptName			VARCHAR(50)		= 'PostDeploymentTasks_End.sql';


BEGIN TRY
	EXEC DeploymentAdmin.[ChangeControl].[RunDeploymentTasks]
		@DatabaseName			= @PostDeploymentTasks_End_DatabaseName
		,@Reference				= @PostDeploymentTasks_End_NewVersionNumber
		,@DeploymentSequence	= @PostDeploymentTasks_End_DeploymentSequence
		,@Success				= @PostDeploymentTasks_End_Success OUTPUT 
		,@ScriptName			= @PostDeploymentTasks_End_ScriptName;
END TRY
BEGIN CATCH
	SELECT @PostDeploymentTasks_End_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @PostDeploymentTasks_End_DatabaseName
		,@ServerName		= @PostDeploymentTasks_End_ServerName
		,@AuditMessage		= @PostDeploymentTasks_End_AuditMessage 
		,@ScriptName		= @PostDeploymentTasks_End_ScriptName
		,@AuditType			= 'Error';

	EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'
	--failure of this script should NOT abort the deployment (not a showstopper)
END CATCH

IF @PostDeploymentTasks_End_Success = 0
BEGIN
	SELECT @PostDeploymentTasks_End_AuditMessage = 'Deployment Tasks Result: FAILED';

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @PostDeploymentTasks_End_DatabaseName
		,@ServerName		= @PostDeploymentTasks_End_ServerName
		,@AuditMessage		= @PostDeploymentTasks_End_AuditMessage 
		,@ScriptName		= @PostDeploymentTasks_End_ScriptName
		,@AuditType			= 'Error';

	RAISERROR('%s%s',10,1,@PostDeploymentTasks_End_ErrorHeaderString,@PostDeploymentTasks_End_AuditMessage) WITH NOWAIT;

	--failure of this script should NOT abort the deployment (not a showstopper)
END
GO --batch seperator added to prevent parsing of proceeding statements in deployment (which may otherwise prevent this script running first)
--[PostDeploymentTasks_End.sql|2.0]