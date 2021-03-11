using DomainModel;
using System.Linq;

namespace Mm.DataAccessLayer
{
    public interface ITeacherRepository : IGenericDataRepository<Teacher>
    {
    }

    public interface ICourseRepository : IGenericDataRepository<Course>
    {
    }

    public class TeacherRepository : GenericDataRepository<Teacher>, ITeacherRepository
    {
    }

    public class CourseRepository : GenericDataRepository<Course>, ICourseRepository
    {
        public override void Remove(params Course[] items)
        {
            using (var contex = new Entities())
            {
                var dbSet = contex.Set<Course>();
                foreach (var item in items)
                {
                    item.Students.Clear(); // remove any students relation
                    item.Teacher = null; // remove any teacher relation
                    dbSet.Add(item);
                    foreach (var entry in contex.ChangeTracker.Entries<Course>())
                    {
                        var entity = entry.Entity;
                        entry.State = GetEntityState(entity.EntityState);
                    }
                }
                contex.SaveChanges();
            }
        }
    }
}
