/*******************************************************************************************************************************
 * AK.RockCon.Stripper
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
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

#endregion

namespace AK.RockCon
{
    /// <summary>
    /// Strips MP3 tag information from MP3 files.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IStripper
    {
        void Strip();
        void StripFile(string file);
    }

    /// <summary>
    /// Strips MP3 tag information from MP3 files.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (IStripper)), PartCreationPolicy(CreationPolicy.Shared)]
    public class Stripper : IStripper
    {
        private const string SearchPattern = "*.mp3";

        private readonly string mediaPath;

        [ImportingConstructor]
        public Stripper([Import] IConfiguration configuration)
        {
            this.mediaPath = configuration.MediaPath;
        }

        public void Strip()
        {
            var files = Directory.GetFiles(this.mediaPath, SearchPattern, SearchOption.AllDirectories);
            Parallel.ForEach(files, this.StripFile);
        }

        public void StripFile(string file)
        {
            byte[] audioData;
            using (var stream = File.OpenRead(file))
            using (var mp3Stream = new Mp3Stream(stream))
            {
                audioData = mp3Stream.GetAudioStream();
            }
            File.WriteAllBytes(file, audioData);
        }
    }
}