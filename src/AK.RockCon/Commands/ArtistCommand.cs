/*******************************************************************************************************************************
 * AK.RockCon.Commands.ArtistCommand
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
    /// Lists albums for and and lets you do other things with a band/artist. Also support for compilations.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class ArtistCommand : CommandBase
    {
        private ArtistCommand(string artist, IEnumerable<Item> items, bool isCompilation)
        {
            this.Name = isCompilation ? "Various Artists" : (artist ?? "Unknown").Trim();

            var filteredItems = items.ToArray();

            if (!isCompilation)
            {
                filteredItems = (artist == null
                                     ? filteredItems.Where(x => x.Artist == null)
                                     : filteredItems.Where(x => x.Artist == artist)).ToArray();
            }

            var albumCommands = filteredItems
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Year)
                .Select(x => x.Year.HasValue ? string.Format("{0} ({1})", x.Album, x.Year.Value) : x.Album)
                .Distinct()
                .Select(x => new AlbumCommand(x, filteredItems, isCompilation));

            if (isCompilation)
            {
                this.Commands = albumCommands.Cast<ICommand>().ToArray();
                return;
            }

            var playAllCommand = new PlayCommand(string.Format("Play All by {0}", this.Name), filteredItems);
            var playAllRandomCommand = new PlayCommand(
                string.Format("Play All Shuffled by {0}", this.Name), filteredItems, true);

            var stripCommand = new StripCommand(filteredItems);
            var syncCommand = new SynchronizeCommand(filteredItems);

            var dividerCommand = new ControlCommand(ControlCommandType.VisualDivider);

            this.Commands = new ICommand[]
                {
                    playAllCommand,
                    playAllRandomCommand,
                    dividerCommand,
                    stripCommand,
                    syncCommand,
                    dividerCommand
                }
                .Concat(albumCommands)
                .ToArray();
        }

        public ArtistCommand(string artist, IEnumerable<Item> items) : this(artist, items, false)
        {
        }

        public static ICommand Compilation(IEnumerable<Item> items)
        {
            return new ArtistCommand(null, items, true);
        }
    }
}