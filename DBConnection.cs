using Microsoft.Data.Sqlite;

public class DBConnection {
    public static SqliteConnection connection = new SqliteConnection("Data Source=data.db");
    public static void init() {
        connection.Open();
    }
}