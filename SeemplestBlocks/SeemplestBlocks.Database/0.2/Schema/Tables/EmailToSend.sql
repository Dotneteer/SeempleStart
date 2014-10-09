CREATE TABLE [Email].[EmailToSend]
(
    [Id] INT IDENTITY NOT NULL,
    [QueuedUtc] DATETIMEOFFSET NOT NULL,
    [FromAddr] NVARCHAR(256) NOT NULL, 
    [FromName] NVARCHAR(256) NULL, 
    [ToAddr] NVARCHAR(256) NOT NULL, 
    [TemplateType] VARCHAR(64) NOT NULL,
    [Parameters] nvarchar(MAX) NULL,
	[Appointment] nvarchar(MAX) NULL,
    [SendAfterUtc] DATETIMEOFFSET NULL, 
    [RetryCount] INT NOT NULL, 
    [LastError] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_EmailToSend] PRIMARY KEY ([Id])
)