﻿using CoreCommon.Infrastructure.Helpers;
using NoDb.Data.Domain.DbModels;
using System.Collections.Generic;
using System.IO;

namespace NoDb.Business.Service.Services
{
    public class NoDbSolutionService
    {
        private readonly NoDbService noDbService;

        NoDbSolution _noDbSolution = new NoDbSolution();

        public List<NoDbProject> Projects => _noDbSolution.Projects;

        public NoDbSolutionService(NoDbService noDbService)
        {
            this.noDbService = noDbService;
            Init();
        }

        public void Init()
        {
        }

        public void SetProjects(List<NoDbProject> projects)
        {
            _noDbSolution.Projects = projects;
            WriteToFile();
        }

        private void WriteToFile()
        {
            var json = ConversionHelper.Serialize(_noDbSolution, isIndented: true);
            File.WriteAllText(noDbService.ProjectsFilePath, json);
        }
    }
}
