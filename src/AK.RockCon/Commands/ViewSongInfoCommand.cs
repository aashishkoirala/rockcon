/*******************************************************************************************************************************
 * AK.RockCon.Commands.ViewSongInfoCommand
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
    /// Displays song information - both library information and MP3/ID3 tag information.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class ViewSongInfoCommand : CommandBase
    {
        private readonly Item item;

        public ViewSongInfoCommand(Item item)
        {
            this.item = item;
            this.Name = "View Information";
        }

        public override void Execute(IContainer container)
        {
            var view = container.View;
            var mp3Item = container.Scanner.ScanFile(this.item.FilePath);

            view.SendMessage("File: {0}", this.item.FilePath);
            view.SendMessage(string.Empty);
            view.SendMessage("Library Information");
            SendInformation(this.item, view, true);
            view.SendMessage(string.Empty);
            view.SendMessage("MP3 Tag Information");
            SendInformation(mp3Item, view, false);
        }

        private static void SendInformation(Item item, IView view, bool includeCompilationStatus)
        {
            view.SendMessage("Artist: {0}", item.Artist ?? "Unknown");
            view.SendMessage("Title: {0}", item.Title ?? "Unknown");
            view.SendMessage("Album: {0}", item.Album ?? "Unknown");
            view.SendMessage("Year: {0}", item.Year);
            view.SendMessage("Genre: {0}", item.Genre ?? "Unknown");
            view.SendMessage("Track #: {0}", item.TrackNumber);

            if (!includeCompilationStatus) return;

            view.SendMessage("Part of compilation: {0}", item.IsPartOfCompilation ? "Yes" : "No");
        }
    }
}