--[ApplyVersionNumber.sql|2.0]
RAISERROR('STARTING ApplyVersionNumber.sql...',10,1) WITH NOWAIT;

DECLARE @upgradeversion_NewVersionNumber	VARCHAR(21)		= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'NewVersionNumber')
		,@upgradeversion_DatabaseName		SYSNAME			= DB_NAME()
		,@upgradeversion_ServerName			NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@upgradeversion_AuditMessage		VARCHAR(MAX)
		,@upgradeversion_ScriptName			VARCHAR(50)		= 'ApplyVersionNumber.sql';

		
BEGIN TRY
	RAISERROR('UPGRADING DATABASE VERSION',10,1) WITH NOWAIT;

	EXEC [DeploymentAdmin].ChangeControl.ApplyVersionNumber 
		@DatabaseName	= @upgradeversion_DatabaseName
		,@VersionNumber	= @upgradeversion_NewVersionNumber
		,@Reference		= @upgradeversion_NewVersionNumber;
END TRY
BEGIN CATCH
	SELECT @upgradeversion_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @upgradeversion_DatabaseName
		,@ServerName		= @upgradeversion_ServerName
		,@AuditMessage		= @upgradeversion_AuditMessage 
		,@ScriptName		= @upgradeversion_ScriptName
		,@AuditType			= 'Error';

	EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'
	--failure of this script should NOT abort the deployment (not a showstopper)
END CATCH
--[ApplyVersionNumber.sql|2.0]