ALTER TABLE [Platform].[ServiceDefinition] WITH CHECK
ADD CONSTRAINT [FK_DefaultUserRole]
FOREIGN KEY ([Code], [DefaultUserRole])
REFERENCES [Platform].[ServiceRole] ([ServiceCode], [RoleCode])
