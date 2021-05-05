--[ChangeControl-ValidationTest_SeedData.sql|2.0]
GO
RAISERROR('STARTING ChangeControl-ValidationTest_SeedData.sql...',10,1) WITH NOWAIT;
SET NOCOUNT ON;

DECLARE @ValidationTestSeedData_Environment					VARCHAR(50)		= [DeploymentAdmin].ChangeControl.GetEnvironment()
		,@ValidationTestSeedData_DatabaseName				VARCHAR(255)	= DB_NAME()
		,@ValidationTestSeedData_ExpectedVersionNumber		VARCHAR(21)		= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'ExpectedVersionNumber')
		,@ValidationTestSeedData_NewVersionNumber			VARCHAR(MAX)	= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'NewVersionNumber')
		,@ValidationTestSeedData_NewSnapshotName			VARCHAR(128)
		,@ValidationTestSeedData_DeploymentAdminVersion_SQL	NVARCHAR(MAX)
		,@ValidationTestSeedData_SnapshotFreeSpace_SQL		NVARCHAR(MAX)
		,@ValidationTestSeedData_CheckDatabaseVersion_SQL	NVARCHAR(MAX)
		,@ValidationTestSeedData_ServerName					NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@ValidationTestSeedData_AuditMessage				VARCHAR(MAX)
		,@ValidationTestSeedData_ScriptName					VARCHAR(50)		= 'ChangeControl-ValidationTest_SeedData.sql';


IF OBJECT_ID('tempdb..#DeploymentTest') IS NOT NULL
	DROP TABLE #DeploymentTest;

CREATE TABLE #DeploymentTest 
(
    TestGroupName		VARCHAR(200)	NOT NULL
    ,TestName			VARCHAR(200)	NULL
    ,TestType			VARCHAR(100)	NOT NULL
    ,Query				NVARCHAR(MAX)	NOT NULL
    ,ExpectedResult		NVARCHAR(MAX)	NULL
	,AbortOnFailure		BIT DEFAULT		(0)
	,Author				VARCHAR(50)		NULL
);

SET @ValidationTestSeedData_Environment = 
	CASE 
		WHEN @ValidationTestSeedData_Environment LIKE '%DEV%' THEN 'DEV'
		WHEN @ValidationTestSeedData_Environment LIKE '%TEST%' THEN 'TEST'
		ELSE @ValidationTestSeedData_Environment 
	END

SELECT	@ValidationTestSeedData_NewSnapshotName							= COALESCE([DeploymentAdmin].ChangeControl.GenerateSnapshotName(@ValidationTestSeedData_DatabaseName,@ValidationTestSeedData_NewVersionNumber) COLLATE LATIN1_GENERAL_CI_AS,'')
		,@ValidationTestSeedData_DeploymentAdminVersion_SQL				=
'
DECLARE	@DatabaseName			NVARCHAR(128)	= ''DeploymentAdmin''
		,@ExpectedVersion		VARCHAR(21)		= ''2.2''
		,@CurrentVersion		VARCHAR(21)		
		,@CurrentVersionMajor	tinyint
		,@CurrentVersionMinor	tinyint
		,@ExpectedVersionMajor	tinyint
		,@ExpectedVersionMinor	tinyint

EXEC DeploymentAdmin.ChangeControl.GetVersion
	@DatabaseName							= @DatabaseName
	,@Version								= @CurrentVersion			OUTPUT

SELECT	@CurrentVersionMajor				= DeploymentAdmin.ChangeControl.GetVersionMajor(@CurrentVersion)
		,@CurrentVersionMinor				= DeploymentAdmin.ChangeControl.GetVersionMinor(@CurrentVersion)
		,@ExpectedVersionMajor				= DeploymentAdmin.ChangeControl.GetVersionMajor(@ExpectedVersion)
		,@ExpectedVersionMinor				= DeploymentAdmin.ChangeControl.GetVersionMinor(@ExpectedVersion)

SELECT	CASE WHEN @CurrentVersionMajor		>=	@ExpectedVersionMajor 
				AND (@CurrentVersionMinor	>=	@ExpectedVersionMinor 
					OR @CurrentVersionMajor >	@ExpectedVersionMajor ) THEN 1 ELSE 0 END
'
		,@ValidationTestSeedData_SnapshotFreeSpace_SQL					=	
'
DECLARE @default_snapshot_path		VARCHAR(128)	= [DeploymentAdmin].ChangeControl.GetDefaultSnapshotPath()
		,@DatabaseName				NVARCHAR(128)	= ''' + @ValidationTestSeedData_DatabaseName + '''
		,@NewVersionNumber			VARCHAR(21)		= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(''' + @ValidationTestSeedData_DatabaseName + ''',''NewVersionNumber'')
		,@NewSnapshotName			NVARCHAR(128) 

SELECT	@NewSnapshotName	= [DeploymentAdmin].ChangeControl.GenerateSnapshotName(@DatabaseName,@NewVersionNumber)

