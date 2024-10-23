using Neo4j.Driver;
using Neo4jClient;
using Neo4jClient.Cypher;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks; // Додано для використання Task

namespace DAL
{
    public class Person
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }
    }

    public class Neo4jCRUD
    {
        public GraphClient client;

        //private readonly IGraphClient client;

        public Neo4jCRUD()
        { 
            client = new GraphClient(new Uri("http://localhost:7474"), "neo4j", "hartovanets2204");
            ConnectAsync();
           
        }
        public async Task InitializeAsync()
        {
            await client.ConnectAsync(); // Асинхронне підключення до сервера
        }

        // Асинхронний метод для підключення
        public async Task ConnectAsync()
        {
            await client.ConnectAsync(); // Підключення до сервера
        }

        public async Task CreateRelationAsync(string user1Id, string user2Id)
        {
            await client.Cypher
              .Match("(user1:Person {userId: $user1Id})", "(user2:Person {userId: $user2Id})")
              .WithParam("user1Id", user1Id)
              .WithParam("user2Id", user2Id)
              .Create("(user1)-[:Following]->(user2)")
              .ExecuteWithoutResultsAsync();
        }

        //private  async Task<int> PathLengthAsync(IDriver driver, string sourceNodeId, string targetNodeId)
        //{
        //    int pathLength = -1;
        //    var query = @"
        //    MATCH (start {id: $sourceNodeId}), (end {id: $targetNodeId})
        //    MATCH p=shortestPath((start)-[*]-(end))
        //    RETURN length(p) AS pathLength
        //";

        //    var session = driver.AsyncSession(o => o.WithDatabase("socialnetwork")); // Use the appropriate database name if not the default

        //    try
        //    {
        //        var result = await session.ReadTransactionAsync(async tx =>
        //        {
        //            var resultCursor = await tx.RunAsync(query, new { sourceNodeId, targetNodeId });
        //            var record = await resultCursor.SingleAsync();
        //            return record["pathLength"].As<int>();
        //        });

        //        pathLength = result;
        //    }
        //    catch (Exception ex)
        //    {
        //        //Console.WriteLine($"An error occurred: {ex.Message}");
        //    }
        //    finally
        //    {
        //        await session.CloseAsync();
        //    }

        //    return pathLength;
        //}

        //public  async Task<int> GetPath(string sourceNodeId, string targetNodeId)
        //{
           
        //    IDriver driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "hartovanets2204"));

        //    try
        //    {
              

        //        var pathLength = await PathLengthAsync(driver, sourceNodeId, targetNodeId);
        //        return pathLength;
        //    }
        //    finally
        //    {
        //        await driver.CloseAsync();
        //    }
            
        //}

        public async Task<int> PathLengthAsync(string user1Id, string user2Id)
        {
            try
            {
                // поміняв u1 на user1 та u2 на user2
                // було "(user1:Person {userId: $user1Id})", "(user2:Person {userId: $user2Id})", "p = shortestPath((user1)-[:Following *]->(user2))"
                var path = await client.Cypher
                    .Match("(user1:Person {userId: $user1Id})", "(user2:Person {userId: $user2Id})", "p = shortestPath((user1)-[:Following *]->(user2))")
                    .WithParam("user1Id", user1Id)
                    .WithParam("user2Id", user2Id)
                    .Return(p => Return.As<int>("length(p)"))
                    .ResultsAsync;

                return path.FirstOrDefault();
                // Використовуйте FirstOrDefault для безпечного доступу
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task DeleteRelationAsync(string user1Id, string user2Id)
        {
            await client.Cypher
                .Match("(user1:Person {userId:$user1Id})-[r:Following]->(user2:Person $userId:$user2Id})")
                .WithParam("user1Id", user1Id)
                .WithParam("user2Id", user2Id)
                .Delete("r")
                .ExecuteWithoutResultsAsync();
        }
    }
}


//bolt://localhost:7687
//http://localhost:7474
// http://localhost:7474/db/data/  СТАСА