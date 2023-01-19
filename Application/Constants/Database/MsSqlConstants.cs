namespace Application.Constants.Database;

public static class MsSqlConstants
{
    public const string PathTables = "Infrastructure.DatabaseScripts.MsSql.Tables.";
    public const string PathStoredProcedures = "Infrastructure.DatabaseScripts.MsSql.StoredProcedures.";

    public static class Tables
    {
        public const string ExampleUser = "ExampleUser.sql";
        public const string Users = "Users.sql";
        public const string Roles = "Roles.sql";
        public const string UserRoleJunctions = "UserRoleJunctions.sql";
        public const string UserExtendedAttributes = "UserExtendedAttributes.sql";
    }
    
    public static class StoredProcedures
    {
        public const string GeneralGetTableRowCount = "spGeneral_GetRowCount.sql";
        
        public const string UserDelete = "spUser_Delete.sql";
        public const string UserGetAll = "spUser_GetAll.sql";
        public const string UserGetByEmail = "spUser_GetByEmail.sql";
        public const string UserGetById = "spUser_GetById.sql";
        public const string UserGetByUsername = "spUser_GetByUsername.sql";
        public const string UserCreate = "spUser_Insert.sql";
        public const string UserSearch = "spUser_Search.sql";
        public const string UserUpdate = "spUser_Update.sql";

        public const string UserExtendedAttributeDelete = "spUserExtendedAttribute_Delete.sql";
        public const string UserExtendedAttributeGetAll = "spUserExtendedAttribute_GetAll.sql";
        public const string UserExtendedAttributeGetByOwnerId = "spUserExtendedAttribute_GetByOwnerId.sql";
        public const string UserExtendedAttributeGetByOwnerIdAndType = "spUserExtendedAttribute_GetByOwnerIdAndType.sql";
        public const string UserExtendedAttributeInsert = "spUserExtendedAttribute_Insert.sql";
        public const string UserExtendedAttributeUpdate = "spUserExtendedAttribute_Update.sql";

        // TODO: Implement Claims to Attach or Junction to Roles to allow permissions bound to Roles
        public const string RoleDelete = "spRole_Delete.sql";
        public const string RoleGetAll = "spRole_GetAll.sql";
        public const string RoleGetById = "spRole_GetById.sql";
        public const string RoleCreate = "spRole_Insert.sql";
        public const string RoleSearch = "spRole_Search.sql";
        public const string RoleUpdate = "spRole_Update.sql";
        
        // TODO: Create Sql Table & Stored Procedures
        public const string JunctionUserRoleDelete = "spJunction_UR_Delete.sql";
        public const string JunctionUserRoleGetAll = "spJunction_UR_GetAll.sql";
        public const string JunctionUserRoleGetByUserRoleId = "spJunction_UR_GetByUserRoleId";
        public const string JunctionUserRoleGetRolesOfUser = "spJunction_UR_GetRolesOfUser.sql";
        public const string JunctionUserRoleGetUsersOfRole = "spJunction_UR_GetUsersOfRole.sql";
        public const string JunctionUserRoleCreate = "spJunction_UR_Insert.sql";
        public const string JunctionUserRoleSearch = "spJunction_UR_Search.sql";
        
        // Example Objects for implementation understanding
        public const string ExampleUserDelete = "spExampleUser_Delete.sql";
        public const string ExampleUserGet = "spExampleUser_Get.sql";
        public const string ExampleUserGetAll = "spExampleUser_GetAll.sql";
        public const string ExampleUserCreate = "spExampleUser_Insert.sql";
        public const string ExampleUserUpdate = "spExampleUser_Update.sql";
    }
}