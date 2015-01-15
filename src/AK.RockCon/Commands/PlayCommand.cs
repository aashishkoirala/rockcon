/*******************************************************************************************************************************
 * AK.RockCon.Commands.PlayCommand
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
using System.Linq;

#endregion

namespace AK.RockCon.Commands
{
    /// <summary>
    /// Plays the given tracks and adds them to the playlist appropriately if needed.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class PlayCommand : CommandBase
    {
        protected readonly bool isPlayListAware;
        protected readonly bool randomizeOrder;
        protected readonly Item[] items;

        public PlayCommand(string name, Item[] items, bool randomizeOrder = false, bool isPlayListAware = true)
        {
            this.Name = name;
            this.isPlayListAware = isPlayListAware;
            this.randomizeOrder = randomizeOrder;
            this.items = items;
        }

        public override void Execute(IContainer container)
        {
            var random = new Random();

            var orderedItems = (this.randomizeOrder
                                    ? this.items.OrderBy(x => random.Next(this.items.Length))
                                    : this.items.Reverse()).ToArray();

            if (this.isPlayListAware)
            {
                foreach (var item in orderedItems) container.PlayList.Insert(item);
                container.PlayList.Reset();
            }

            var playItem = orderedItems.LastOrDefault();
            if (playItem == null) return;

            container.Player.Path = playItem.FilePath;
            container.Player.Play();
        }
    }
}