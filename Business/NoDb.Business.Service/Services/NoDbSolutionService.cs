﻿using CoreCommon.Infra.Helpers;
using NoDb.Data.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NoDb.Business.Service.Services
{
    public class NoDbSolutionService
    {
        NoDbSolution _noDbSolution;
        readonly string _solutionFolder;
        public string SolutionFilePath
        {
            get
            {
                return _solutionFolder + Path.DirectorySeparatorChar + "NoDbSettings" + Path.DirectorySeparatorChar + "solution.json";
            }
        }

        public List<NoDbProject> Projects => _noDbSolution.Projects;

        public List<NoDbProject> GetSelectedProjects() => _noDbSolution.Projects.Where(x =>
        {
            return x.IsSelected;
        }).ToList();

        public NoDbSolutionService(string solutionFolder)
        {
            _solutionFolder = solutionFolder;
            ReadFromSettingsFolder();
        }

        public void ReadFromSettingsFolder()
        {
            if (!File.Exists(SolutionFilePath))
            {
                _noDbSolution = new NoDbSolution();
            }
            else
            {
                var json = File.ReadAllText(SolutionFilePath);
                _noDbSolution = ConversionHelper.Deserialize<NoDbSolution>(json);
            }
        }

        public void UpdateModule(string moduleName, bool isSelected)
        {
            if (string.IsNullOrEmpty(moduleName))
            {
                throw new Exception("Name can not be empty!");
            }

            var module = _noDbSolution.Projects.FirstOrDefault(x => x.Name == moduleName);
            module.IsSelected = isSelected;

            WriteToFile();
        }

        public void UpdateAllModules(List<NoDbProject> modules, bool keepOld = true)
        {
            if (keepOld)
            {
                foreach (var module in modules)
                {
                    var oldModule = _noDbSolution.Projects.FirstOrDefault(x => x.Name == module.Name);
                    if (oldModule != null)
                    {
                        module.IsSelected = oldModule.IsSelected;
                    }
                }
            }
            _noDbSolution.Projects = modules;

            WriteToFile();
        }

        private void WriteToFile()
        {
            var json = ConversionHelper.Serialize(_noDbSolution);
            File.WriteAllText(SolutionFilePath, json);
        }
    }
}