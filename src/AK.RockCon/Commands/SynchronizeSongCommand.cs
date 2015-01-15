/*******************************************************************************************************************************
 * AK.RockCon.Commands.SynchronizeSongCommand
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
    /// Updates the given song's MP3 file's ID3 tags with repository/library information.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class SynchronizeSongCommand : CommandBase
    {
        private readonly Item item;

        public SynchronizeSongCommand(Item item)
        {
            this.item = item;
            this.Name = "Update MP3 Tags with Library Information for this Track";
        }

        public override void Execute(IContainer container)
        {
            container.Synchronizer.SyncLibraryToFileSystemForFile(this.item.FilePath);
        }
    }
}