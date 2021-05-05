--[AuditSecurity.sql|2.0]
RAISERROR('STARTING AuditSecurity.sql...',10,1) WITH NOWAIT;
DECLARE @AuditSecurity_DatabaseName					VARCHAR(255)	= DB_NAME()
		,@AuditSecurity_NewVersionNumber			VARCHAR(21)		= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'NewVersionNumber')
		,@AuditSecurity_ServerName					NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@AuditSecurity_AuditMessage				VARCHAR(MAX)
		,@AuditSecurity_ScriptName					VARCHAR(50)		= 'AuditSecurity.sql';

BEGIN TRY
	EXEC [DeploymentAdmin].[ChangeControl].[AuditSecurity]
		@DatabaseName	= @AuditSecurity_DatabaseName
		,@Reference		= @AuditSecurity_NewVersionNumber;
END TRY
BEGIN CATCH
	SELECT @AuditSecurity_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @AuditSecurity_DatabaseName
		,@ServerName		= @AuditSecurity_ServerName
		,@AuditMessage		= @AuditSecurity_AuditMessage 
		,@ScriptName		= @AuditSecurity_ScriptName
		,@AuditType			= 'Error';

	EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'
	--failure of this script should NOT abort the deployment (not a showstopper)
END CATCH
--[AuditSecurity.sql|2.0]