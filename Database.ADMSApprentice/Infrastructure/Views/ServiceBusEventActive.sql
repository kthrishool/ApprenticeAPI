

CREATE VIEW [Infrastructure].[ServiceBusEventActive]
AS 

	SELECT   [Id]
			,[CorrelationId]
			,[MessageId]
			,[EventType]
			,[Message]
			,[Status]
			,[Created]
			,[LastUpdated]
			,[CustomHeaders]
			,[ChainId]
			,[ParentChainId]
	FROM  [Infrastructure].[ServiceBusEvent]
	WHERE ([PublishAfter] IS NULL OR [PublishAfter] <= GETDATE())
		 AND ([Status] = 0)