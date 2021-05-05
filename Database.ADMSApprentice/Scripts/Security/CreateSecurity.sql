--[CreateSecurity.sql|2.0]
RAISERROR('STARTING CreateSecurity.sql...',10,1) WITH NOWAIT;
DECLARE @CreateSecurity_DatabaseName		VARCHAR(255)	= DB_NAME()
		,@CreateSecurity_NewVersionNumber	VARCHAR(21)		= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'NewVersionNumber')	
		,@CreateSecurity_ServerName			NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@CreateSecurity_AuditMessage		VARCHAR(MAX)
		,@CreateSecurity_ScriptName			VARCHAR(50)		= 'CreateSecurity.sql';



BEGIN TRY
	EXEC [DeploymentAdmin].[ChangeControl].[CreateSecurity]
		@DatabaseName	= @CreateSecurity_DatabaseName
		,@Reference		= @CreateSecurity_NewVersionNumber
		,@ScriptName	= @CreateSecurity_ScriptName;
		
END TRY
BEGIN CATCH
	SELECT @CreateSecurity_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @CreateSecurity_DatabaseName
		,@ServerName		= @CreateSecurity_ServerName
		,@AuditMessage		= @CreateSecurity_AuditMessage 
		,@ScriptName		= @CreateSecurity_ScriptName
		,@AuditType			= 'Error';
	
	EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'

	EXEC [DeploymentAdmin].[ChangeControl].[DeploymentFailure]
		@DatabaseName	= @CreateSecurity_DatabaseName
		,@ScriptName	= @CreateSecurity_ScriptName;

	--failure of this script should prevent further predeployment scripts from being run (treat as a showstopper)
	SET NOEXEC ON;
END CATCH
GO
--[CreateSecurity.sql|2.0]