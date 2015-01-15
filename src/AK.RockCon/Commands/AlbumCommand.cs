/*******************************************************************************************************************************
 * AK.RockCon.Commands.AlbumCommand
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
    /// Lists songs in and and lets you do other things with an album.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class AlbumCommand : CommandBase
    {
        public AlbumCommand(string album, IEnumerable<Item> items, bool includeArtistInTitle = false)
        {
            this.Name = (album ?? "Unknown").Trim();

            var filteredItems = (album == null
                                     ? items.Where(x => x.Album == null)
                                     : items.Where(x => IsEqual(x.Album, x.Year, album))).ToArray();

            var songCommands = filteredItems
                .OrderBy(x => x.TrackNumber)
                .ThenBy(x => x.Title)
                .Select(x => new SongCommand(x, includeArtistInTitle))
                .Cast<ICommand>()
                .ToArray();

            var playAllCommand = new PlayCommand(string.Format("Play All in {0}", this.Name), filteredItems);
            var playAllRandomCommand = new PlayCommand(
                string.Format("Play All Shuffled in {0}", this.Name), filteredItems, true);

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
                .Concat(songCommands)
                .ToArray();
        }

        public override void Execute(IContainer container)
        {
            container.View.SendMessage("Please pick a song/track.");
        }

        private static bool IsEqual(string album, int? year, string targetAlbum)
        {
            return year.HasValue
                       ? targetAlbum.Equals(string.Format("{0} ({1})", album, year.Value))
                       : targetAlbum == album;
        }
    }
}