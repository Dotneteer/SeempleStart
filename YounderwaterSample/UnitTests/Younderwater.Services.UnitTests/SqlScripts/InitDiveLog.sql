delete from [Dive].[DiveLog]

set identity_insert [Dive].[DiveLog] on

insert into [Dive].[DiveLog] ([Id], [UserId], [Date], [DiveSite], [Location], [MaxDepth], [BottomTime], [Comment]) values
(1, 'test', '06/20/2008', 'Fanous East', 'Hughada, Egypt', 13.4, 68, null),
(2, 'test', '06/21/2008', 'Abu Ramada South', 'Hughada, Egypt', 24.6, 79, null),
(3, 'test', '06/21/2008', 'Gotta Abu Ramada', 'Hughada, Egypt', 7.8, 84, null),
(4, 'test', '06/23/2008', 'Small Giftun', 'Hughada, Egypt', 24.0, 64, null),
(5, 'test', '06/21/2008', 'Erg Somaya', 'Hughada, Egypt', 20.0, 78, null),
(6, 'test', '07/18/2008', 'Ecsédi tó', 'Egyéd, Magyarország', 8.0, 64, null),
(7, 'test', '07/18/2008', 'Ecsédi tó', 'Egyéd, Magyarország', 8.0, 72, null),
(8, 'test', '12/02/2008', 'Molnár János Barlang', 'Budapest, Magyarország', 36.0, 55, null),
(9, 'test', '01/16/2009', 'Ponte Wahoo', 'Maehbourg, Mauritius', 24.3, 49, null),
(10, 'other', '01/16/2009', 'Ponte Wahoo', 'Maehbourg, Mauritius', 24.3, 49, null)

set identity_insert [Dive].[DiveLog] off
