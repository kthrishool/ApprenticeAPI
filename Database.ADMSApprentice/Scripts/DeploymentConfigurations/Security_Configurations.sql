--[Security_Configurations.sql|2.0]
GO
RAISERROR('STARTING Security_Configurations.sql...',10,1) WITH NOWAIT;

DECLARE @Security_Configurations_Environment		VARCHAR(50)		= [DeploymentAdmin].ChangeControl.GetEnvironment()
		,@Security_Configurations_DatabaseName		VARCHAR(255)	= DB_NAME()
		,@Security_Configurations_NewVersionNumber	VARCHAR(21)		= [DeploymentAdmin].ChangeControl.GetDeploymentConfiguration(DB_NAME(),'NewVersionNumber')	
		,@Security_Configurations_ServerName		NVARCHAR(128)	= CAST(SERVERPROPERTY('SERVERNAME') AS VARCHAR(128))
		,@Security_Configurations_AuditMessage		VARCHAR(MAX)
		,@Security_Configurations_ScriptName		VARCHAR(50)		= 'Security_Configurations.sql';


SET @Security_Configurations_Environment = 
								CASE 
									WHEN @Security_Configurations_Environment LIKE '%DEV%' THEN 'DEV'
									WHEN @Security_Configurations_Environment LIKE '%TEST%' THEN 'TEST'
									ELSE @Security_Configurations_Environment 
								END

IF OBJECT_ID('tempdb..#Roles') IS NOT NULL
	DROP TABLE #Roles;

CREATE TABLE #Roles
(
	RowId					INT IDENTITY (1,1)
	,RoleName				SYSNAME
	,UNIQUE NONCLUSTERED (RoleName) 
)


IF OBJECT_ID('tempdb..#RoleMembership') IS NOT NULL
	DROP TABLE #RoleMembership;

CREATE TABLE #RoleMembership
(
	RowId					INT IDENTITY (1,1)
	,AcountName				SYSNAME
	,RoleName				SYSNAME
	,DatabaseUserName		SYSNAME
	,ReservedLogin			BIT
	,UNIQUE NONCLUSTERED (AcountName, RoleName) 
)

IF OBJECT_ID('tempdb..#Permissions') IS NOT NULL
	DROP TABLE #Permissions;

CREATE TABLE #Permissions
(
	RowId					INT IDENTITY (1,1)
	,GrantOrDeny			SYSNAME
	,ServerLoginName		SYSNAME
	,SchemaName				SYSNAME
	,ObjectName				SYSNAME
	,ObjectClass			SYSNAME
	,PermissionName			SYSNAME
	,DatabaseUserName		SYSNAME
	,ReservedLogin			BIT
)	




;WITH IntendedRoles
(				EnvironmentName		,RoleName									)
AS
(
		SELECT	'ALL'				,N'_ApprenticeRole'
UNION	SELECT	'ALL'				,N'_BatchRole'
UNION	SELECT	'ALL'				,N'_BackgroundProcessRole'
UNION	SELECT	'ALL'				,N'_LegacyRole'
UNION	SELECT	'ALL'				,N'_SensitiveLoggingReader'
UNION	SELECT	'ALL'				,N'_ServiceBusRole'
)
INSERT INTO #Roles
( 
			RoleName
)
SELECT		r.RoleName
FROM		IntendedRoles																						r
CROSS APPLY	[DeploymentAdmin].ChangeControl.UDF_SplitStringToTable_varcharmax(r.EnvironmentName,',')			e
WHERE		e.Element	IN ( @Security_Configurations_Environment, 'ALL'	)
GROUP BY	r.RoleName


