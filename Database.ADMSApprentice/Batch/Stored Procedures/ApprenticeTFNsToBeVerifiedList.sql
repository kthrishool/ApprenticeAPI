
/**************************************************************************
[Batch].[ApprenticeTFNsToBeVerifiedList]
Description:	Returns Apprentice TFN details that still need to be (re)submitted for verification
				TFNs will be returned where:
					StatusCode = 'TBVE'
					OR StatusCode = 'TERR'
					OR StatusCode = 'SBMT' AND was last submitted over @ResubmitHoursAfter (24) ago.

Usage:			[Batch].[ApprenticeTFNsToBeVerifiedList]
					@CurrentDateTime = NULL

Author:			JD3044
Created:		19/03/2021

Modification History:
UserId	Date		    Description
=======	==========		====================================================
PS1508	16/04/2021		Only select records that have a [MessageQueueCorrelationId] Not Null
***************************************************************************/
CREATE PROC [Batch].[ApprenticeTFNsToBeVerifiedList]
    @CurrentDateTime DATETIME2 = NULL,
	@ResubmitHoursAfter SMALLINT = 24
AS
	SET NOCOUNT ON;

	BEGIN TRY

		-- If no date is passed then default to now
		SELECT  @CurrentDateTime = ISNULL(@CurrentDateTime, GETDATE());
		DECLARE @CompareDate DATETIME2 = DATEADD(hh,-1*@ResubmitHoursAfter,@CurrentDateTime)

		SELECT   AT.[ApprenticeTFNId]
				,A.[ApprenticeId]
				,AT.[TaxFileNumber]
				,AT.[MessageQueueCorrelationId]
				,A.[FirstName]
				,A.[OtherNames]
				,A.[Surname]
				,A.[BirthDate]
				,AA.[StreetAddress1]
				,AA.[StreetAddress2]
				,AA.[Locality]
				,AA.[PostCode]
				,AA.[StateCode]
				,AT.[StatusCode]
				,AT.[Version]
		FROM [dbo].[Apprentice] A
		INNER JOIN [dbo].[ApprenticeTFN] AT
			ON A.[ApprenticeId] = AT.[ApprenticeId]
		LEFT JOIN [dbo].[ApprenticeAddress] AA
			ON A.[ApprenticeId] = AA.[ApprenticeId]
			AND AA.[AddressTypeCode] = 'RESD'
		WHERE AT.[MessageQueueCorrelationId] is NOT NULL 
			AND (AT.[StatusCode] = 'TBVE'
				OR AT.[StatusCode] = 'TERR'
				OR (AT.[StatusCode] = 'SBMT'
					AND AT.[StatusDate] <= @CompareDate)
					);

	END TRY

	BEGIN CATCH
		THROW;
	END CATCH

