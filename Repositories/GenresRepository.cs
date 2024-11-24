using Dapper;
using Microsoft.Data.SqlClient;
using MinimalAPIsMovies.Entities;
using System.Data;


namespace MinimalAPIsMovies.Repositories
{
    public class GenresRepository : IGenresRepository
    {
		private readonly string connectionString;

        public GenresRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

         public async Task<int> Create(Genre genre)
        {
            using (var connection = new SqlConnection(connectionString))
            {
            	var query=@"
            				INSERT INTO Genres (Name)
            				VALUES (@Name);
            				SELECT SCOPE_IDENTITY();
			            	";
                var id = await connection.QuerySingleAsync<int>(query,genre);
                genre.Id = id;
                return id;
            }
        }

        public async Task<List<Genre>> GetAll()
        {
            using (var connection=new SqlConnection(connectionString))
            {
                var genres=await connection.QueryAsync<Genre>(@"SELECT Id, Name From Genres");
                return genres.ToList();
            }
        }

        public async Task<Genre?> GetById(int id)
        {
            using(var connection=new SqlConnection(connectionString))
            {
                var genre=await connection.QueryFirstOrDefaultAsync<Genre>(@"SELECT Id, Name
                                                                             From Genres
                                                                            WHERE Id=@Id",new {id});
                return genre;
            }
        }

        public async Task<bool> Exists(int id)
        {
            using (var connection=new SqlConnection(connectionString)){
                var exists=await connection.QuerySingleAsync<bool>(@"IF Exists (SELECT 1 FROM Genres Where Id=@Id)
                                                                        SELECT 1;
                                                                        ELSE
                                                                        SELECT 0;",new {id});
                return exists;
            }
        }

        public async Task Update(Genre genre)
        {
            using (var connection=new SqlConnection(connectionString)){
                await connection.ExecuteAsync(@"UPDATE Genres SET Name=@Name WHERE Id=@Id",genre);
            }
        }

        public async Task Delete(int id){
            using (var connection=new SqlConnection(connectionString)){
                await connection.ExecuteAsync("DELETE Genres Where Id=@Id",new {id});
            }
        }
    }
}