CREATE OR ALTER PROCEDURE [dbo].[spUserExtendedAttribute_Insert]
    @OwnerId UNIQUEIDENTIFIER,
    @Name NVARCHAR(256),
    @Value NVARCHAR(1024),
    @Type int,
    @Created datetime2,
    @Updated datetime2
AS
begin
    insert into dbo.[UserExtendedAttributes] (OwnerId, Name, Value, Type, Created, Updated)
    values (@OwnerId, @Name, @Value, @Type, @Created, @Updated);
end
