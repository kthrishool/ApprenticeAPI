--[CreateSnapshot.sql|2.0]
RAISERROR('STARTING CreateSnapshot.sql...',10,1) WITH NOWAIT;

DECLARE @CreateSnapshot_rc					INT				= 0
		,@CreateSnapshot_NewVersionNumber	VARCHAR(21)		= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'NewVersionNumber')	
		,@CreateSnapshot_NewSnapshotName					VARCHAR(128)			
		,@CreateSnapshot_DatabaseName		VARCHAR(255)	= DB_NAME() --TFS executes this script directly, so cant use SQLCMD variable $(DatabaseName)
		,@CreateSnapshot_ErrorHeaderString	NVARCHAR(50)	= [DeploymentAdmin].[ChangeControl].[GetOutputHeader]('red')
		,@CreateSnapshot_ServerName			NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@CreateSnapshot_AuditMessage		VARCHAR(MAX)
		,@CreateSnapshot_ScriptName			VARCHAR(50)		= 'CreateSnapshot.sql';


BEGIN TRY
	EXEC @CreateSnapshot_rc = [DeploymentAdmin].ChangeControl.CreateSnapshot 
		@DatabaseName			= @CreateSnapshot_DatabaseName
		,@NewVersionNumber		= @CreateSnapshot_NewVersionNumber
		,@NewSnapshotName		= @CreateSnapshot_NewSnapshotName					OUTPUT;

	--cache value for later use in validation script
	EXEC [DeploymentAdmin].ChangeControl.InsertDeploymentConfiguration
		@DatabaseName			= @CreateSnapshot_DatabaseName
		,@ConfigurationName		= 'NewSnapshotName'
		,@ConfigurationValue	= @CreateSnapshot_NewSnapshotName
		,@Reference				= @CreateSnapshot_NewVersionNumber

	IF @CreateSnapshot_rc = 0
	BEGIN
		RAISERROR('[DeploymentAdmin].ChangeControl.CreateSnapshot return value unsuccessful',16,1) WITH NOWAIT;
	END
END TRY
BEGIN CATCH

	SELECT @CreateSnapshot_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @CreateSnapshot_DatabaseName
		,@ServerName		= @CreateSnapshot_ServerName
		,@AuditMessage		= @CreateSnapshot_AuditMessage 
		,@ScriptName		= @CreateSnapshot_ScriptName
		,@AuditType			= 'Error';

	--this is needed to prevent further predeployment scripts from being run, a workaround as the following is not yet supported ( :on error exit )
	RAISERROR('%sFailed - error detected in CreateSnapshot.sql', 16, 1,@CreateSnapshot_ErrorHeaderString);

	EXEC [DeploymentAdmin].[ChangeControl].[DeploymentFailure]
		@DatabaseName	= @CreateSnapshot_DatabaseName
		,@ScriptName	= @CreateSnapshot_ScriptName;

	--failure of this script should prevent further predeployment scripts from being run (treat as a showstopper)
	SET NOEXEC ON;
END CATCH
GO
--[CreateSnapshot.sql|2.0]