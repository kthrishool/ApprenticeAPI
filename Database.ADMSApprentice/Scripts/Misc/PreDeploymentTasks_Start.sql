--[PreDeploymentTasks_Start.sql|2.0]
RAISERROR('STARTING PreDeploymentTasks_Start.sql...',10,1) WITH NOWAIT;
SET NOCOUNT ON;

DECLARE @PreDeploymentTasks_Start_Success				BIT
		,@PreDeploymentTasks_Start_DeploymentSequence	VARCHAR(50)		= 'PREDEPLOYMENT_START'
		,@PreDeploymentTasks_Start_DatabaseName			SYSNAME			= DB_NAME()
		,@PreDeploymentTasks_Start_ErrorHeaderString	NVARCHAR(50)	= [DeploymentAdmin].[ChangeControl].[GetOutputHeader]('red')
		,@PreDeploymentTasks_Start_NewVersionNumber		VARCHAR(21)		= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'NewVersionNumber')
		,@PreDeploymentTasks_Start_ServerName			NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@PreDeploymentTasks_Start_AuditMessage			VARCHAR(MAX)
		,@PreDeploymentTasks_Start_ScriptName			VARCHAR(50)		= 'PreDeploymentTasks_Start.sql';


BEGIN TRY
	EXEC DeploymentAdmin.[ChangeControl].[RunDeploymentTasks]
		@DatabaseName			= @PreDeploymentTasks_Start_DatabaseName
		,@Reference				= @PreDeploymentTasks_Start_NewVersionNumber
		,@DeploymentSequence	= @PreDeploymentTasks_Start_DeploymentSequence
		,@Success				= @PreDeploymentTasks_Start_Success OUTPUT 
		,@ScriptName			= @PreDeploymentTasks_Start_ScriptName;
END TRY
BEGIN CATCH
	SELECT @PreDeploymentTasks_Start_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @PreDeploymentTasks_Start_DatabaseName
		,@ServerName		= @PreDeploymentTasks_Start_ServerName
		,@AuditMessage		= @PreDeploymentTasks_Start_AuditMessage 
		,@ScriptName		= @PreDeploymentTasks_Start_ScriptName
		,@AuditType			= 'Error';

	EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'
	--failure of this script should NOT abort the deployment (not a showstopper)
END CATCH

IF @PreDeploymentTasks_Start_Success = 0
BEGIN
	SELECT @PreDeploymentTasks_Start_AuditMessage = 'Deployment Tasks Result: FAILED';

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @PreDeploymentTasks_Start_DatabaseName
		,@ServerName		= @PreDeploymentTasks_Start_ServerName
		,@AuditMessage		= @PreDeploymentTasks_Start_AuditMessage 
		,@ScriptName		= @PreDeploymentTasks_Start_ScriptName
		,@AuditType			= 'Error';

	RAISERROR('%s%s',10,1,@PreDeploymentTasks_Start_ErrorHeaderString,@PreDeploymentTasks_Start_AuditMessage) WITH NOWAIT;

	--failure of this script should NOT abort the deployment (not a showstopper)
END
GO --batch seperator added to prevent parsing of proceeding statements in deployment (which may otherwise prevent this script running first)
--[PreDeploymentTasks_Start.sql|2.0]