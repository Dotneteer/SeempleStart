--- A táblák törlése
delete from [Config].[ListItemDefinition]
delete from [Config].[ListDefinition]
delete from [Config].[LocalizedResource]

--- A listak beszúrása
insert into [Config].[ListDefinition]([Id], [IsSystemList]) values
('Type', 0),
('Cat', 0)

insert into [Config].[ListItemDefinition]([ListId], [ItemId], [IsSystemItem]) values
('Type', '1', 0),
('Type', '2', 0),
('Type', '3', 0),
('Cat', 'F', 0),
('Cat', 'S', 0)

--- A listaerőforrások beszúrása
insert into [Config].[LocalizedResource]([Locale], [Category], [ResourceKey], [Value]) values
('def', 'ListItems', 'Type.1.Name', 'Type.Value1'),
('def', 'ListItems', 'Type.1.Description', 'Type.Des1'),
('def', 'ListItems', 'Type.2.Name', 'Type.Value2'),
('def', 'ListItems', 'Type.2.Description', 'Type.Des2'),
('def', 'ListItems', 'Type.3.Name', 'Type.Value3'),
('def', 'ListItems', 'Type.3.Description', 'Type.Des3'),
('def', 'ListItems', 'Cat.F.Name', 'Cat.Value1'),
('def', 'ListItems', 'Cat.F.Description', 'Cat.Des1'),
('def', 'ListItems', 'Cat.S.Name', 'Cat.Value2'),
('def', 'ListItems', 'Cat.S.Description', 'Cat.Des2'),
('hu', 'ListItems', 'Type.1.Name', 'Type.1'),
('hu', 'ListItems', 'Type.1.Description', 'Type.D1'),
('hu', 'ListItems', 'Type.2.Name', 'Type.2'),
('hu', 'ListItems', 'Type.2.Description', 'Type.D2'),
('hu', 'ListItems', 'Type.3.Name', 'Type.3'),
('hu', 'ListItems', 'Type.3.Description', 'Type.D3'),
('hu', 'ListItems', 'Cat.F.Name', 'Cat.1'),
('hu', 'ListItems', 'Cat.F.Description', 'Cat.D1'),
('hu', 'ListItems', 'Cat.S.Name', 'Cat.2'),
('hu', 'ListItems', 'Cat.S.Description', 'Cat.D2')
