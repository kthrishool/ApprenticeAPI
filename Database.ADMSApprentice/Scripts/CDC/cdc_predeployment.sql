--[cdc_predeployment.sql|2.0]
RAISERROR('STARTING cdc_predeployment.sql...',10,1) WITH NOWAIT;
SET NOCOUNT ON;

DECLARE @cdc_predeployment_DatabaseName				VARCHAR(255)	= DB_NAME() --TFS executes this script directly, so cant use SQLCMD variable $(DatabaseName)
		,@cdc_predeployment_Environment				VARCHAR(50)		= [DeploymentAdmin].ChangeControl.GetEnvironment()
		,@cdc_predeployment_TableNames				NVARCHAR(MAX)	= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'CDCTablesWithChanges')
		,@cdc_predeployment_NewVersionNumber		VARCHAR(21)		= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'NewVersionNumber')	
		,@cdc_predeployment_ServerName				NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@cdc_predeployment_AuditMessage			VARCHAR(MAX)
		,@cdc_predeployment_ScriptName				VARCHAR(50)		= 'cdc_predeployment.sql';


IF @cdc_predeployment_Environment IN('TEST', 'TESTFIX', 'PREPROD', 'PROD') --dont configure cdc in dev
BEGIN

	BEGIN TRY
		EXEC [DeploymentAdmin].ChangeControl.CDCShutdown 
			@DatabaseName		= @cdc_predeployment_DatabaseName
			,@TableNames		= @cdc_predeployment_TableNames		
			,@Reference			= @cdc_predeployment_NewVersionNumber
	END TRY
	BEGIN CATCH
		SELECT @cdc_predeployment_AuditMessage = ERROR_MESSAGE();

		EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
			@DatabaseName		= @cdc_predeployment_DatabaseName
			,@ServerName		= @cdc_predeployment_ServerName
			,@AuditMessage		= @cdc_predeployment_AuditMessage 
			,@ScriptName		= @cdc_predeployment_ScriptName
			,@AuditType			= 'Error';

		EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'

		EXEC [DeploymentAdmin].[ChangeControl].[DeploymentFailure]
			@DatabaseName	= @cdc_predeployment_DatabaseName
			,@ScriptName	= @cdc_predeployment_ScriptName;				

		--failure of this script should prevent further predeployment scripts from being run (treat as a showstopper)
		SET NOEXEC ON;
	END CATCH
END
ELSE
BEGIN
	--check that CDC schema has been deployed to target database, which is not the case until after post-deploy for a new database
	IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID('dbo.CDCLockedTables'))  
		RAISERROR('dbo.CDCLockedTables not found - skipping',10,1) WITH NOWAIT;

	IF @cdc_predeployment_Environment NOT IN('TEST', 'TESTFIX', 'PREPROD', 'PROD') --dont configure cdc in dev
		RAISERROR('%s environment detected - skipping',10,1,@cdc_predeployment_Environment) WITH NOWAIT;
END
--[cdc_predeployment.sql|2.0]