;WITH fileDetails AS
(
SELECT		[DeploymentAdmin].Staging.AvailableFreeSpaceBytes(p.FilePath) / 1024 / 1024 /1024							AS freeGB
FROM		[DeploymentAdmin].ChangeControl.GetNewSnapshotFiles(@DatabaseName,@default_snapshot_path,@NewSnapshotName) p
GROUP BY	FilePath	
)
,checkFreeSpace AS
(
SELECT		CASE WHEN freeGB >= 2 THEN 1 ELSE 0 END														AS freeSpaceResult
FROM		fileDetails																								
)
SELECT		MIN(freeSpaceResult)    
FROM		checkFreeSpace
'
		,@ValidationTestSeedData_CheckDatabaseVersion_SQL				=
'
declare @checkversion_rc							int
		,@checkversion_ExpectedVersionNumber		VARCHAR(21)		= ''' + @ValidationTestSeedData_ExpectedVersionNumber		+ '''
		,@checkversion_NewVersionNumber				VARCHAR(21)		= ''' + @ValidationTestSeedData_NewVersionNumber			+ '''
		,@DatabaseName								NVARCHAR(128)	= ''' + @ValidationTestSeedData_DatabaseName + '''


IF OBJECT_ID(''[' + @ValidationTestSeedData_DatabaseName + '].ChangeControl.VersionHistory'') IS NOT NULL --skip for initial deployment because change control schema hasn''t been deployed to target database yet
BEGIN
	BEGIN TRY
		EXEC [DeploymentAdmin].ChangeControl.IsVersionUpgradeValid 
			@DatabaseName			= @DatabaseName
			,@ExpectedVersionNumber	= @checkversion_ExpectedVersionNumber
			,@NewVersionNumber		= @checkversion_NewVersionNumber
			,@ReturnCode			= @checkversion_rc	OUTPUT;
	END TRY
	BEGIN CATCH
	END CATCH

	IF @checkversion_rc IN (2,3) --ignore redeployment scenarios 
	BEGIN
		SET @checkversion_rc = 1;
	END

END
ELSE
BEGIN
	SET @checkversion_rc = 1; --effectivly ignore the return value, because change control schema hasn''t been deployed to target database yet
END

SELECT COALESCE(@checkversion_rc,0)'





