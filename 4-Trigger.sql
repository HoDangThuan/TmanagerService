go
use TmanagerDB
 -----------------------------------TRIGGER---------------------------------------------

   --1.TRIGGER KHI THÊM, SỬA ĐẶT SÂN
go
create trigger  TRIGGER_AreaWorkings_insert_update on AreaWorkings for insert, update
As
 Begin
	declare @areaWorkingid nvarchar(50), @adminId nvarchar(450), @listAreaWorkingId nvarchar(MAX)
	select @areaWorkingid=id, @adminId=AdminId from inserted
	select @listAreaWorkingId=ListAreaWorkingId from AspNetUsers where id=@adminId
	if(@listAreaWorkingId is null)
		update AspNetUsers
		set ListAreaWorkingId=@areaWorkingid
		where id=@adminId 
	else
		update AspNetUsers
		set ListAreaWorkingId=CONCAT(ListAreaWorkingId,',NStr~| ',@areaWorkingid)
		where id=@adminId 
		
 End