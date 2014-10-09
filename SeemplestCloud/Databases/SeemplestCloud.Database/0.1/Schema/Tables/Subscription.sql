CREATE TABLE [Platform].[Subscription]
(
	[Id] int identity NOT NULL,
	[SubscriberName] nvarchar(255) NOT NULL,
	[PrimaryEmail] nvarchar(255) NOT NULL,
	[PrimaryPhone] nvarchar(255) NULL,
	[AddrCountry] nvarchar(64) NULL,
	[AddrZip] nvarchar(16) NULL,
	[AddrTown] nvarchar(64) NULL,
	[AddrLine1] nvarchar(64) NULL,
	[AddrLine2] nvarchar(64) NULL,
	[AddrState] nvarchar(32) NULL,
	[TaxId] nvarchar(32) NULL,
	[BankAccountNo] nvarchar(24) NULL,
    [CreatedUtc] datetimeoffset NOT NULL, 
    [LastModifiedUtc] datetimeoffset NULL,
    CONSTRAINT [PK_Subscription] PRIMARY KEY ([Id]), 
)
