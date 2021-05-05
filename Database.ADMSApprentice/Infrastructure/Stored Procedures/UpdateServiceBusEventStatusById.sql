/********************************************************************************************

Object:         [Infrastructure].[UpdateServiceBusEventStatusById]

Description:    Matches and Updates all ServiceBusEvents to a given status in 2nd column of 
           @IdToStatus, Id values to use are found in the first column of @IdToStatus.


Usage:          DECLARE @Input AS IntValues 
           INSERT INTO @Input (1,3)
           INSERT INTO @Input (2,3)
           EXEC usp_InsertProductionLocation @IdToStatus=@Input                  

Output:         None (Default Rowcount)

Author:         NATION\sp3336

Created:   2019-12-17

Filename:           $Workfile: $
SourceSafe version: $Revision: $
Last Changed By:    $Author:   $
Last modification:  $Modtime:  $
Last check in:      $Date:     $

Modification History:
USERID     Date            Description
======     ==========      ====================================================


********************************************************************************************/
CREATE PROCEDURE [Infrastructure].[UpdateServiceBusEventStatusById]

@IdToStatus IdentityAndStatus READONLY

AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION

	UPDATE sbe SET	[sbe].[Status] = [its].[Status],
					[sbe].LastUpdated = getdate()
                FROM [Infrastructure].[ServiceBusEvent] sbe 
                                INNER JOIN @IdToStatus its ON [its].[ID] = [sbe].[Id]
	
	END TRY

/*-------------------- ERROR HANDLING -------------------------------------*/

	--  DO NOT MODIFY THIS SECTION
	BEGIN CATCH
		IF XACT_STATE() = -1 OR @@TRANCOUNT = 1
		BEGIN
			ROLLBACK TRANSACTION
		END
		ELSE IF @@TRANCOUNT > 1 
		BEGIN
			COMMIT TRANSACTION
		END

/*-------------------- VARIABLE DECLARATION -------------------------------*/

		DECLARE @ErrorMessage  nvarchar(1000),
				@DatabaseName  varchar(128),
				@ServerName    varchar(128),
				@ErrorLogId    int = 0,
				@ErrorSeverity int,
				@ErrorState    int
				      
		SELECT @DatabaseName = DB_NAME(), @ServerName = @@SERVERNAME;

		-- raise user defined error message the error again as it has been consumed by the catch block
		SELECT @ErrorSeverity = ERROR_SEVERITY(),
				@ErrorState    = ERROR_STATE(),
				@ErrorMessage  = ERROR_MESSAGE();
		RAISERROR(@ErrorMessage,@ErrorSeverity,@ErrorState);

		RETURN ERROR_NUMBER();
	END CATCH
/*-------------------- FINALISATION -------------------------------------*/
	-- Normal Exit Processing
	IF @@TRANCOUNT > 0 
		BEGIN
			COMMIT TRANSACTION
		END

-- Return 0 to indicate successful run of procedure
	RETURN(0);

 END
  /*-------------------- END PROCEDURE --------------------------------------*/
