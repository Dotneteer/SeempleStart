--- A táblák törlése
delete from [Config].[LocalizedResource]
delete from [Config].[Locale]

--- Az alapértelmezett nyelvek inicializálása
insert into [Config].[Locale] (Code, DisplayName) values
('def', N'Alapértelmezett nyelv'),
('hu', N'Alapértelmezett magyar nyelvű beállítások'),
('hu-hu', N'Magyarországi magyar nyelvű beállítások'),
('de', N'Default German resource settings'),
('en', N'Default English resource settings'),
('en-us', N'English resource settings for US'),
('en-gb', N'English resource settings for Great Britain')

insert into [Config].[LocalizedResource]([Locale], [Category], [ResourceKey], [Value]) values
('def', 'Cat1', '1', 'Cat1Value1'),
('def', 'Cat1', '2', 'Cat1Value2'),
('def', 'Cat1', '3', 'Cat1Value3'),
('hu-hu', 'Cat1', '1', 'Cat1Value1-hu-hu'),
('hu-hu', 'Cat1', '2', 'Cat1Value2-hu-hu'),
('hu', 'Cat1', '1', 'Cat1Value1-hu'),
('hu', 'Cat1', '2', 'Cat1Value2-hu'),
('de', 'Cat1', '1', 'Cat1Value1-de'),
('de', 'Cat1', '2', 'Cat1Value2-de'),
('en-us', 'Cat1', '1', 'Cat1Value1-en-us'),
('en-us', 'Cat1', '2', 'Cat1Value2-en-us'),
('en', 'Cat1', '1', 'Cat1Value1-en'),
('en', 'Cat1', '2', 'Cat1Value2-en'),
('def', 'Cat2', '1', 'Cat2Value1'),
('def', 'Cat2', '2', 'Cat2Value2'),
('def', 'Cat2', '3', 'Cat2Value3'),
('hu-hu', 'Cat2', '1', 'Cat2Value1-hu-hu'),
('hu-hu', 'Cat2', '2', 'Cat2Value2-hu-hu'),
('hu', 'Cat2', '1', 'Cat2Value1-hu'),
('hu', 'Cat2', '2', 'Cat2Value2-hu'),
('en-us', 'Cat2', '1', 'Cat2Value1-en-us'),
('en-us', 'Cat2', '2', 'Cat2Value2-en-us'),
('en', 'Cat2', '1', 'Cat2Value1-en'),
('en', 'Cat2', '2', 'Cat2Value2-en')