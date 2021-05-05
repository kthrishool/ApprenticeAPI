--[PostDeploymentTasks_Start.sql|2.0]
RAISERROR('STARTING PostDeploymentTasks_Start.sql...',10,1) WITH NOWAIT;
SET NOCOUNT ON;

DECLARE @PostDeploymentTasks_Start_Success				BIT
		,@PostDeploymentTasks_Start_DeploymentSequence	VARCHAR(50)		= 'POSTDEPLOYMENT_START'
		,@PostDeploymentTasks_Start_DatabaseName		SYSNAME			= DB_NAME()
		,@PostDeploymentTasks_Start_ErrorHeaderString	NVARCHAR(50)	= [DeploymentAdmin].[ChangeControl].[GetOutputHeader]('red')
		,@PostDeploymentTasks_Start_NewVersionNumber	VARCHAR(21)		= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'NewVersionNumber')
		,@PostDeploymentTasks_Start_ServerName			NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@PostDeploymentTasks_Start_AuditMessage		VARCHAR(MAX)
		,@PostDeploymentTasks_Start_ScriptName			VARCHAR(50)		= 'PostDeploymentTasks_Start.sql';


BEGIN TRY
	EXEC DeploymentAdmin.[ChangeControl].[RunDeploymentTasks]
		@DatabaseName			= @PostDeploymentTasks_Start_DatabaseName
		,@Reference				= @PostDeploymentTasks_Start_NewVersionNumber
		,@DeploymentSequence	= @PostDeploymentTasks_Start_DeploymentSequence
		,@Success				= @PostDeploymentTasks_Start_Success OUTPUT 
		,@ScriptName			= @PostDeploymentTasks_Start_ScriptName;
END TRY
BEGIN CATCH
	SELECT @PostDeploymentTasks_Start_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @PostDeploymentTasks_Start_DatabaseName
		,@ServerName		= @PostDeploymentTasks_Start_ServerName
		,@AuditMessage		= @PostDeploymentTasks_Start_AuditMessage 
		,@ScriptName		= @PostDeploymentTasks_Start_ScriptName
		,@AuditType			= 'Error';

	EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'
	--failure of this script should NOT abort the deployment (not a showstopper)
END CATCH

IF @PostDeploymentTasks_Start_Success = 0
BEGIN
	SELECT @PostDeploymentTasks_Start_AuditMessage = 'Deployment Tasks Result: FAILED';

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @PostDeploymentTasks_Start_DatabaseName
		,@ServerName		= @PostDeploymentTasks_Start_ServerName
		,@AuditMessage		= @PostDeploymentTasks_Start_AuditMessage 
		,@ScriptName		= @PostDeploymentTasks_Start_ScriptName
		,@AuditType			= 'Error';

	RAISERROR('%s%s',10,1,@PostDeploymentTasks_Start_ErrorHeaderString,@PostDeploymentTasks_Start_AuditMessage) WITH NOWAIT;

	--failure of this script should NOT abort the deployment (not a showstopper)
END
GO --batch seperator added to prevent parsing of proceeding statements in deployment (which may otherwise prevent this script running first)
--[PostDeploymentTasks_Start.sql|2.0]