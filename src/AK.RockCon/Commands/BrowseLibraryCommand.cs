/*******************************************************************************************************************************
 * AK.RockCon.Commands.BrowseLibraryCommand
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

using System.Linq;

namespace AK.RockCon.Commands
{
    /// <summary>
    /// Represents the "Browse Library" command - lists genres among other things.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class BrowseLibraryCommand : CommandBase
    {
        private readonly Item[] items;

        public BrowseLibraryCommand(Item[] items)
        {
            this.Name = "Browse Library";
            this.items = items;

            var genreCommands = this
                .items
                .Select(x => x.Genre)
                .Distinct()
                .OrderBy(x => x)
                .Select(x => new GenreCommand(x, this.items));

            var playAllCommand = new PlayCommand("Play All", this.items);
            var playAllRandomCommand = new PlayCommand("Play All Shuffled", this.items, true);

            var dividerCommand = new ControlCommand(ControlCommandType.VisualDivider);

            this.Commands = new ICommand[] {playAllCommand, playAllRandomCommand, dividerCommand}
                .Concat(genreCommands)
                .ToArray();
        }

        public override void Execute(IContainer container)
        {
            container.View.SendMessage("Please pick a genre.");
        }
    }
}