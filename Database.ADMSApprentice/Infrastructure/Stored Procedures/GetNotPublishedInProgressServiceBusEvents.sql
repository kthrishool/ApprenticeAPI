/********************************************************************************************

Object:         [Infrastructure].[GetNotPublishedInProgressServiceBusEvents]

Description:    Gets top 1000 service bus events ordered by Id where events are Not Published or In Progress


Usage:          EXEC [Infrastructure].[GetNotPublishedInProgressServiceBusEvents]
                     

Output:         Query result

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
JD3044        09/06/2020           Added index hint to use filtered index

********************************************************************************************/

CREATE PROCEDURE [Infrastructure].[GetNotPublishedInProgressServiceBusEvents]

AS
BEGIN
SELECT TOP (1000) 
                [Id]                         AS [Id],
                [CorrelationId] AS [CorrelationId],
                [MessageId]       AS [MessageId],
                [EventType]       AS [EventType],
                [Message]           AS [Message],
                [Status]                AS [Status],
                [Created]             AS [Created],
                [LastUpdated]   AS [LastUpdated],
                [CustomHeaders] AS [CustomHeaders],
                [ChainId]              AS [ChainId],
                [ParentChainId] AS [ParentChainId] 
FROM
                [Infrastructure].[ServiceBusEvent] WITH (INDEX = [FUIX_ServiceBusEvent_IdFiltered_status_filteredStatus])
WHERE           [Status] = 0 OR [Status] = 1

ORDER BY
                [Id] ASC
/*-------------------- FINALISATION -------------------------------------*/

  /* return 0 to indicate success, -1 for failure */
  IF @@ERROR<>0
    RETURN(-1)
  ELSE
    RETURN(0) 
END
/*-------------------- END PROCEDURE --------------------------------------*/
