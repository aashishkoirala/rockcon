/*******************************************************************************************************************************
 * AK.RockCon.Scanner
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
using Id3.Frames;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

#endregion

namespace AK.RockCon
{
    /// <summary>
    /// Scans and provides information about MP3 files.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IScanner
    {
        Item[] Scan();
        Item ScanFile(string file);
    }

    /// <summary>
    /// Scans and provides information about MP3 files.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (IScanner)), PartCreationPolicy(CreationPolicy.Shared)]
    public class Scanner : IScanner
    {
        private const string SearchPattern = "*.mp3";

        private readonly string mediaPath;

        [ImportingConstructor]
        public Scanner([Import] IConfiguration configuration)
        {
            this.mediaPath = configuration.MediaPath;
        }

        public Item[] Scan()
        {
            var files = Directory.GetFiles(this.mediaPath, SearchPattern, SearchOption.AllDirectories);
            var list = new List<Item>();

            Parallel.ForEach(files, x => list.Add(this.ScanFile(x)));
            return list.ToArray();
        }

        public Item ScanFile(string file)
        {
            var item = new Item {FilePath = file};
            var tag = GetId3Tag(file);
            if (tag == null) return item;

            item.Genre = Format(tag.Genre);
            item.Artist = Format(tag.Artists);
            item.Title = Format(tag.Title);
            item.Album = Format(tag.Album);
            item.Year = FormatYear(tag.Year);
            item.TrackNumber = tag.Track.AsInt;

            return item;
        }

        private static Id3Tag GetId3Tag(string file)
        {
            try
            {
                using (var stream = File.OpenRead(file))
                using (var mp3Stream = new Mp3Stream(stream))
                {
                    if (mp3Stream.HasTagOfFamily(Id3TagFamily.FileStartTag))
                        return mp3Stream.GetTag(Id3TagFamily.FileStartTag);

                    if (mp3Stream.HasTagOfFamily(Id3TagFamily.FileStartTag))
                        return mp3Stream.GetTag(Id3TagFamily.FileStartTag);
                }

                return null;
            }
            catch (Exception)
            {
                // Nothing we can do about it here, we could not read the ID3 tag.
                // That's it.
                //
                return null;
            }
        }

        private static string Format(TextFrame frame)
        {
            return frame.IsAssigned ? frame.Value : null;
        }

        private static int? FormatYear(DateTimeTextFrame frame)
        {
            var dateTime = frame.AsDateTime;
            return dateTime.HasValue ? dateTime.Value.Year : (int?) null;
        }

        #region Dead Code That I'll Need Later

        // I need the following method for some thing I need to do at some point.
        //
        /*
        private static string GetId3InfoLine(string file)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("{0}\t", file);

            using (var stream = File.OpenRead(file))
            using (var mp3 = new Mp3Stream(stream))
            {
                try
                {
                    var tag = mp3.HasTagOfFamily(Id3TagFamily.FileStartTag) ? mp3.GetTag(Id3TagFamily.FileStartTag) : null;
                    if (tag == null) builder.Append("null\tnull\tnull\tnull\tnull\tnull");
                    else
                    {
                        builder.AppendFormat("{0}\t", Format(tag.Genre));
                        builder.AppendFormat("{0}\t", Format(tag.Artists));
                        builder.AppendFormat("{0}\t", Format(tag.Title));
                        builder.AppendFormat("{0}\t", Format(tag.Album));
                        builder.AppendFormat("{0}\t", Format(tag.Year));
                        builder.AppendFormat("{0}", Format(tag.Track));
                    }
                }
                catch (Exception ex)
                {
                    builder.Append(ex.Message);
                }
            }
            return builder.ToString();
        }*/

        #endregion
    }
}