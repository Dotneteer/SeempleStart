delete from [Platform].[UserInvitation]
delete from [Platform].[SubscriptionOwner]
delete from [Platform].[SubscriptionContent]
delete from [Platform].[SubscriptionParameterValue]

update [Platform].[User] set [SubscriptionId] = null

delete from [Platform].[Subscription]
delete from [Platform].[UserAccount]
delete from [Platform].[User]
