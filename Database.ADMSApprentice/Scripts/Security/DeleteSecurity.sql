--[DeleteSecurity.sql|2.0]
GO
RAISERROR('STARTING DeleteSecurity.sql...',10,1) WITH NOWAIT;
SET NOCOUNT ON;

DECLARE @DeleteSecurity_DatabaseName		VARCHAR(255)	= DB_NAME()
		,@DeleteSecurity_ServerName			NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@DeleteSecurity_AuditMessage		VARCHAR(MAX)
		,@DeleteSecurity_ScriptName			VARCHAR(50)		= 'DeleteSecurity.sql';


BEGIN TRY
	EXEC [DeploymentAdmin].[ChangeControl].[DeleteSecurity]
		@DatabaseName	= @DeleteSecurity_DatabaseName;	
		
END TRY
BEGIN CATCH
	SELECT @DeleteSecurity_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @DeleteSecurity_DatabaseName
		,@ServerName		= @DeleteSecurity_ServerName
		,@AuditMessage		= @DeleteSecurity_AuditMessage 
		,@ScriptName		= @DeleteSecurity_ScriptName
		,@AuditType			= 'Error';

	EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'
	--failure of this script should NOT abort the deployment (not a showstopper)
END CATCH
GO
--[DeleteSecurity.sql|2.0]