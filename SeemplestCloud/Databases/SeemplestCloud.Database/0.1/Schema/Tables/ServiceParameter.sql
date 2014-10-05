CREATE TABLE [Platform].[ServiceParameter]
(
	[ServiceCode] varchar(32) NOT NULL,
	[ParameterCode] varchar(32) NOT NULL,
	[Name] varchar(64) NOT NULL,
	[Description] nvarchar(1024),
	[Type] varchar(32) NOT NULL,
    CONSTRAINT [PK_ServiceParameter] PRIMARY KEY ([ServiceCode], [ParameterCode]), 
    CONSTRAINT [AK_ServiceParameterName] UNIQUE ([ServiceCode], [Name])
)
