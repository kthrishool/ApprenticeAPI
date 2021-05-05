--[DropSnapshot.sql|2.0]
RAISERROR('STARTING DropSnapshot.sql...',10,1) WITH NOWAIT;

DECLARE @DropSnapshot__NewVersionNumber		varchar(21)		= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'NewVersionNumber')
		,@DropSnapshot_DatabaseName			sysname			= DB_NAME()
		,@DropSnapshot_Snapshotname			VARCHAR(256)
		,@DropSnapshot_ServerName			NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@DropSnapshot_AuditMessage			VARCHAR(MAX)
		,@DropSnapshot_ScriptName			VARCHAR(50)		= 'DropSnapshot.sql';


SELECT @DropSnapshot_Snapshotname = [DeploymentAdmin].ChangeControl.GenerateSnapshotName(@DropSnapshot_DatabaseName,@DropSnapshot__NewVersionNumber)

BEGIN TRY
	EXEC [DeploymentAdmin].[ChangeControl].[DropSnapshot] 
		@DatabaseName		= @DropSnapshot_DatabaseName
		,@Snapshotname		= @DropSnapshot_Snapshotname;
END TRY
BEGIN CATCH
	SELECT @DropSnapshot_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @DropSnapshot_DatabaseName
		,@ServerName		= @DropSnapshot_ServerName
		,@AuditMessage		= @DropSnapshot_AuditMessage 
		,@ScriptName		= @DropSnapshot_ScriptName
		,@AuditType			= 'Error';

	EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'
	--failure of this script should NOT abort the deployment (not a showstopper)
END CATCH
--[DropSnapshot.sql|2.0]