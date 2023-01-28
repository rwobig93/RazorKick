CREATE OR ALTER PROCEDURE [dbo].[spUserExtendedAttribute_Update]
    @Id UNIQUEIDENTIFIER,
    @OwnerId UNIQUEIDENTIFIER,
    @Name NVARCHAR(256),
    @Value NVARCHAR(1024),
    @Type int,
    @Updated datetime2,
    @PreviousValue NVARCHAR(1024)
AS
begin
    update dbo.[spUserExtendedAttributes]
    set OwnerId = @OwnerId, Name = @Name, Value = @Value, Type = @Type, Updated = @Updated, PreviousValue = @PreviousValue
    where Id = @Id;
end
