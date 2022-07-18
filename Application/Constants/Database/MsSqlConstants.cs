namespace Application.Constants.Database;

public static class MsSqlConstants
{
    public const string PathTables = "Infrastructure.DatabaseScripts.MsSql.Tables.";
    public const string PathStoredProcedures = "Infrastructure.DatabaseScripts.MsSql.StoredProcedures.";

    public static class Tables
    {
        public const string ExampleUser = "ExampleUser.sql";
        public const string Users = "Users.sql";
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
        public const string UserGetAll = "spUser_GetAll.sql";
        public const string UserCreate = "spUser_Insert.sql";
        public const string UserUpdate = "spUser_Update.sql";
        public const string UserCount = "spUser_GetCount.sql";
    }
}