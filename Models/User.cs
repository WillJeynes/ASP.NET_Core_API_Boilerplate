using System.Data;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HackSheffield.Models;

public class User
{
    public long Id { get; set; }
    public string Email { get; set; }
    // public string Password { get; set; }
    // public string Hash { get; set; }

    public User(long id, string email)
    {
        Id = id;
        Email = email;
    }

    public static User? getByKey(String key)
    {
        var command = DBConnection.connection.CreateCommand();
        command.CommandText =
        @"
            SELECT *
            FROM Users WHERE (Key=$key)
        ";
        command.Parameters.AddWithValue("$key", key);
        User tmp = null;
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                tmp = new User(reader.GetInt64(0), reader.GetString(1));
            }
        }
        return tmp;
    }
    public static string? auth(String email, String password)
    {
        var command = DBConnection.connection.CreateCommand();
        command.CommandText =
            @"
            SELECT (PasswordSalt)
            FROM Users WHERE (Email=$email)
        ";
        command.Parameters.AddWithValue("email",email);
        String tmp = null;
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                tmp = reader.GetString(0);
            }
        }

        // return tmp;
        if (tmp != null)
        {
            command = DBConnection.connection.CreateCommand();
            command.CommandText =
                @"
            SELECT * FROM Users WHERE PasswordHash == $pwd
        ";
            command.Parameters.AddWithValue("$pwd", hashResult(password, tmp));
            User tmpUser = null;
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    tmpUser = new User(reader.GetInt64(0), reader.GetString(1));
                }
            }

            if (tmpUser != null)
            {
                byte[] key = RandomNumberGenerator.GetBytes(128 / 8);
                string keystr = Convert.ToBase64String(key);
                
                command = DBConnection.connection.CreateCommand();
                command.CommandText =
                    @"
            UPDATE Users SET Key=$key WHERE Email==$email
        ";
                command.Parameters.AddWithValue("$key", keystr);
                command.Parameters.AddWithValue("$email", email);
                command.ExecuteNonQuery();

                return keystr;
            }


        }

        return "User not found";

    }
    
    public static bool insert(String email, String password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
        string saltstr = Convert.ToBase64String(salt);
        string hash = hashResult(password, saltstr);
        
        // String[] pwd = hash(password);
        var command = DBConnection.connection.CreateCommand();
        command.CommandText =
        @"
            INSERT INTO Users (Email,PasswordHash,PasswordSalt) VALUES($email,$pwd,$salt)
        ";
        command.Parameters.AddWithValue("$email", email);
        command.Parameters.AddWithValue("$pwd", hash);
        command.Parameters.AddWithValue("$salt", saltstr);
        command.ExecuteNonQuery();
        return true;
    }
    

    public static string hashResult(String password, string saltstr)
    {
        byte[] salt = Convert.FromBase64String(saltstr);
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password!,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
        
        return hashed;
    }
}
