using Domain.Models;
namespace LearnGraphQL.Schema
{
    public class Mutation
    {
        public CourseResult CreateCourse(Course course)
        {
            return new CourseResult
            {
                Id = course.Id,
                Name = course.Name,
            };
        }
    }
}
