/*******************************************************************************************************************************
 * AK.RockCon.Commands.ManageLibraryCommand
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

namespace AK.RockCon.Commands
{
    /// <summary>
    /// Represents the "Manage Library" command that spawns options for managing the repository.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class ManageLibraryCommand : CommandBase
    {
        public ManageLibraryCommand()
        {
            this.Name = "Manage Library";
            this.Commands = new ICommand[]
                {
                    new ScanNewMediaCommand(),
                    new StripLibraryCommand(),
                    new SynchronizeLibraryCommand()
                };
        }
    }
}