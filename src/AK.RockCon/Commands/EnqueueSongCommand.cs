/*******************************************************************************************************************************
 * AK.RockCon.Commands.EnqueueSongCommand
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
    /// Adds a song to the end of the active playlist.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class EnqueueSongCommand : CommandBase
    {
        private readonly Item item;

        public EnqueueSongCommand(Item item)
        {
            this.item = item;
            this.Name = "Add To End of List";
        }

        public override void Execute(IContainer container)
        {
            container.PlayList.Enqueue(this.item);
            if (container.PlayList.Current != this.item) return;

            container.Player.Path = this.item.FilePath;
            container.Player.Play();
        }
    }
}