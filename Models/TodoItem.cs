using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HackSheffield.Models;

public class TodoItem
{
    public long Id { get; set; }
    public string? Name { get; set; }

    public TodoItem(long id, string name)
    {
        Id = id;
        Name = name;
    }

    public static List<TodoItem> getAll()
    {
        var command = DBConnection.connection.CreateCommand();
        command.CommandText =
        @"
            SELECT *
            FROM ToDo
        ";
        // command.Parameters.AddWithValue("$id", id);
        List<TodoItem> list = new List<TodoItem>();
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                list.Add(new TodoItem(reader.GetInt64(0), reader.GetString(1)));
            }
        }
        return list;
    }
    public static bool insert(String item)
    {
        var command = DBConnection.connection.CreateCommand();
        command.CommandText =
        @"
            INSERT INTO ToDo (Name) VALUES($name)
        ";
        command.Parameters.AddWithValue("$name", item);
        command.ExecuteNonQuery();
        return true;
    }

    public static bool delete(String id)
    {
        var command = DBConnection.connection.CreateCommand();
        command.CommandText =
            @"
            DELETE FROM ToDo WHERE Id=$id
        ";
        command.Parameters.AddWithValue("$id", int.Parse(id));
        command.ExecuteNonQuery();
        return true;
    }

}
