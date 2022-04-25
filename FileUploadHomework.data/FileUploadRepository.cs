using System;
using System.Data.SqlClient;

namespace FileUploadHomework.data
{
    public class FileUploadRepository
    {
        private string _connectionString;
        public FileUploadRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int AddImage(Image image)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Images (ImagePath, Password, Views) 
                                VALUES (@imagePath, @password, 0)
                                SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@imagePath", image.ImagePath);
            cmd.Parameters.AddWithValue("@password", image.Password);
            
            connection.Open();
            int id = (int)(decimal)cmd.ExecuteScalar();

            connection.Close();
            connection.Dispose();

            return id;
        }

        public void UpdateView(int id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE Images set Views = Views + 1
                                WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
            connection.Dispose();
        }

        public Image GetImageById(int id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Images
                                WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
                
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            Image image = new Image
                {
                    Id = (int)reader["Id"],
                    ImagePath = (string)reader["ImagePath"],
                    Password = (string)reader["Password"],
                    Views = (int)reader["Views"]
                };
            connection.Close();
            connection.Dispose();
            return image;
        }

        public string GetPasswordByImageId(int id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT Password FROM Images
                                WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();
           
            string password = (string)cmd.ExecuteScalar();

            connection.Close();
            connection.Dispose();

            return password;
        }

    }
}
