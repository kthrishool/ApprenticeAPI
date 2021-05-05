--[PostDeployment_Release_Validation.sql|2.0]
RAISERROR('STARTING PostDeployment_Release_Validation.sql...',10,1) WITH NOWAIT;
SET NOCOUNT ON;

DECLARE @PostValidation_SuccessRegression		BIT
		,@PostValidation_SuccessTest			BIT
		,@PostValidation_ValidationResult		VARCHAR(100)
		,@PostValidation_DatabaseName			SYSNAME			= DB_NAME()
		,@PostValidation_NewVersionNumber		VARCHAR(21)		= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'NewVersionNumber')	
		,@PostValidation_ErrorHeaderString		NVARCHAR(50)	= [DeploymentAdmin].[ChangeControl].[GetOutputHeader]('red')
		,@PostValidation_SuccessHeaderString	NVARCHAR(50)	= [DeploymentAdmin].[ChangeControl].[GetOutputHeader]('green')
		,@PostValidation_ServerName				NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@PostValidation_AuditMessage			VARCHAR(MAX)
		,@PostValidation_ScriptName				VARCHAR(50)		= 'PostDeployment_Release_Validation.sql';


BEGIN TRY
	EXEC [DeploymentAdmin].[ChangeControl].[RunDeploymentTests]
		@TestGroupName	= @PostValidation_NewVersionNumber
		,@DatabaseName	= @PostValidation_DatabaseName
		,@Reference		= @PostValidation_NewVersionNumber
		,@Success		= @PostValidation_SuccessTest	OUTPUT 
		,@ScriptName	= @PostValidation_ScriptName;

	EXEC [DeploymentAdmin].[ChangeControl].[RunDeploymentTests]
		@TestGroupName	= 'Regression Tests'
		,@DatabaseName	= @PostValidation_DatabaseName
		,@Reference		= @PostValidation_NewVersionNumber
		,@Success		= @PostValidation_SuccessRegression	OUTPUT 
		,@ScriptName	= @PostValidation_ScriptName;
END TRY
BEGIN CATCH
	SELECT @PostValidation_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @PostValidation_DatabaseName
		,@ServerName		= @PostValidation_ServerName
		,@AuditMessage		= @PostValidation_AuditMessage 
		,@ScriptName		= @PostValidation_ScriptName
		,@AuditType			= 'Error';

	EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'
	--allow below logic to manage a failure
END CATCH


IF COALESCE(@PostValidation_SuccessRegression,0) = 1 AND COALESCE(@PostValidation_SuccessTest,0) = 1
BEGIN
	SELECT 	@PostValidation_ValidationResult	= 'PostDeployment Validations Result: SUCCESS'

	RAISERROR('%s%s',10,1,@PostValidation_SuccessHeaderString,@PostValidation_ValidationResult) WITH NOWAIT;
END
ELSE
BEGIN
	SELECT 	@PostValidation_ValidationResult	= 'PostDeployment Validations Result: FAILED'

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @PostValidation_DatabaseName
		,@ServerName		= @PostValidation_ServerName
		,@AuditMessage		= @PostValidation_ValidationResult 
		,@ScriptName		= @PostValidation_ScriptName
		,@AuditType			= 'Error';

	RAISERROR('%s%s',10,1,@PostValidation_ErrorHeaderString,@PostValidation_ValidationResult) WITH NOWAIT;

	EXEC [DeploymentAdmin].[ChangeControl].[DeploymentFailure]
		@DatabaseName	= @PostValidation_DatabaseName
		,@ScriptName	= @PostValidation_ScriptName;

	--failure of this script should prevent further predeployment scripts from being run (treat as a showstopper)
	SET NOEXEC ON;
END
--[PostDeployment_Release_Validation.sql|2.0]