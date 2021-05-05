--[PreDeploymentTasks_End.sql|2.0]
RAISERROR('STARTING PreDeploymentTasks_End.sql...',10,1) WITH NOWAIT;
SET NOCOUNT ON;

DECLARE @PreDeploymentTasks_End_Success					BIT
		,@PreDeploymentTasks_End_DeploymentSequence		VARCHAR(50)		= 'PREDEPLOYMENT_END'
		,@PreDeploymentTasks_End_DatabaseName			SYSNAME			= DB_NAME()		
		,@PreDeploymentTasks_End_ErrorHeaderString		NVARCHAR(50)	= [DeploymentAdmin].[ChangeControl].[GetOutputHeader]('red')
		,@PreDeploymentTasks_End_NewVersionNumber		VARCHAR(21)		= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'NewVersionNumber')

		,@PreDeploymentTasks_End_ServerName				NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@PreDeploymentTasks_End_AuditMessage			VARCHAR(MAX)
		,@PreDeploymentTasks_End_ScriptName				VARCHAR(50)		= 'PreDeploymentTasks_End.sql';

BEGIN TRY
	EXEC DeploymentAdmin.[ChangeControl].[RunDeploymentTasks]
		@DatabaseName			= @PreDeploymentTasks_End_DatabaseName
		,@Reference				= @PreDeploymentTasks_End_NewVersionNumber
		,@DeploymentSequence	= @PreDeploymentTasks_End_DeploymentSequence
		,@Success				= @PreDeploymentTasks_End_Success OUTPUT 
		,@ScriptName			= @PreDeploymentTasks_End_ScriptName;
END TRY
BEGIN CATCH
	SELECT @PreDeploymentTasks_End_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @PreDeploymentTasks_End_DatabaseName
		,@ServerName		= @PreDeploymentTasks_End_ServerName
		,@AuditMessage		= @PreDeploymentTasks_End_AuditMessage 
		,@ScriptName		= @PreDeploymentTasks_End_ScriptName
		,@AuditType			= 'Error';

	EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'
	--failure of this script should NOT abort the deployment (not a showstopper)
END CATCH

IF @PreDeploymentTasks_End_Success = 0
BEGIN
	SELECT @PreDeploymentTasks_End_AuditMessage = 'Deployment Tasks Result: FAILED';

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @PreDeploymentTasks_End_DatabaseName
		,@ServerName		= @PreDeploymentTasks_End_ServerName
		,@AuditMessage		= @PreDeploymentTasks_End_AuditMessage 
		,@ScriptName		= @PreDeploymentTasks_End_ScriptName
		,@AuditType			= 'Error';

	RAISERROR('%s%s',10,1,@PreDeploymentTasks_End_ErrorHeaderString,@PreDeploymentTasks_End_AuditMessage) WITH NOWAIT;

	--failure of this script should NOT abort the deployment (not a showstopper)
END
GO --batch seperator added to prevent parsing of proceeding statements in deployment (which may otherwise prevent this script running first)
--[PreDeploymentTasks_End.sql|2.0]