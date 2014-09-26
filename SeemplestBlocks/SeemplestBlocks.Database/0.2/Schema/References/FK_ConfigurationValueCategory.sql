ALTER TABLE [Config].[ConfigurationValue] WITH CHECK
ADD CONSTRAINT [FK_ConfigurationValueCategory]
FOREIGN KEY ([Category])
REFERENCES [Config].[ConfigurationCategory] ([Id])
