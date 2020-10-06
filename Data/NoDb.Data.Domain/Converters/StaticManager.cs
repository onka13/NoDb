using NoDb.Data.Domain.DbModels;
using System.Collections.Generic;
using System.Linq;

namespace NoDb.Data.Domain.Converters
{
    public static class StaticManager
    {
        static NoDbSolutionModel _solution;
        public static NoDbSolutionModel Solution
        {
            get
            {
                if (_solution == null) _solution = new NoDbSolutionModel();
                return _solution;
            }
            set
            {
                _solution = value;
            }
        }

        static string _selectedProject;
        public static string SelectedProject
        {
            get
            {
                return _selectedProject;
            }
            set
            {
                _selectedProject = value;
                _enums = null;
            }
        }
        
        static List<NoDbEnumDetail> _enums;
        public static List<NoDbEnumDetail> Enums
        {
            get
            {
                if(_enums == null) _enums = GetSelectedProject(SelectedProject).NoDbEnum.EnumList;
                return _enums;
            }
            set
            {
                _enums = value;
            }
        }

        public static NoDbTable SelectedTable { get; set; }


        public static string SelectedForeignTable { get; set; }

        public static NoDbProjectModel GetSelectedProject(string projectName)
        {
            var project = Solution.Projects.FirstOrDefault(x => x.Project.Name == projectName);
            return project;
        }
    }
}
