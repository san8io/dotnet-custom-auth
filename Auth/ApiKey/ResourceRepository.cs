using Dapper;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace custom_auth.Auth
{
    public interface IResourceRepository
    {
        Task<Resource> GetResourceByResourceRefAsync(string resourceRef);
    }
    public class ResourceRepository : IResourceRepository
    {
        public ResourceRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private readonly string connectionString;

        public async Task<Resource> GetResourceByResourceRefAsync(string resourceRef)
        {
            const string query = @"
SELECT * FROM Resources
WHERE ResourceRef = @resourceRef
        ";
            using SqlConnection conn = new SqlConnection(connectionString);
            return await conn.QuerySingleOrDefaultAsync<Resource>(query, new { resourceRef });
        }
    }
}