--[Pre_PreDeploymentTasks_Start.sql|2.0]
RAISERROR('STARTING Pre_PreDeploymentTasks_Start.sql...',10,1) WITH NOWAIT;
SET NOCOUNT ON;

DECLARE @PrePreDeploymentTasks_Start_Success				BIT
		,@PrePreDeploymentTasks_Start_DeploymentSequence	VARCHAR(50)		= 'PRE_PREDEPLOYMENT_START'
		,@PrePreDeploymentTasks_Start_DatabaseName			SYSNAME			= DB_NAME()
		,@PrePreDeploymentTasks_Start_ErrorMessage			VARCHAR(MAX)
		,@PrePreDeploymentTasks_Start_NewVersionNumber		VARCHAR(21)		= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'NewVersionNumber')
		,@PrePreDeploymentTasks_Start_ServerName			NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@PrePreDeploymentTasks_Start_AuditMessage			VARCHAR(MAX)
		,@PrePreDeploymentTasks_Start_ScriptName			VARCHAR(50)		= 'Pre_PreDeploymentTasks_Start.sql';



BEGIN TRY
	EXEC DeploymentAdmin.[ChangeControl].[RunDeploymentTasks]
		@DatabaseName			= @PrePreDeploymentTasks_Start_DatabaseName
		,@Reference				= @PrePreDeploymentTasks_Start_NewVersionNumber
		,@DeploymentSequence	= @PrePreDeploymentTasks_Start_DeploymentSequence
		,@Success				= @PrePreDeploymentTasks_Start_Success				OUTPUT 
		,@ErrorMessage			= @PrePreDeploymentTasks_Start_ErrorMessage			OUTPUT
		,@ScriptName			= @PrePreDeploymentTasks_Start_ScriptName;
END TRY
BEGIN CATCH --below catch block shouldn't acually occur because ChangeControl.RunDeploymentTasks internally catches exceptions
	SELECT @PrePreDeploymentTasks_Start_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @PrePreDeploymentTasks_Start_DatabaseName
		,@ServerName		= @PrePreDeploymentTasks_Start_ServerName
		,@AuditMessage		= @PrePreDeploymentTasks_Start_AuditMessage 
		,@ScriptName		= @PrePreDeploymentTasks_Start_ScriptName
		,@AuditType			= 'Error';

	EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'

	SELECT	@PrePreDeploymentTasks_Start_ErrorMessage = ', ' + ERROR_MESSAGE()
			,@PrePreDeploymentTasks_Start_Success = 0
END CATCH
	
IF @PrePreDeploymentTasks_Start_Success = 0
BEGIN
	--this will reraise the exception in order that the TFS task will report the failure
	EXEC [DeploymentAdmin].[ChangeControl].[DeploymentFailure]
		@DatabaseName	= @PrePreDeploymentTasks_Start_DatabaseName
		,@ErrorMessage	= @PrePreDeploymentTasks_Start_ErrorMessage
		,@ScriptName	= @PrePreDeploymentTasks_Start_ScriptName;
END
GO
--[Pre_PreDeploymentTasks_Start.sql|2.0]