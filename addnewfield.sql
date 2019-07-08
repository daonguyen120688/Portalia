select * from [dbo].[AttributeDetail]

update [dbo].[AttributeDetail] set IsRequired = 1,AttributeTypeId=1 where AttributeDetailId = 31
insert into [dbo].[AttributeDetail] values('Situation', 1,'select','/DataSource/Situation')

use portalia
go

CREATE PROCEDURE AddNewAttributeForUsers   
AS   
	DECLARE @AttributeCount INT = 0;
    Set @AttributeCount = (SELECT COUNT(*) FROM [dbo].[AttributeDetail])
    ;WITH UserProfile AS 
     (SELECT DISTINCT [UP].UserProfileId AS UserProfileId FROM [dbo].[UserProfileAttribute] [UP]
     GROUP BY [UP].UserProfileId
     HAVING COUNT(*) < @AttributeCount)
     insert into [dbo].[UserProfileAttribute](Value, UserProfileId, AttributeDetailId, FileBinary) 
     SELECT NULL, [UPID].UserProfileId , [AD].AttributeDetailId, NULL
     from  [dbo].[AttributeDetail]  [AD] 
           JOIN UserProfile [UPID] ON 1 = 1
           left join [dbo].[UserProfileAttribute] [UP] ON [UP].AttributeDetailId = [AD].AttributeDetailId AND [UP].UserProfileId = [UPID].UserProfileId
	 WHERE [UP].UserProfileId IS NULL
GO  

select a.UserProfileId,at.AttributeDetailId
from [dbo].[UserProfile] u, dbo.UserProfileAttribute a,dbo.[AttributeDetail] at
where u.UserProfileId = a.UserProfileId and at.AttributeDetailId = a.AttributeDetailId


select * from [dbo].[AttributeDetail]

select * from [dbo].[UserProfileAttribute] UP
inner join [dbo].[AttributeDetail] AD ON AD.AttributeDetailId = UP.AttributeDetailId
ORDER BY UP.UserProfileId


