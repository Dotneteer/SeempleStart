CREATE TABLE [Platform].[SubscriptionContent]
(
	[SubscriptionId] int NOT NULL,
	[BasePackageCode] varchar(32) NOT NULL,
	[IsFree] bit NOT NULL,
	[MonthlyPrice] money NULL,
	[CurrencyType] varchar(3) NULL,
	[Comment] nvarchar(max) NULL,
    [CreatedUtc] datetimeoffset NOT NULL, 
    [LastModifiedUtc] datetimeoffset NULL,
    CONSTRAINT [PK_SubscriptionContent] PRIMARY KEY ([SubscriptionId]), 
)
