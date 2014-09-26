delete from [Config].[CurrentConfigurationVersion]
delete from [Config].[ConfigurationValue]
delete from [Config].[ConfigurationVersion]
delete from [Config].[ConfigurationKey]
delete from [Config].[ConfigurationCategory]

set identity_insert [Config].[ConfigurationVersion] on

insert into [Config].[ConfigurationVersion]([Id], [Label], [Created]) 
values(1, N'First', getdate())

set identity_insert [Config].[ConfigurationVersion] off

insert into [Config].[CurrentConfigurationVersion]([Id], [CurrentVersion], [LastModified])
values(1, 1, getdate())

insert into [Config].[ConfigurationCategory]([Id], [Description], [IsActive], [LastModified]) values
('Category1', null, 1, getdate()),
('Category2', null, 1, getdate())

insert into [Config].[ConfigurationKey]([Id], [CategoryId], [Description], [IsActive], [LastModified]) values
('Key1', 'Category1', null, 1, getdate()),
('Key2', 'Category1', null, 1, getdate()),
('Key3', 'Category1', null, 1, getdate()),
('Key4', 'Category1', null, 1, getdate()),
('BoolVal', 'Category2', null, 1, getdate()),
('DoubleVal', 'Category2', null, 1, getdate()),
('StringVal', 'Category2', null, 1, getdate())

insert into [Config].[ConfigurationValue]([VersionId], [Category], [ConfigKey], [Value]) values
(1, 'Category1', 'Key1', '123'),
(1, 'Category1', 'Key2', 'Hello'),
(1, 'Category1', 'Key3', 'Doe, John'),
(1, 'Category2', 'BoolVal', 'True'),
(1, 'Category2', 'DoubleVal', '12.23'),
(1, 'Category2', 'StringVal', 'Hi!')