;WITH IntendedRoleMembership
(				EnvironmentName			,AcountName								,RoleName							)
AS
(
		SELECT		'ALL'				,N'NATION\Admin_SQLSVR_ESGRead'			,N'db_datareader'
UNION	SELECT		'DEV,TEST'			,N'NATION\MGT_DEV_ADMS_Developer'		,N'db_datareader'
UNION	SELECT		'DEV,TEST'			,N'NATION\MGT_DEV_ADMS_Tester'			,N'db_datareader'
UNION	SELECT		'DEV,TEST'			,N'NATION\MGT_DEV_ADMS_DBReader'		,N'db_datareader'
UNION	SELECT		'DEV,TEST'			,N'NATION\ADMIN_TYIMS'					,N'db_datareader'
UNION	SELECT		'DEV,TEST'			,N'NATION\MGT_Dev_TYIMS_Developers'		,N'db_datareader'
UNION	SELECT		'DEV,TEST'			,N'NATION\MGT_TYIMS_Testers'			,N'db_datareader'
UNION	SELECT		'DEV'				,N'NATION\MGT_DEV_ADMS_Developer'		,N'db_datawriter'

UNION	SELECT		'DEV,TEST'			,N'ENETDEV\Service_APIApprent_D'		,N'_ApprenticeRole'
UNION	SELECT		'DEV,TEST'			,N'ENETDEV\serviceautosys_d'			,N'_BatchRole'
UNION	SELECT		'DEV,TEST'			,N'ENETDEV\Service_ADMSBGProc_D'		,N'_BackgroundProcessRole'
UNION	SELECT		'DEV,TEST'			,N'ENETDEV\Service_ADMS_Claim_D'		,N'_LegacyRole'
UNION	SELECT		'DEV,TEST'			,N'ENETDEV\Service_TYIMS_D'				,N'_LegacyRole'
UNION	SELECT		'DEV,TEST'			,N'Enetdev\Service_TySAnon_d'			,N'_LegacyRole'
UNION	SELECT		'DEV,TEST'			,N'Enetdev\Service_TYSBatch_d'			,N'_LegacyRole'
UNION	SELECT		'DEV,TEST'			,N'enetdev\Service_TYSMts_D'			,N'_LegacyRole'
UNION	SELECT		'DEV,TEST'			,N'ENETDEV\service_syslog_sql_d'		,N'_SensitiveLoggingReader'

UNION	SELECT		'PREPROD'			,N'ENETPPROD\Service_APIApprent_P'		,N'_ApprenticeRole'
UNION	SELECT		'PREPROD'			,N'ENETPPROD\serviceautosys_P'			,N'_BatchRole'
UNION	SELECT		'PREPROD'			,N'ENETPPROD\Service_ADMSBGProc_P'		,N'_BackgroundProcessRole'
UNION	SELECT		'PREPROD'			,N'ENETPPROD\Service_ADMS_Claim_P'		,N'_LegacyRole'
UNION	SELECT		'PREPROD'			,N'ENETPPROD\Service_TYIMS_P'			,N'_LegacyRole'
UNION	SELECT		'PREPROD'			,N'ENETPPROD\Service_TySAnon_p'			,N'_LegacyRole'
--UNION	SELECT		'PREPROD'			,N'ENETPPROD\Service_TYSBatch_p'		,N'_LegacyRole'
UNION	SELECT		'PREPROD'			,N'ENETPPROD\Service_TYSMts_P'			,N'_LegacyRole'
UNION	SELECT		'PREPROD'			,N'ENETPPROD\service_syslog_sql_p'		,N'_SensitiveLoggingReader'

UNION	SELECT		'PROD'				,N'ENETAPP\Service_APIApprent'			,N'_ApprenticeRole'
UNION	SELECT		'PROD'				,N'ENETAPP\serviceautosys'				,N'_BatchRole'
UNION	SELECT		'PROD'				,N'ENETAPP\Service_ADMSBGProc'			,N'_BackgroundProcessRole'
UNION	SELECT		'PROD'				,N'ENETAPP\Service_ADMS_Claim'			,N'_LegacyRole'
UNION	SELECT		'PROD'				,N'ENETAPP\Service_TYIMS'				,N'_LegacyRole'
UNION	SELECT		'PROD'				,N'ENETAPP\Service_TySAnon'				,N'_LegacyRole'
--UNION	SELECT		'PROD'				,N'ENETAPP\Service_TYSBatch'			,N'_LegacyRole'
UNION	SELECT		'PROD'				,N'ENETAPP\Service_TYSMts'				,N'_LegacyRole'
UNION	SELECT		'PROD'				,N'ENETAPP\service_syslog_sql'			,N'_SensitiveLoggingReader'
)
INSERT INTO #RoleMembership 
(			AcountName				
			,RoleName
			,DatabaseUserName
			,ReservedLogin
)
SELECT		a.AcountName	
			,a.RoleName
			,COALESCE(d.name,a.AcountName) COLLATE Latin1_General_CI_AS
			,CASE WHEN MAX(COALESCE(ReservedLogins.name,'')) <> '' THEN 1 ELSE 0 END
