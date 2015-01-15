/*******************************************************************************************************************************
 * AK.RockCon.Configuration
 * Copyright © 2015 Aashish Koirala <http://aashishkoirala.github.io>
 * 
 * This file is part of RockCon.
 *  
 * RockCon is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * RockCon is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with RockCon.  If not, see <http://www.gnu.org/licenses/>.
 * 
 *******************************************************************************************************************************/

#region Namespace Imports

using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;

#endregion

namespace AK.RockCon
{
    /// <summary>
    /// Provides access to configuration settings.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IConfiguration
    {
        string RepositoryFilePath { get; }
        string MediaPath { get; }
        int MenuCount { get; }
    }

    /// <summary>
    /// Provides access to configuration settings.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (IConfiguration)), PartCreationPolicy(CreationPolicy.Shared)]
    public class Configuration : IConfiguration
    {
        private const string RepositoryFilePathConfigKey = "RepositoryFilePath";
        private const string MediaPathConfigKey = "MediaPath";
        private const string MenuCountConfigKey = "MenuCount";
        private const string EmptyJsonArray = "[]";
        private const string DefaultRepositoryFileName = "Repository.json";
        private const int DefaultMenuCount = 20;

        public Configuration()
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Debug.Assert(directory != null);

            var repositoryFilePath = ConfigurationManager.AppSettings[RepositoryFilePathConfigKey];

            if (string.IsNullOrWhiteSpace(repositoryFilePath))
                repositoryFilePath = Path.Combine(directory, DefaultRepositoryFileName);

            if (!File.Exists(repositoryFilePath)) File.WriteAllText(repositoryFilePath, EmptyJsonArray);

            var mediaPath = ConfigurationManager.AppSettings[MediaPathConfigKey];
            if (String.IsNullOrWhiteSpace(mediaPath))
                mediaPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

            var menuCountConfig = ConfigurationManager.AppSettings[MenuCountConfigKey];
            int menuCount;
            if (string.IsNullOrWhiteSpace(menuCountConfig) || !int.TryParse(menuCountConfig, out menuCount))
                menuCount = DefaultMenuCount;

            this.RepositoryFilePath = repositoryFilePath;
            this.MediaPath = mediaPath;
            this.MenuCount = menuCount;
        }

        public string RepositoryFilePath { get; private set; }

        public string MediaPath { get; private set; }

        public int MenuCount { get; private set; }
    }
}