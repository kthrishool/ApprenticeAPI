

/* =============================================
-- Author:		JL0758
-- Create date: 03/09/2018
-- Description:	Copies events with a status of 2 and a created date less than retention days 
--				from ServiceBusEvent to ServiceBusEventArchive table.
--				Then deletes all the applicable rows from the ServiceBusEvent table.

--  Parameters
--	@RetentionDays - days of data to retain - defaults to 7 days
--  @TerminateTime  - Last time to run so as not to interfere with online day
--	@BatchSize      - Records to delete in one hit so as to minimise blocking

BT2551	14/11/2019	Add ChainId and ParentChainId 
-- =============================================
*/
CREATE PROCEDURE [Batch].[InfrastructureServiceBusInsertIntoArchive]
    @RetentionDays INT = 7 ,
    @TerminateTime CHAR(5) = '05:00' ,
    @BatchSize INT = 10000
AS
    BEGIN

        DECLARE @RetentionDate DATETIME = DATEADD(DAY ,-@RetentionDays, GETDATE()) , --Today's date less the @RetentionDays
                @rows INT = 1;



        IF OBJECT_ID('tempdb..#TEMP_EVENTS') IS NOT NULL
            DROP TABLE #TEMP_EVENTS;

        CREATE TABLE #TEMP_EVENTS
            (
                [Id] [INT] NOT NULL ,
                [CorrelationId] [UNIQUEIDENTIFIER] NOT NULL ,
                [MessageId] [UNIQUEIDENTIFIER] NOT NULL ,
                [EventType] [NVARCHAR](50) NOT NULL ,
                [Message] [NVARCHAR](MAX) NOT NULL ,
                [Status] [INT] NOT NULL ,
                [Created] [DATETIME] NOT NULL ,
                [LastUpdated] [DATETIME] NOT NULL ,
                [CustomHeaders] [NVARCHAR](1024) NULL ,
				[ChainId]		[NVARCHAR] (50)    NULL,
				[ParentChainId] [NVARCHAR] (50)    NULL,
				[PublishAfter]  [DATETIME] NULL
            );


        BEGIN TRY
            WHILE @rows > 0 AND CAST(GETDATE() AS TIME) < @TerminateTime -- While there are rows and it is before the cut off time
                BEGIN

                    DELETE FROM #TEMP_EVENTS;

                    INSERT INTO #TEMP_EVENTS
                                SELECT TOP ( @BatchSize ) [Id] ,
                                       [CorrelationId] ,
                                       [MessageId] ,
                                       [EventType] ,
                                       [Message] ,
                                       [Status] ,
                                       [Created] ,
                                       [LastUpdated] ,
                                       [CustomHeaders],
									   [ChainId],
									   [ParentChainId],
									   [PublishAfter]
                                FROM   [Infrastructure].[ServiceBusEvent] WITH (NOLOCK)
                                WHERE  Status = 2 AND Created < @RetentionDate;

                    SELECT @rows = @@ROWCOUNT;

                    IF @rows > 0
                        BEGIN
                            BEGIN TRANSACTION;
                            INSERT INTO [Infrastructure].[ServiceBusEventArchive]
                                        SELECT *
                                        FROM   #TEMP_EVENTS;


                            RAISERROR('Deleting next (%d) batch of rows from [Infrastructure].[ServiceBusEvent]...' ,10 ,1 ,@rows) WITH NOWAIT;

                            DELETE x
                            FROM   [Infrastructure].[ServiceBusEvent] x
                                   INNER JOIN #TEMP_EVENTS t ON x.Id = t.Id;
                            COMMIT TRANSACTION;

                            WAITFOR DELAY '00:00:01';
                        END;
                END;

        END TRY

        /*-------------------- ERROR HANDLING -------------------------------------*/
        BEGIN CATCH
            IF XACT_STATE() = -1
               OR @@TRANCOUNT = 1
                BEGIN
                    ROLLBACK TRANSACTION;
                    PRINT ERROR_MESSAGE();
                END;
            ELSE IF @@TRANCOUNT > 1
                     BEGIN
                         COMMIT TRANSACTION;
                     END;
        END CATCH;

        /*-------------------- FINALISATION -------------------------------------*/
        DROP TABLE #TEMP_EVENTS;

        -- Normal Exit Processing
        IF @@TRANCOUNT > 0
            BEGIN
                COMMIT TRANSACTION;
            END;

        /* return 0 to indicate success, -1 for failure */
        IF @@ERROR <> 0
            RETURN ( -1 );
        ELSE
            RETURN ( 0 );



    END;