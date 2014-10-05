ALTER TABLE [Platform].[UserRole] WITH CHECK
ADD CONSTRAINT [FK_ServiceOfRole]
FOREIGN KEY ([ServiceCode], [RoleCode])
REFERENCES [Platform].[ServiceRole] ([ServiceCode], [RoleCode])
