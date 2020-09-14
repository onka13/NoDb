using System.Collections.Generic;

namespace NoDb.Data.Domain.DbModels
{
    public class NoDbSolution
    {
        public List<NoDbProject> Projects { get; set; }

        public NoDbSolution()
        {
            Projects = new List<NoDbProject>();
        }
    }

    public class NoDbProject
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public bool IsSelected { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class NoDbSolutionModel
    {
        public List<NoDbProjectModel> Projects { get; set; }

        public NoDbSolutionModel()
        {
            Projects = new List<NoDbProjectModel>();
        }
    }

    public class NoDbProjectModel
    {
        public NoDbProject Project { get; set; }

        public List<NoDbTable> Tables { get; set; }

        public NoDbEnum NoDbEnum { get; set; }

        public override string ToString()
        {
            return Project.Name;
        }
    }
}
