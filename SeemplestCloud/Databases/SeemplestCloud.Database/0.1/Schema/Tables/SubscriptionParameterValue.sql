CREATE TABLE [Platform].[SubscriptionParameterValue]
(
	[SubscriptionId] int NOT NULL,
	[ServiceCode] varchar(32) NOT NULL,
	[ParameterCode] varchar(32) NOT NULL,
	[Value] nvarchar(1024) NOT NULL
    CONSTRAINT [PK_SubscriptionParameterValue] PRIMARY KEY ([SubscriptionId], [ServiceCode], [ParameterCode])
)
