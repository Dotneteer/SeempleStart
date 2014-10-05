ALTER TABLE [Platform].[SubscriptionParameterValue] WITH CHECK
ADD CONSTRAINT [FK_ServiceOfParameterValue]
FOREIGN KEY ([ServiceCode], [ParameterCode])
REFERENCES [Platform].[ServiceParameter] ([ServiceCode], [ParameterCode])
