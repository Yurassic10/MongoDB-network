using Neo4j.Driver;
using Neo4jClient;
using Neo4jClient.Cypher;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks; 

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
            await client.ConnectAsync(); 
        }

        public async Task ConnectAsync()
        {
            await client.ConnectAsync(); 
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



        public async Task<int> PathLengthAsync(string user1Id, string user2Id)
        {
            try
            {
               
                var path = await client.Cypher
                    .Match("(user1:Person {userId: $user1Id})", "(user2:Person {userId: $user2Id})", "p = shortestPath((user1)-[:Following *]->(user2))")
                    .WithParam("user1Id", user1Id)
                    .WithParam("user2Id", user2Id)
                    .Return(p => Return.As<int>("length(p)"))
                    .ResultsAsync;

                return path.FirstOrDefault();
                
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