;WITH VALIDATION_TESTS
(				NewVersionNumber	,EnvironmentName		,TestGroupName										,TestName													,Author										,AbortOnFailure		,TestType	,ExpectedResult								,Query)
AS	 																																									
(	 																																									
		SELECT	NULL				,'ALL'					,'PreDeployment Validations'						,'Check Enviornment can be determined (predeploy)'			,NULL										,1					,'Match'	,'1'										,'SELECT CASE WHEN COALESCE(DeploymentAdmin.ChangeControl.GetEnvironment() COLLATE LATIN1_GENERAL_CI_AS,'''') <> '''' THEN 1 ELSE 0 END'
UNION	SELECT	NULL				,'DEV,TEST'				,'PreDeployment Validations'						,'TFS Deployment Service Account sysadmin membership'		,NULL										,1					,'Exists'	,NULL										,'SELECT 1 FROM sys.server_role_members m INNER JOIN sys.server_principals r ON r.principal_id = m.role_principal_id INNER JOIN sys.server_principals p ON p.principal_id = m.member_principal_id WHERE r.name = ''sysadmin'' AND p.name = ''ENETDEV\Service_ESGDBDploy_d'''
UNION	SELECT	NULL				,'PREPROD'				,'PreDeployment Validations'						,'TFS Deployment Service Account sysadmin membership'		,NULL										,1					,'Exists'	,NULL										,'SELECT 1 FROM sys.server_role_members m INNER JOIN sys.server_principals r ON r.principal_id = m.role_principal_id INNER JOIN sys.server_principals p ON p.principal_id = m.member_principal_id WHERE r.name = ''sysadmin'' AND p.name = ''ENETPPROD\Service_ESGDBDploy_p'''
UNION	SELECT	NULL				,'PROD'					,'PreDeployment Validations'						,'TFS Deployment Service Account sysadmin membership'		,NULL										,1					,'Exists'	,NULL										,'SELECT 1 FROM sys.server_role_members m INNER JOIN sys.server_principals r ON r.principal_id = m.role_principal_id INNER JOIN sys.server_principals p ON p.principal_id = m.member_principal_id WHERE r.name = ''sysadmin'' AND p.name = ''Enetapp\service_ESGDBDploy'''
UNION	SELECT	NULL				,'ALL'					,'PreDeployment Validations'						,'DeploymentAdmin database exists'							,NULL										,1					,'Exists'	,NULL										,'SELECT TOP 1 1 FROM [sys].[databases] WITH(NOLOCK) WHERE name = ''DeploymentAdmin'' AND state_desc = ''ONLINE'' AND is_read_only = 0  AND user_access_desc IN (''MULTI_USER'',''RESTRICTED_USER'')'
UNION	SELECT	NULL				,'TEST,PREPROD,PROD'	,'PreDeployment Validations'						,'CDCAdmin database exists'									,NULL										,1					,'Exists'	,NULL										,'SELECT TOP 1 1 FROM [sys].[databases] WITH(NOLOCK) WHERE name = ''CDCAdmin'' AND state_desc = ''ONLINE'' AND is_read_only = 0  AND user_access_desc IN (''MULTI_USER'',''RESTRICTED_USER'')'
UNION	SELECT	NULL				,'ALL'					,'PreDeployment Validations'						,'DeploymentAdmin minimum expected version'					,NULL										,1					,'Match'	,'1'										,@ValidationTestSeedData_DeploymentAdminVersion_SQL
UNION	SELECT	NULL				,'ALL'					,'PreDeployment Validations'						,'Deployment Snapshot Backup disk free space sufficient'	,NULL										,0					,'Match'	,'1'										,@ValidationTestSeedData_SnapshotFreeSpace_SQL
UNION	SELECT	NULL				,'ALL'					,'PreDeployment Validations'						,'Check Database Version'									,NULL										,1					,'Match'	,'1'										,@ValidationTestSeedData_CheckDatabaseVersion_SQL
UNION	SELECT	NULL				,'ALL'					,'Regression Tests'									,'Check Enviornment can be determined (postdeploy)'			,NULL										,1					,'Match'	,'1'										,'SELECT CASE WHEN COALESCE(DeploymentAdmin.ChangeControl.GetEnvironment() COLLATE LATIN1_GENERAL_CI_AS,'''') <> '''' THEN 1 ELSE 0 END'
UNION	SELECT	NULL				,'ALL'					,'Regression Tests'									,'Database Collation Latin1_General_CI_AS'					,NULL										,0					,'Match'	,'SQL_Latin1_General_CP1_CI_AS'				,'SELECT collation_name FROM sys.databases WHERE name = ''' + @ValidationTestSeedData_DatabaseName + ''''
UNION	SELECT	NULL				,'ALL'					,@ValidationTestSeedData_NewVersionNumber			,'ChangeControl.VersionHistory'								,NULL										,0					,'Match'	,@ValidationTestSeedData_NewVersionNumber	,'DECLARE @Version VARCHAR(21); EXEC DeploymentAdmin.ChangeControl.GetVersion @DatabaseName = ''' + @ValidationTestSeedData_DatabaseName + ''',@Version = @Version OUTPUT;SELECT @Version;'
)
,FILTER_ENVIRONMENT AS
(
SELECT	DISTINCT	v.TestGroupName
					,v.TestName	
					,v.Query	
					,v.ExpectedResult	
					,v.TestType
					,COALESCE(v.AbortOnFailure,0)																						AS AbortOnFailure
					,v.Author
FROM				VALIDATION_TESTS																									v
CROSS APPLY			[DeploymentAdmin].ChangeControl.UDF_SplitStringToTable_varcharmax(v.EnvironmentName,',')							e
WHERE				e.Element	IN ( @ValidationTestSeedData_Environment, 'ALL'	)
AND					COALESCE(v.NewVersionNumber,@ValidationTestSeedData_NewVersionNumber) = @ValidationTestSeedData_NewVersionNumber
)
INSERT INTO #DeploymentTest 
(
		[TestGroupName]    
		,[TestName]         
		,[TestType]         
		,[Query]            
		,[ExpectedResult]   
		,AbortOnFailure
		,Author
)
SELECT	TestGroupName 
		,TestName 
		,TestType 
		,Query 
		,ExpectedResult 
		,AbortOnFailure
		,Author
FROM	FILTER_ENVIRONMENT;


BEGIN TRY
	EXEC [DeploymentAdmin].[ChangeControl].[UpsertDeploymentTest]
		@DatabaseName	= @ValidationTestSeedData_DatabaseName	
		,@Reference		= @ValidationTestSeedData_NewVersionNumber;
END TRY
BEGIN CATCH
	SELECT @ValidationTestSeedData_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @ValidationTestSeedData_DatabaseName
		,@ServerName		= @ValidationTestSeedData_ServerName
		,@AuditMessage		= @ValidationTestSeedData_AuditMessage 
		,@ScriptName		= @ValidationTestSeedData_ScriptName
		,@AuditType			= 'Error';


	EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'

	EXEC [DeploymentAdmin].[ChangeControl].[DeploymentFailure]
		@DatabaseName	= @ValidationTestSeedData_DatabaseName
		,@ScriptName	= @ValidationTestSeedData_ScriptName;

	--failure of this script should prevent further predeployment scripts from being run (treat as a showstopper)
	SET NOEXEC ON;
END CATCH
--[ChangeControl-ValidationTest_SeedData.sql|2.0]