/*******************************************************************************************************************************
 * AK.RockCon.Commands.GenreCommand
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

using System.Collections.Generic;
using System.Linq;

#endregion

namespace AK.RockCon.Commands
{
    /// <summary>
    /// Lists artists in and and lets you do other things with a genre.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class GenreCommand : CommandBase
    {
        public GenreCommand(string genre, IEnumerable<Item> items)
        {
            this.Name = (genre ?? "Unknown").Trim();

            var filteredItems = (genre == null
                                     ? items.Where(x => x.Genre == null)
                                     : items.Where(x => x.Genre == genre)).ToArray();

            var artistItems = filteredItems.Where(x => !x.IsPartOfCompilation).ToArray();
            var compilationItems = filteredItems.Where(x => x.IsPartOfCompilation).ToArray();

            var artistCommands = artistItems
                .Select(x => x.Artist)
                .Distinct()
                .OrderBy(x => x)
                .Select(x => new ArtistCommand(x, filteredItems));

            var playGenreCommand = new PlayCommand("Play Genre", filteredItems);
            var playGenreRandomCommand = new PlayCommand("Play Genre Shuffled", filteredItems, true);

            var stripCommand = new StripCommand(filteredItems);
            var syncCommand = new SynchronizeCommand(filteredItems);

            var dividerCommand = new ControlCommand(ControlCommandType.VisualDivider);

            var compilationCommand = ArtistCommand.Compilation(compilationItems);

            this.Commands = new ICommand[]
                {
                    playGenreCommand,
                    playGenreRandomCommand,
                    dividerCommand,
                    stripCommand,
                    syncCommand,
                    dividerCommand
                }
                .Concat(artistCommands)
                .Concat(new[]
                    {
                        dividerCommand,
                        compilationCommand
                    })
                .ToArray();
        }

        public override void Execute(IContainer container)
        {
            container.View.SendMessage("Please pick a band/artist.");
        }
    }
}