FROM		IntendedRoleMembership																								a
LEFT JOIN	sys.server_principals																								s		--join to detrumine database user name as may differ from server login name
ON			s.name																= a.AcountName
LEFT JOIN	sys.database_principals																								d
ON			d.sid																= s.sid
LEFT JOIN	(
					SELECT	'cdc'
			UNION	SELECT	'dbo'
			UNION	SELECT	'guest'
			UNION	SELECT	'INFORMATION_SCHEMA'
			UNION	SELECT	'sys'
			)																													ReservedLogins (name)
ON			ReservedLogins.name													=  a.AcountName
CROSS APPLY	[DeploymentAdmin].ChangeControl.UDF_SplitStringToTable_varcharmax(a.EnvironmentName,',')							e
WHERE		e.Element															IN ( @Security_Configurations_Environment, 'ALL'	)
GROUP BY	a.AcountName
			,a.RoleName
			,COALESCE(d.name,a.AcountName) COLLATE Latin1_General_CI_AS


--note: database CONNECT permissions are granted by default by the proceeding script, hence do not need to be defined here
;WITH IntendedPermissions
(				EnvironmentName			,GrantOrDeny	,ServerLoginName						,SchemaName					,ObjectName												,ObjectType		,PermissionName							)
AS
(
		SELECT	'ALL'					,N'GRANT'		,N'NATION\Admin_SQLSVR_ESGRead'			,N''						,N''													,N'DATABASE'	,N'VIEW DEFINITION'
UNION	SELECT	'DEV,TEST'				,N'GRANT'		,N'NATION\MGT_DEV_ADMS_Developer'		,N''						,N''													,N'DATABASE'	,N'VIEW DEFINITION'
UNION	SELECT	'DEV,TEST'				,N'GRANT'		,N'NATION\MGT_DEV_ADMS_Tester'			,N''						,N''													,N'DATABASE'	,N'VIEW DEFINITION'
UNION	SELECT	'DEV,TEST'				,N'GRANT'		,N'NATION\MGT_DEV_ADMS_DBReader'		,N''						,N''													,N'DATABASE'	,N'VIEW DEFINITION'
UNION	SELECT	'DEV'					,N'GRANT'		,N'NATION\MGT_DEV_ADMS_Developer'		,N''						,N''													,N'DATABASE'	,N'EXECUTE'

UNION	SELECT	'ALL'					,N'GRANT'		,N'_ApprenticeRole'						,N'dbo'						,N''													,N'SCHEMA'		,N'SELECT'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_ApprenticeRole'						,N'dbo'						,N''													,N'SCHEMA'		,N'INSERT'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_ApprenticeRole'						,N'dbo'						,N''													,N'SCHEMA'		,N'UPDATE'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_ApprenticeRole'						,N'dbo'						,N''													,N'SCHEMA'		,N'DELETE'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_ApprenticeRole'						,N'dbo'						,N''													,N'SCHEMA'		,N'EXECUTE'

UNION	SELECT	'ALL'					,N'GRANT'		,N'_ApprenticeRole'						,N'Infrastructure'			,N''													,N'SCHEMA'		,N'SELECT'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_ApprenticeRole'						,N'Infrastructure'			,N''													,N'SCHEMA'		,N'INSERT'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_ApprenticeRole'						,N'Infrastructure'			,N''													,N'SCHEMA'		,N'UPDATE'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_ApprenticeRole'						,N'Infrastructure'			,N''													,N'SCHEMA'		,N'DELETE'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_ApprenticeRole'						,N'Infrastructure'			,N''													,N'SCHEMA'		,N'EXECUTE'

UNION	SELECT	'ALL'					,N'GRANT'		,N'_ApprenticeRole'						,N'SensitiveLogging'		,N'APIRequestLog'										,N'OBJECT'		,N'INSERT'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_ApprenticeRole'						,N'SensitiveLogging'		,N'RequestLog'											,N'OBJECT'		,N'INSERT'

UNION	SELECT	'ALL'					,N'GRANT'		,N'_LegacyRole'							,N'dbo'						,N''													,N'SCHEMA'		,N'SELECT'

UNION	SELECT	'ALL'					,N'GRANT'		,N'_BatchRole'							,N'dbo'						,N''													,N'SCHEMA'		,N'SELECT'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BatchRole'							,N'dbo'						,N''													,N'SCHEMA'		,N'INSERT'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BatchRole'							,N'dbo'						,N''													,N'SCHEMA'		,N'UPDATE'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BatchRole'							,N'dbo'						,N''													,N'SCHEMA'		,N'DELETE'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BatchRole'							,N'dbo'						,N''													,N'SCHEMA'		,N'EXECUTE'

UNION	SELECT	'ALL'					,N'GRANT'		,N'_BatchRole'							,N'Infrastructure'			,N''													,N'SCHEMA'		,N'SELECT'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BatchRole'							,N'Infrastructure'			,N''													,N'SCHEMA'		,N'INSERT'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BatchRole'							,N'Infrastructure'			,N''													,N'SCHEMA'		,N'UPDATE'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BatchRole'							,N'Infrastructure'			,N''													,N'SCHEMA'		,N'DELETE'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BatchRole'							,N'Infrastructure'			,N''													,N'SCHEMA'		,N'EXECUTE'

UNION	SELECT	'ALL'					,N'GRANT'		,N'_BatchRole'							,N'Batch'					,N''													,N'SCHEMA'		,N'SELECT'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BatchRole'							,N'Batch'					,N''													,N'SCHEMA'		,N'INSERT'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BatchRole'							,N'Batch'					,N''													,N'SCHEMA'		,N'UPDATE'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BatchRole'							,N'Batch'					,N''													,N'SCHEMA'		,N'DELETE'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BatchRole'							,N'Batch'					,N''													,N'SCHEMA'		,N'EXECUTE'

UNION	SELECT	'ALL'					,N'GRANT'		,N'_BackgroundProcessRole'				,N'dbo'						,N''													,N'SCHEMA'		,N'SELECT'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BackgroundProcessRole'				,N'dbo'						,N''													,N'SCHEMA'		,N'INSERT'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BackgroundProcessRole'				,N'dbo'						,N''													,N'SCHEMA'		,N'UPDATE'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BackgroundProcessRole'				,N'dbo'						,N''													,N'SCHEMA'		,N'DELETE'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BackgroundProcessRole'				,N'dbo'						,N''													,N'SCHEMA'		,N'EXECUTE'

UNION	SELECT	'ALL'					,N'GRANT'		,N'_BackgroundProcessRole'				,N'Infrastructure'			,N''													,N'SCHEMA'		,N'SELECT'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BackgroundProcessRole'				,N'Infrastructure'			,N''													,N'SCHEMA'		,N'INSERT'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BackgroundProcessRole'				,N'Infrastructure'			,N''													,N'SCHEMA'		,N'UPDATE'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BackgroundProcessRole'				,N'Infrastructure'			,N''													,N'SCHEMA'		,N'DELETE'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BackgroundProcessRole'				,N'Infrastructure'			,N''													,N'SCHEMA'		,N'EXECUTE'

UNION	SELECT	'ALL'					,N'GRANT'		,N'_BackgroundProcessRole'				,N'Batch'					,N''													,N'SCHEMA'		,N'SELECT'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BackgroundProcessRole'				,N'Batch'					,N''													,N'SCHEMA'		,N'INSERT'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BackgroundProcessRole'				,N'Batch'					,N''													,N'SCHEMA'		,N'UPDATE'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BackgroundProcessRole'				,N'Batch'					,N''													,N'SCHEMA'		,N'DELETE'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_BackgroundProcessRole'				,N'Batch'					,N''													,N'SCHEMA'		,N'EXECUTE'

UNION	SELECT	'ALL'					,N'GRANT'		,N'_ServiceBusRole'						,N'Infrastructure'			,N''													,N'SCHEMA'		,N'EXECUTE'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_ServiceBusRole'						,N'Infrastructure'			,N'ServiceBusEvent'										,N'OBJECT'		,N'SELECT'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_ServiceBusRole'						,N'Infrastructure'			,N'ServiceBusEvent'										,N'OBJECT'		,N'INSERT'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_ServiceBusRole'						,N'Infrastructure'			,N'ServiceBusEvent'										,N'OBJECT'		,N'UPDATE'
UNION	SELECT	'ALL'					,N'GRANT'		,N'_ServiceBusRole'						,N'Infrastructure'			,N'ServiceBusEvent'										,N'OBJECT'		,N'DELETE'

UNION	SELECT	'ALL'					,N'GRANT'		,N'_SensitiveLoggingReader'				,N'SensitiveLogging'		,N''													,N'SCHEMA'		,N'SELECT'

)
INSERT INTO #Permissions 
(						
			GrantOrDeny
			,ServerLoginName		
			,SchemaName			
			,ObjectName			
			,ObjectClass			
			,PermissionName	
			,DatabaseUserName
			,ReservedLogin
)
SELECT		a.GrantOrDeny
			,a.ServerLoginName		
			,a.SchemaName			
			,a.ObjectName			
			,a.ObjectType			
			,a.PermissionName	
			,COALESCE(d.name,a.ServerLoginName) COLLATE Latin1_General_CI_AS
			,CASE WHEN MAX(COALESCE(ReservedLogins.name,'')) <> '' THEN 1 ELSE 0 END
