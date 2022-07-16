namespace Application.Constants.Database;

public static class MsSqlConstants
{
    public const string PathTables = "Infrastructure.DatabaseScripts.MsSql.Tables.";
    public const string PathStoredProcedures = "Infrastructure.DatabaseScripts.MsSql.StoredProcedures.";

    public static class Tables
    {
        public const string User = "User.sql";
    }
    
    public static class StoredProcedures
    {
        public const string UserDelete = "spUser_Delete.sql";
        public const string UserGet = "spUser_Get.sql";
        public const string UserGetAll = "spUser_GetAll.sql";
        public const string UserCreate = "spUser_Insert.sql";
        public const string UserUpdate = "spUser_Update.sql";
    }
}