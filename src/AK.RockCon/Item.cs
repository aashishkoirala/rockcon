/*******************************************************************************************************************************
 * AK.RockCon.Item
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

namespace AK.RockCon
{
    /// <summary>
    /// Represents a single music item (song/track).
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class Item
    {
        public string FilePath { get; set; }
        public string Genre { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public int? Year { get; set; }
        public int? TrackNumber { get; set; }
        public bool IsPartOfCompilation { get; set; }
    }
}