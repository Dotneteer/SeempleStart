CREATE TABLE [Config].[Locale]
(
  [Code] VARCHAR(5) NOT NULL, 
  [DisplayName] NVARCHAR(128) NOT NULL, 
  CONSTRAINT [PK_Locale] PRIMARY KEY ([Code]), 
  CONSTRAINT [AK_LocaleOnDisplayName] UNIQUE ([DisplayName])
)
