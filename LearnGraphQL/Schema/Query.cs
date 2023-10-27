using LearnGraphQL.Models;

namespace LearnGraphQL.Schema
{
    public class Query
    {
        public string Test => "testing";

        public IEnumerable<Course> GetAllCourses() 
        {
            return new List<Course>()
            {
                new Course{Id = 1, Name = "bumindu", GPA = 4.5},
                new Course{Id = 2, Name = "yasith", GPA = 1.2},
            };
        }
    }
}
