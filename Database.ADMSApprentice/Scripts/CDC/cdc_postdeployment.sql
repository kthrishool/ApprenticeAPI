--[cdc_postdeployment.sql|2.0]
RAISERROR('STARTING cdc_postdeployment.sql...',10,1) WITH NOWAIT;


DECLARE @cdc_postdeployment_TableNames				NVARCHAR(MAX)	= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'CDCEnabledTables')
		,@cdc_postdeployment_NewVersionNumber		VARCHAR(21)		= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'NewVersionNumber')	
		,@cdc_postdeployment_Environment			VARCHAR(50)		= [DeploymentAdmin].ChangeControl.GetEnvironment()
		,@cdc_postdeployment_DatabaseName			SYSNAME			= DB_NAME()
		,@cdc_postdeployment_ServerName				NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@cdc_postdeployment_AuditMessage			VARCHAR(MAX)
		,@cdc_postdeployment_ScriptName				VARCHAR(50)		= 'cdc_postdeployment.sql';

IF @cdc_postdeployment_Environment IN('TEST', 'TESTFIX', 'PREPROD', 'PROD') --dont configure cdc in dev
BEGIN
	BEGIN TRY
		EXEC [DeploymentAdmin].ChangeControl.CDCStartUp 
			@DatabaseName		= @cdc_postdeployment_DatabaseName
			,@TableNames		= @cdc_postdeployment_TableNames
			,@Reference			= @cdc_postdeployment_NewVersionNumber
		
	END TRY
	BEGIN CATCH
		SELECT @cdc_postdeployment_AuditMessage = ERROR_MESSAGE();

		EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
			@DatabaseName		= @cdc_postdeployment_DatabaseName
			,@ServerName		= @cdc_postdeployment_ServerName
			,@AuditMessage		= @cdc_postdeployment_AuditMessage 
			,@ScriptName		= @cdc_postdeployment_ScriptName
			,@AuditType			= 'Error';
		
		EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'

		EXEC [DeploymentAdmin].[ChangeControl].[DeploymentFailure]
			@DatabaseName	= @cdc_postdeployment_DatabaseName
			,@ScriptName	= @cdc_postdeployment_ScriptName;				
		--failure of this script should prevent further predeployment scripts from being run (treat as a showstopper)
		SET NOEXEC ON;
	END CATCH

END
ELSE
	RAISERROR('skipping cdc configuration - %s environment detected...',10,1,@cdc_postdeployment_Environment) WITH NOWAIT;
GO
--[cdc_postdeployment.sql|2.0]