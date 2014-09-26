insert into [Config].[Locale](Code, DisplayName) values
('fr', 'Francia teszt adatok')

insert into [Config].[LocalizedResource]([Locale], [Category], [ResourceKey], [Value]) values
('fr', 'Message', '1', 'Une')

delete from [Config].[LocalizedResource]
where [Locale] = 'def' and [Category] = 'Message' and [ResourceKey] = '3'
delete from [Config].[LocalizedResource]
where [Locale] = 'def' and [Category] = 'Error' and [ResourceKey] = '2'
