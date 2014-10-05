CREATE TABLE [Platform].[ServiceDefinition]
(
	[Code] varchar(32) NOT NULL,
	[Name] nvarchar(64) NOT NULL,
	[MarketingName] nvarchar(64) NOT NULL,
	[Description] nvarchar(max) NOT NULL,
	[StartDateUtc] datetimeoffset NULL,
	[EndDateUtc] datetimeoffset NULL,
	[DefaultOwnerRole] varchar(32) NULL,
	[DefaultUserRole] varchar(32) NULL,
    [CreatedUtc] datetimeoffset NOT NULL, 
    [LastModifiedUtc] datetimeoffset NULL,
    CONSTRAINT [PK_ServiceDefinition] PRIMARY KEY ([Code]), 
    CONSTRAINT [AK_DefinitionName] UNIQUE ([Name]),
    CONSTRAINT [AK_DefinitionMarketingName] UNIQUE ([MarketingName])
)
