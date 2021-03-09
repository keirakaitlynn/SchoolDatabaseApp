using DomainModel;
using Mm.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleClient
{
    class Program
    {
        private static IBusinessLayer businessLayer = new BuinessLayer();

        static void Main(string[] args)
        {
            run();
        }

        /// <summary>
        /// Display the menu and get user selection until exit.
        /// </summary>
        public static void run()
        {
            bool repeat = true;
            int input;

            do
            {
                Menu.displayMenu();
                input = Validator.getMenuInput();

                switch (input)
                {
                    case 0:
                        repeat = false;
                        break;
                    case 1:
                        Menu.clearMenu();
                        addTeacher();
                        break;
                    case 2:
                        Menu.clearMenu();
                        updateTeacher();
                        break;
                    case 3:
                        Menu.clearMenu();
                        removeTeacher();
                        break;
                    case 4:
                        Menu.clearMenu();
                        listTeachers();
                        break;
                    case 5:
                        Menu.clearMenu();
                        listTeacherCourses();
                        break;
                    case 6:
                        Menu.clearMenu();
                        addCourse();
                        break;
                    case 7:
                        Menu.clearMenu();
                        updateCourse();
                        break;
                    case 8:
                        Menu.clearMenu();
                        removeCourse();
                        break;
                    case 9:
                        Menu.clearMenu();
                        listCourses();
                        break;
                    case 10:
                        Menu.clearMenu();
                        addExistingCourseToTeacher();
                        break;
                }
            } while (repeat);
        }

        //CRUD for teachers

        /// <summary>
        /// Add a teacher to the database.
        /// </summary>
        public static void addTeacher()
        {
            Console.WriteLine("Enter a teacher name: ");
            string teacherName = Console.ReadLine();
            Teacher it = new Teacher() { TeacherName = teacherName };
            it.EntityState = EntityState.Added;
            businessLayer.AddTeacher(it);
            Console.WriteLine("{0} has been added to the database.", teacherName);
        }

        /// <summary>
        /// Update the name of a teacher.
        /// </summary>
        public static void updateTeacher()
        {
            Menu.displaySearchOptions();
            int input = Validator.getOptionInput();
            listTeachers();

            //Find by a teacher's name
            if (input == 1)
            {
                Console.WriteLine("Enter a teacher's name: ");
                Teacher teacher = businessLayer.GetTeacherByName(Console.ReadLine());
                if (teacher != null)
                {
                    Console.WriteLine("Change this teacher's name to: ");
                    teacher.TeacherName = Console.ReadLine();
                    teacher.EntityState = EntityState.Modified;
                    businessLayer.UpdateTeacher(teacher);
                }
                else
                {
                    Console.WriteLine("Teacher does not exist.");
                }
            }
            //find by a teacher's id
            else if (input == 2)
            {
                int id = Validator.getId();
                Teacher teacher = businessLayer.GetTeacherById(id);
                if (teacher != null)
                {
                    Console.WriteLine("Change this teacher's name to: ");
                    teacher.TeacherName = Console.ReadLine();
                    teacher.EntityState = EntityState.Modified;
                    businessLayer.UpdateTeacher(teacher);
                }
                else
                {
                    Console.WriteLine("Teacher does not exist.");
                }
            }
        }

        /// <summary>
        /// Remove a teacher from the database.
        /// </summary>
        public static void removeTeacher()
        {
            listTeachers();
            int id = Validator.getId();
            Teacher teacher = businessLayer.GetTeacherById(id);
            if (teacher != null)
            {
                Console.WriteLine("{0} has been removed.", teacher.TeacherName);
                teacher.EntityState = EntityState.Deleted;
                businessLayer.RemoveTeacher(teacher);
            }
            else
            {
                Console.WriteLine("Teacher does not exist.");
            }
        }

        /// <summary>
        /// List all teachers in the database.
        /// </summary>
        public static void listTeachers()
        {
            IList<Teacher> teachers = businessLayer.GetAllTeachers();
            foreach (Teacher teacher in teachers)
                Console.WriteLine("Teacher ID: {0}, Name: {1}", teacher.TeacherId, teacher.TeacherName);
        }

        /// <summary>
        /// List the courses of a specified teacher.
        /// </summary>
        public static void listTeacherCourses()
        {
            listTeachers();
            int id = Validator.getId();
            Teacher teacher = businessLayer.GetTeacherById(id);
            if (teacher != null)
            {
                Console.WriteLine("Listing courses for [ID: {0}, Name: {1}]:", teacher.TeacherId, teacher.TeacherName);
                if (teacher.Courses.Count > 0)
                {
                    foreach (Course course in teacher.Courses)
                        Console.WriteLine("Course ID: {0}, Name: {1}", course.CourseId, course.CourseName);
                }
                else
                {
                    Console.WriteLine("No courses for [ID: {0}, Name: {1}]:", teacher.TeacherId, teacher.TeacherName);
                }
            }
            else
            {
                Console.WriteLine("Teacher does not exist.");
            }
        }

        //CRUD for courses

        /// <summary>
        /// Add a course to a teacher.
        /// </summary>
        public static void addCourse()
        {
            Console.WriteLine("Enter a course name: ");
            string courseName = Console.ReadLine();

            listTeachers();
            Console.WriteLine("Select a teacher for this course. ");
            int id = Validator.getId();
            Teacher teacher = businessLayer.GetTeacherById(id);

            if (teacher != null)
            {
                //create course
                Course course = new Course()
                {
                    CourseName = courseName,
                    TeacherId = teacher.TeacherId,
                    EntityState = EntityState.Added
                };

                //add course to teacher
                teacher.EntityState = EntityState.Modified;
                foreach (Course c in teacher.Courses)
                    c.EntityState = EntityState.Unchanged;
                teacher.Courses.Add(course);
                businessLayer.UpdateTeacher(teacher);
                Console.WriteLine("{0} has been added to the database.", courseName);
            }
            else
            {
                Console.WriteLine("Teacher does not exist.");
            }
        }

        /// <summary>
        /// Update the name of a course.
        /// </summary>
        public static void updateCourse()
        {
            Menu.displaySearchOptions();
            int input = Validator.getOptionInput();
            listCourses();

            //find course by name
            if (input == 1)
            {
                Console.WriteLine("Enter a course's name: ");
                Course course = businessLayer.GetCourseByName(Console.ReadLine());
                if (course != null)
                {
                    Menu.displayUpdateCourseOptions();
                    int ucoInput = Validator.getOptionInput();
                    // [1] change name
                    if (ucoInput == 1)
                    {
                        Console.WriteLine("Change this course's name to: ");
                        course.CourseName = Console.ReadLine();
                        course.EntityState = EntityState.Modified;
                        businessLayer.UpdateCourse(course);
                    }
                    // TODO [2] change teacher
                    else if (ucoInput == 2) 
                    {
                        //listCoursesAndTeachers();
                        Console.WriteLine("Change this course's teacher to: ");
                        string inputTeacherName = Console.ReadLine();
                        Teacher newTeacher = businessLayer.GetTeacherByName(inputTeacherName);
                        if (newTeacher != null)
                        {
                            // update course
                            Console.WriteLine("{0} has been updated.", course.CourseName);
                            course.Teacher.TeacherId = newTeacher.TeacherId;
                            course.Teacher.TeacherName = newTeacher.TeacherName;
                            course.EntityState = EntityState.Modified;
                            businessLayer.UpdateCourse(course);
                            // update old teacher & new teacher?
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Course does not exist.");
                }
            }
            //find course by id
            else if (input == 2)
            {
                int id = Validator.getId();
                Course course = businessLayer.GetCourseById(id);
                if (course != null)
                {
                    Menu.displayUpdateCourseOptions();
                    int ucoInput = Validator.getOptionInput();
                    // [1] change name
                    if (ucoInput == 1)
                    {
                        Console.WriteLine("Change this course's name to: ");
                        course.CourseName = Console.ReadLine();
                        course.EntityState = EntityState.Modified;
                        businessLayer.UpdateCourse(course);
                    }
                    // TODO: [2] change teacher
                    else if (ucoInput == 2)
                    {
                        //listCoursesAndTeachers();
                        Console.WriteLine("Change this course's teacher to: ");
                        string inputTeacherName = Console.ReadLine();
                        Teacher newTeacher = businessLayer.GetTeacherByName(inputTeacherName);
                        if (newTeacher != null) {
                            // update course
                            Console.WriteLine("{0} has been updated.", course.CourseName);
                            course.Teacher.TeacherId = newTeacher.TeacherId;
                            course.Teacher.TeacherName = newTeacher.TeacherName;
                            course.EntityState = EntityState.Modified;
                            businessLayer.UpdateCourse(course);
                            // update old teacher & new teacher?
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Course does not exist.");
                }
            }
        }

        /// <summary>
        /// Remove a course in the database.
        /// </summary>
        public static void removeCourse()
        {
            listCourses();
            int id = Validator.getId();
            Course course = businessLayer.GetCourseById(id);
            if (course != null)
            {
                Console.WriteLine("{0} has been removed.", course.CourseName);
                course.EntityState = EntityState.Deleted;
                businessLayer.RemoveCourse(course);
            }
            else
            {
                Console.WriteLine("Course does not exist.");
            }
        }

        /// <summary>
        /// List all courses in the database.
        /// </summary>
        public static void listCourses()
        {
            IList<Course> courses = businessLayer.GetAllCourses();
            foreach (Course course in courses)
                Console.WriteLine("Course ID: {0}, Name: {1}", course.CourseId, course.CourseName);
        }

        //public static void listCoursesAndTeachers() // TODO <--------
        //{
        //    IList<Course> courses = businessLayer.GetAllCourses();
        //    IList<Teacher> teachers = businessLayer.GetAllTeachers();
        //    foreach (Course course in courses)
        //    {
        //        var teachersList = (from t in teachers where t.TeacherId == course.TeacherId select t).ToList();
        //        Console.WriteLine($"Listing teachers for [Course ID: {course.CourseId}, Name: {course.CourseName}]:");
        //        if (teachersList.Count > 0)
        //        {
        //            foreach (Teacher teacher in teachersList)
        //                Console.WriteLine($"ID: {teacher.TeacherId}, Name: {teacher.TeacherName}");
        //        }
        //    }
        //}

        public static void addExistingCourseToTeacher()
        {
            // list all the existing courses
            listCourses();

            // select an existing course
            Console.WriteLine("Select a course for the new teacher. ");
            int id = Validator.getId();
            Course course = businessLayer.GetCourseById(id);

            if (course != null)
            {
                Console.WriteLine("{0} has been selected.", course.CourseName);

                // create teacher
                Console.WriteLine("Enter a teacher name: ");
                string teacherName = Console.ReadLine();
                Teacher teacher = new Teacher() { TeacherName = teacherName };
                teacher.EntityState = EntityState.Added;
                businessLayer.AddTeacher(teacher);
                Console.WriteLine("{0} has been added to the database.", teacherName);

                // create course
                course = new Course()
                {
                    CourseName = course.CourseName,
                    TeacherId = teacher.TeacherId,
                    EntityState = EntityState.Added
                };

                // add course to teacher
                teacher.EntityState = EntityState.Modified;
                foreach (Course c in teacher.Courses)
                    c.EntityState = EntityState.Unchanged;
                teacher.Courses.Add(course);
                businessLayer.UpdateTeacher(teacher);
            }
            else
            {
                Console.WriteLine("Course does not exist.");
            }
        }
    }
}