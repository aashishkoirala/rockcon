/*******************************************************************************************************************************
 * AK.RockCon.Commands.ScanNewMediaCommand
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
    /// Scans the media path for any MP3 files that are not included in the repository already, and adds their information to
    /// the repository.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class ScanNewMediaCommand : CommandBase
    {
        public ScanNewMediaCommand()
        {
            this.Name = "Scan New Tracks and Add to Library";
        }

        public override void Execute(IContainer container)
        {
            var view = container.View;

            view.SendMessage("Scanning {0}...", container.Configuration.MediaPath);
            var items = container.Scanner.Scan();

            var existingItems = container.ItemRepository.Items;
            var newItems = items
                .Where(x => existingItems.All(y => y.FilePath != x.FilePath))
                .ToArray();

            if (!newItems.Any())
            {
                view.SendMessage("Did not find anything new.");
                return;
            }

            view.SendMessage("Found {0} new items, adding to library...", newItems.Length);
            items = existingItems.Concat(newItems).ToArray();

            container.ItemRepository.Items = items;
            view.SendMessage("Done.");
        }
    }
}