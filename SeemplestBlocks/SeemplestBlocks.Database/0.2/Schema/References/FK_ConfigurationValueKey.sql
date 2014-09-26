ALTER TABLE [Config].[ConfigurationValue] WITH CHECK
ADD CONSTRAINT [FK_ConfigurationValueKey]
FOREIGN KEY ([ConfigKey])
REFERENCES [Config].[ConfigurationKey] ([Id])
