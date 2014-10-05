ALTER TABLE [Platform].[ServiceDefinition] WITH CHECK
ADD CONSTRAINT [FK_DefaultOwnerRole]
FOREIGN KEY ([Code], [DefaultOwnerRole])
REFERENCES [Platform].[ServiceRole] ([ServiceCode], [RoleCode])
