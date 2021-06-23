IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'adms' )
BEGIN
	EXEC('CREATE SCHEMA [adms] AUTHORIZATION [dbo]');
END
GO

DROP PROCEDURE IF EXISTS [adms].[ApprenticeApplication_RegistrationQualification_GetByRegistrationId]
GO

CREATE  PROCEDURE [adms].[ApprenticeApplication_RegistrationQualification_GetByRegistrationId]
	@RegistrationID integer
AS

SET NOCOUNT ON 

SELECT
	r.RegistrationId
	,r.StartDate
	,r.CurrentActualEndDate
	,tc.ApplicationId
	,tc.NTISQualificationCode as "QualificationCode"
	,r.CurrentEndReasonCode

		
FROM TrainingContract tc WITH (NOLOCK)
	INNER JOIN Registration r WITH (NOLOCK)
		ON r.RegistrationId = tc.TYIMSRegistrationId
WHERE	        
	tc.TYIMSRegistrationID = @RegistrationID 
	AND tc.CurrentApplicationStatusCode = 'APPD'
ORDER BY tc.StartDate DESC