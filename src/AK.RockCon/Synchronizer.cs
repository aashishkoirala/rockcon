/*******************************************************************************************************************************
 * AK.RockCon.Synchronizer
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

using Id3;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace AK.RockCon
{
    /// <summary>
    /// Synchronizes an MP3 file with the information in the repository by copying over relevant information to
    /// its ID3 tag.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface ISynchronizer
    {
        void SyncLibraryToFileSystem();
        void SyncLibraryToFileSystemForFile(string file);
    }

    /// <summary>
    /// Synchronizes an MP3 file with the information in the repository by copying over relevant information to
    /// its ID3 tag.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (ISynchronizer)), PartCreationPolicy(CreationPolicy.Shared)]
    public class Synchronizer : ISynchronizer
    {
        private const string SearchPattern = "*.mp3";

        private readonly IDictionary<string, Item> items;
        private readonly string mediaPath;

        [ImportingConstructor]
        public Synchronizer([Import] IItemRepository repository, [Import] IConfiguration configuration)
        {
            this.items = repository.Items.ToDictionary(x => x.FilePath);
            this.mediaPath = configuration.MediaPath;
        }

        public void SyncLibraryToFileSystem()
        {
            var files = Directory.GetFiles(this.mediaPath, SearchPattern, SearchOption.AllDirectories);
            Parallel.ForEach(files, this.SyncLibraryToFileSystemForFile);
        }

        public void SyncLibraryToFileSystemForFile(string file)
        {
            Item item;
            if (!this.items.TryGetValue(file, out item)) return;

            using (var stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite))
            using (var mp3Stream = new Mp3Stream(stream, Mp3Permissions.ReadWrite))
            {
                var tag = Id3Tag.Create(2, 3);
                tag.Genre.Value = item.Genre;
                tag.Artists.Value = item.Artist;
                tag.Band.Value = item.Artist;
                tag.Title.Value = item.Title;
                tag.Album.Value = item.Album;
                tag.Year.Value = item.Year.HasValue ? item.Year.Value.ToString() : string.Empty;
                tag.Track.Value = item.TrackNumber.HasValue ? item.TrackNumber.Value.ToString() : string.Empty;

                mp3Stream.WriteTag(tag, WriteConflictAction.Replace);
            }
        }
    }
}