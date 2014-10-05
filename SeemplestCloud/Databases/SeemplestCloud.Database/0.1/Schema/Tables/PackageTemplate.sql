CREATE TABLE [Platform].[PackageTemplate]
(
	[Code] varchar(32) NOT NULL,
	[Name] nvarchar(64) NOT NULL,
	[UpgradeCode] varchar(32) NULL,
	[MarketingName] nvarchar(64) NOT NULL,
	[Description] nvarchar(max) NOT NULL,
	[MarketingDescription] nvarchar(max) NOT NULL,
	[IsFree] bit NOT NULL,
	[MonthlyPrice] money NULL,
	[CurrencyType] varchar(3) NULL,
    [CreatedUtc] datetimeoffset NOT NULL, 
    [LastModifiedUtc] datetimeoffset NULL,
    CONSTRAINT [PK_PackageTemplate] PRIMARY KEY ([Code]), 
    CONSTRAINT [AK_PackageTemplateName] UNIQUE ([Name]),
    CONSTRAINT [AK_PackageTemplateMarketingName] UNIQUE ([MarketingName])
)
