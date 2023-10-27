using Domain.Models;
using GraphQL;
using GraphQL.Client.Abstractions;

namespace GraphQLConsumer
{
    public class OwnerConsumer
    {
        private readonly IGraphQLClient _client;

        public OwnerConsumer(IGraphQLClient client)
        {
            _client = client;
        }

        public async Task GetAllOwners()
        {
            var query = new GraphQLRequest
            {
                Query = @"
                query {
                    allCourses{
                      id,
                      name,
                      gpa
                    }
                }"
            };

            var responseInDynamic = await _client.SendQueryAsync<dynamic>(query);
            var response = await _client.SendQueryAsync<AllCoursesType>(query);
            var data = response.Data;
        }
    }

    public class AllCoursesType
    {
        public List<Course> AllCourses { get; set; } // feild name should match with query return type
    }
}
