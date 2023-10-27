using Domain.Models;

namespace LearnGraphQL.Schema
{
    [ExtendObjectType("Query")]
    public class StudentQuery
    {
        public string StudentTest => "StudentTest";

        public IEnumerable<Student> GetAllStudents()
        {
            return new List<Student>()
            {
                new Student{Id = 1, Name = "bumindu", Grade = 10},
                new Student{Id = 2, Name = "yasith", Grade = 11},
            };
        }
    }
}
