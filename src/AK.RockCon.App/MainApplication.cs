/*******************************************************************************************************************************
 * AK.RockCon.App.MainApplication
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
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Reflection;

#endregion

namespace AK.RockCon.App
{
    /// <summary>
    /// Represents a way to start the main console application.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IMainApplication
    {
        void Start();
    }

    /// <summary>
    /// Main entry point and application startup logic.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (IMainApplication)), PartCreationPolicy(CreationPolicy.Shared)]
    public class MainApplication : IMainApplication
    {
        private readonly IController controller;

        [ImportingConstructor]
        public MainApplication([Import] IController controller)
        {
            this.controller = controller;
        }

        public void Start()
        {
            this.controller.Execute();
        }

        public static void Main(string[] args)
        {
            try
            {
                var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Debug.Assert(location != null);

                IMainApplication application;

                using (var directoryCatalog = new DirectoryCatalog(location))
                using (var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly()))
                using (var catalog = new AggregateCatalog(directoryCatalog, assemblyCatalog))
                using (var container = new CompositionContainer(catalog))
                {
                    application = container.GetExportedValue<IMainApplication>();
                }

                application.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}