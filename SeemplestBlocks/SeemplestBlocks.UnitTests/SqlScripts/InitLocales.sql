--- A táblák törlése
delete from [Config].[LocalizedResource]
delete from [Config].[Locale]

--- Az alapértelmezett nyelvek inicializálása
insert into [Config].[Locale] (Code, DisplayName) values
('def', N'Alapértelmezett nyelv'),
('hu', N'Alapértelmezett magyar nyelvű beállítások'),
('hu-hu', N'Magyarországi magyar nyelvű beállítások'),
('en', N'Default English resource settings'),
('en-us', N'English resource settings for US'),
('en-gb', N'English resource settings for Great Britain')
