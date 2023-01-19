CREATE OR ALTER PROCEDURE [dbo].[spUserExtendedAttribute_Update]
    @Id UNIQUEIDENTIFIER,
    @OwnerId UNIQUEIDENTIFIER,
    @Name NVARCHAR(256),
    @Value NVARCHAR(256),
    @Type int,
    @Updated datetime2,
    @PreviousValues NVARCHAR(2048)
AS
begin
    update dbo.[spUserExtendedAttributes]
    set OwnerId = @OwnerId, Name = @Name, Value = @Value, Type = @Type, Updated = @Updated, PreviousValues = @PreviousValues
    where Id = @Id;
end
