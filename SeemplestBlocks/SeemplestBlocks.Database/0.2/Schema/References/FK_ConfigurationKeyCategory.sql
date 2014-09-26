ALTER TABLE [Config].[ConfigurationKey] WITH CHECK
ADD CONSTRAINT [FK_ConfigurationKeyCategory]
FOREIGN KEY ([CategoryId])
REFERENCES [Config].[ConfigurationCategory] ([Id])
