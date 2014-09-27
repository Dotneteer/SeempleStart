CREATE TABLE [Dive].[DiveLog]
(
	[Id] int identity not null,
	[UserId] varchar(32) NOT NULL,
	[Date] datetime NOT NULL,
	[DiveSite] nvarchar(64) NOT NULL,
	[Location] nvarchar(64) NOT NULL,
	[MaxDepth] decimal(5,2) NOT NULL,
	[BottomTime] int NOT NULL,
	[Comment] nvarchar(max) NULL,
    CONSTRAINT [PK_DiveLog] PRIMARY KEY ([Id])
)
