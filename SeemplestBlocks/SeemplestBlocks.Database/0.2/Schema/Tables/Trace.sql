CREATE TABLE [Diagnostics].[Trace]
(
	[Id] INT NOT NULL IDENTITY, 
    [Timestamp] DATETIME NOT NULL, 
    [SessionId] VARCHAR(64) NULL, 
    [BusinessTransactionId] VARCHAR(64) NULL, 
    [OperationInstanceId] VARCHAR(64) NULL, 
    [TenantId] VARCHAR(64) NULL, 
    [OperationType] VARCHAR(64) NOT NULL, 
    [Message] NVARCHAR(MAX) NOT NULL, 
    [DetailedMessage] NVARCHAR(MAX) NULL, 
    [Type] INT NOT NULL, 
    [ServerName] VARCHAR(64) NOT NULL, 
    [ThreadId] INT NOT NULL, 
    CONSTRAINT [PK_Trace] PRIMARY KEY ([Id])
)
