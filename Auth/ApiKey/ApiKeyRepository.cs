using Dapper;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace custom_auth.Auth
{
    public class ApiKeyRepository
    {
        private readonly string connString;

        public ApiKeyRepository(string connString)
        {
            this.connString = connString;
        }

        public async Task<ApiKeyDB> GetApiKeyInfoAsync(ApiKey apiKey)
        {
            using SqlConnection conn = new SqlConnection(connString);
            return await conn.QuerySingleOrDefaultAsync<ApiKeyDB>(@"SELECT * FROM ApiKeys WHERE Id = @Id;", apiKey);
        }

        public async Task<int> UpdateApiKeyHashAsync(long id, string hash)
        {
            using SqlConnection conn = new SqlConnection(connString);
            return await conn.ExecuteAsync(@"UPDATE ApiKeys SET Hash = @hash WHERE Id = @id;", new { id, hash });
        }
    }
}
