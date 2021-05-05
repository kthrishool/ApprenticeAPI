--[SetDatabaseRestrictedUser.sql|2.0]
RAISERROR('STARTING SetDatabaseRestrictedUser.sql...',10,1) WITH NOWAIT;

DECLARE @SetDatabaseRestrictedUser_DatabaseName		SYSNAME			= DB_NAME()
		,@SetDatabaseRestrictedUser_ServerName		NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@SetDatabaseRestrictedUser_AuditMessage	VARCHAR(MAX)
		,@SetDatabaseRestrictedUser_ScriptName		VARCHAR(50)		= 'SetDatabaseRestrictedUser.sql';



BEGIN TRY
	EXEC [DeploymentAdmin].[ChangeControl].[SetDatabaseRestrictedUser]
		@DatabaseName	= @SetDatabaseRestrictedUser_DatabaseName
END TRY
BEGIN CATCH
	SELECT @SetDatabaseRestrictedUser_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @SetDatabaseRestrictedUser_DatabaseName
		,@ServerName		= @SetDatabaseRestrictedUser_ServerName
		,@AuditMessage		= @SetDatabaseRestrictedUser_AuditMessage 
		,@ScriptName		= @SetDatabaseRestrictedUser_ScriptName
		,@AuditType			= 'Error';

	EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'
	--failure of this script should NOT abort the deployment (not a showstopper)
END CATCH
--[SetDatabaseRestrictedUser.sql|2.0]