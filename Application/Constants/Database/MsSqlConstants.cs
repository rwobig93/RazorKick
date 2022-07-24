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
        public const string UserRoleJunction = "UserRoleJunction.sql";
        public const string Validators = "Validators.sql";
    }
    
    public static class StoredProcedures
    {
        public const string ExampleUserDelete = "spExampleUser_Delete.sql";
        public const string ExampleUserGet = "spExampleUser_Get.sql";
        public const string ExampleUserGetAll = "spExampleUser_GetAll.sql";
        public const string ExampleUserCreate = "spExampleUser_Insert.sql";
        public const string ExampleUserUpdate = "spExampleUser_Update.sql";
        
        public const string UserDelete = "spUser_Delete.sql";
        public const string UserGetById = "spUser_GetById.sql";
        public const string UserGetByUsername = "spUser_GetByUsername.sql";
        public const string UserGetByEmail = "spUser_GetByEmail.sql";
        public const string UserGetAll = "spUser_GetAll.sql";
        public const string UserCreate = "spUser_Insert.sql";
        public const string UserUpdate = "spUser_Update.sql";
        public const string UserCount = "spUser_GetCount.sql";
        public const string UserSearch = "spUser_Search.sql";
        
        // TODO: Create Sql Table & Stored Procedures
        public const string RoleDelete = "spRole_Delete.sql";
        public const string RoleGetById = "spRole_GetById.sql";
        public const string RoleGetByUsername = "spRole_GetByUsername.sql";
        public const string RoleGetAll = "spRole_GetAll.sql";
        public const string RoleCreate = "spRole_Insert.sql";
        public const string RoleUpdate = "spRole_Update.sql";
        public const string RoleCount = "spRole_GetCount.sql";
        
        // TODO: Create Sql Table & Stored Procedures
        public const string ValidatorDelete = "spValidator_Delete.sql";
        public const string ValidatorCreate = "spValidator_Insert.sql";
        public const string ValidatorGet = "spValidator_Get.sql";
        
        // TODO: Create Sql Table & Stored Procedures
        public const string JunctionUserRoleDelete = "spJunction_UserRoleDelete.sql";
        public const string JunctionUserRoleCreate = "spJunction_UserRoleInsert.sql";
        public const string JunctionUserRoleGetForUser = "spJunction_UserRoleGetForUser.sql";
        public const string JunctionUserRoleGetForRole = "spJunction_UserRoleGetForRole.sql";
    }
}