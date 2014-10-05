CREATE TABLE [Platform].[PackageTemplateParameter]
(
	[PackageCode] varchar(32) NOT NULL,
	[ServiceCode] varchar(32) NOT NULL,
	[ParameterCode] varchar(32) NOT NULL,
	[Value] nvarchar(1024) NOT NULL
    CONSTRAINT [PK_PackageTemplateParameter] PRIMARY KEY ([PackageCode], [ServiceCode], [ParameterCode])
)
