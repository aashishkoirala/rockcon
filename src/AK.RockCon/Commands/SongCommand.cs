/*******************************************************************************************************************************
 * AK.RockCon.Commands.SongCommand
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
    /// Lets you do things with a single song.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class SongCommand : CommandBase
    {
        private readonly Item item;

        public SongCommand(Item item, bool includeArtistInTitle = false, bool isPlayListAware = true)
        {
            this.item = item;
            this.Name = includeArtistInTitle ? string.Format("{0} - {1}", item.Artist, item.Title) : item.Title;

            var dividerCommand = new ControlCommand(ControlCommandType.VisualDivider);

            this.Commands = new ICommand[]
                {
                    new PlaySongCommand(item, isPlayListAware),
                    new PlaySongNextCommand(item),
                    new EnqueueSongCommand(item),
                    dividerCommand,
                    new ViewSongInfoCommand(item),
                    new StripSongCommand(item),
                    new SynchronizeSongCommand(item)
                };
        }

        public override void Execute(IContainer container)
        {
            container.View.SendMessage("{0} - {1}", this.item.Artist, this.item.Title);
        }
    }
}