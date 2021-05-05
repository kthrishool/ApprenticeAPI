--[PreDeployment_Release_Validation.sql|2.0]
RAISERROR('STARTING PreDeployment_Release_Validation.sql...',10,1) WITH NOWAIT;
SET NOCOUNT ON;

DECLARE @PreValidation_Success					BIT
		,@PreValidation_ValidationResult		VARCHAR(100)
		,@PreValidation_DatabaseName			SYSNAME			= DB_NAME()
		,@PreValidation_ErrorHeaderString		NVARCHAR(50)	= [DeploymentAdmin].[ChangeControl].[GetOutputHeader]('red')
		,@PreValidation_SuccessHeaderString		NVARCHAR(50)	= [DeploymentAdmin].[ChangeControl].[GetOutputHeader]('green')
		,@PreValidation_NewVersionNumber		VARCHAR(21)		= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'NewVersionNumber')	
		,@PreValidation_ServerName				NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@PreValidation_AuditMessage			VARCHAR(MAX)
		,@PreValidation_ScriptName				VARCHAR(50)		= 'PreDeployment_Release_Validation.sql';


BEGIN TRY
	EXEC [DeploymentAdmin].[ChangeControl].[RunDeploymentTests]
		@TestGroupName	= 'PreDeployment Validations'
		,@DatabaseName	= @PreValidation_DatabaseName
		,@Reference		= @PreValidation_NewVersionNumber
		,@Success		= @PreValidation_Success	OUTPUT 
END TRY
BEGIN CATCH
	SELECT @PreValidation_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @PreValidation_DatabaseName
		,@ServerName		= @PreValidation_ServerName
		,@AuditMessage		= @PreValidation_AuditMessage 
		,@ScriptName		= @PreValidation_ScriptName
		,@AuditType			= 'Error';


	EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'
END CATCH


IF @PreValidation_Success = 1
BEGIN
	SELECT 	@PreValidation_ValidationResult	= 'PreDeployment Validations Result: SUCCESS'

	RAISERROR('%s%s',10,1,@PreValidation_SuccessHeaderString,@PreValidation_ValidationResult) WITH NOWAIT;
END
ELSE
BEGIN
	SELECT 	@PreValidation_ValidationResult	= 'PreDeployment Validations Result: FAILED'

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @PreValidation_DatabaseName
		,@ServerName		= @PreValidation_ServerName
		,@AuditMessage		= @PreValidation_ValidationResult 
		,@ScriptName		= @PreValidation_ScriptName
		,@AuditType			= 'Error';

			
	RAISERROR('%s%s',10,1,@PreValidation_ErrorHeaderString,@PreValidation_ValidationResult) WITH NOWAIT;		

	EXEC [DeploymentAdmin].[ChangeControl].[DeploymentFailure]
		@DatabaseName	= @PreValidation_DatabaseName
		,@ScriptName	= @PreValidation_ScriptName;

	--failure of this script should prevent further predeployment scripts from being run (treat as a showstopper)
	SET NOEXEC ON;
END
--[PreDeployment_Release_Validation.sql|2.0]