FROM		IntendedPermissions																											a
LEFT JOIN	sys.server_principals																										s		--join to detrumine database user name as may differ from server login name
ON			s.name																		= a.ServerLoginName
LEFT JOIN	sys.database_principals																										d
ON			d.sid																		= s.sid
LEFT JOIN	(
					SELECT	'cdc'
			UNION	SELECT	'dbo'
			UNION	SELECT	'guest'
			UNION	SELECT	'INFORMATION_SCHEMA'
			UNION	SELECT	'sys'
			)																															ReservedLogins (name)
ON			ReservedLogins.name															=  a.ServerLoginName
CROSS APPLY	[DeploymentAdmin].ChangeControl.UDF_SplitStringToTable_varcharmax(a.EnvironmentName,',')									e
WHERE		e.Element																	IN ( @Security_Configurations_Environment, 'ALL'	)
GROUP BY	a.GrantOrDeny
			,a.ServerLoginName		
			,a.SchemaName			
			,a.ObjectName			
			,a.ObjectType			
			,a.PermissionName	
			,COALESCE(d.name,a.ServerLoginName) COLLATE Latin1_General_CI_AS


BEGIN TRY
	EXEC [DeploymentAdmin].[ChangeControl].[UpsertSecurity]
		@DatabaseName	= @Security_Configurations_DatabaseName
		,@Reference		= @Security_Configurations_NewVersionNumber
		
END TRY
BEGIN CATCH
	SELECT @Security_Configurations_AuditMessage = ERROR_MESSAGE();

	EXEC [DeploymentAdmin].[ChangeControl].[InsertAuditLog]
		@DatabaseName		= @Security_Configurations_DatabaseName
		,@ServerName		= @Security_Configurations_ServerName
		,@AuditMessage		= @Security_Configurations_AuditMessage 
		,@ScriptName		= @Security_Configurations_ScriptName
		,@AuditType			= 'Error';


	EXEC SP_EXECUTESQL N'DECLARE @ERROR_MSG NVARCHAR(128) = [DeploymentAdmin].[ChangeControl].[GetOutputHeader](''red'') + ERROR_MESSAGE();RAISERROR(''%s'',10,1,@ERROR_MSG) WITH NOWAIT;'

	EXEC [DeploymentAdmin].[ChangeControl].[DeploymentFailure]
		@DatabaseName	= @Security_Configurations_DatabaseName
		,@ScriptName	= @Security_Configurations_ScriptName;

	--failure of this script should prevent further predeployment scripts from being run (treat as a showstopper)
	SET NOEXEC ON;
END CATCH
GO
--[Security_Configurations.sql|2.0]