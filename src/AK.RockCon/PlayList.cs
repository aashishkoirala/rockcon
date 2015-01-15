/*******************************************************************************************************************************
 * AK.RockCon.PlayList
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
using System.ComponentModel.Composition;
using System.Linq;

#endregion

namespace AK.RockCon
{
    /// <summary>
    /// Maintains a playlist of music items.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IPlayList
    {
        Item[] Items { get; }
        Item Previous { get; }
        Item Current { get; }
        Item Next { get; }

        void Clear();
        void Reset();
        void Enqueue(Item item);
        void Insert(Item item);
        void InsertAfterCurrent(Item item);
        void MoveNext();
        void MovePrevious();
    }

    /// <summary>
    /// Maintains a playlist of music items.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (IPlayList)), PartCreationPolicy(CreationPolicy.Shared)]
    public class PlayList : IPlayList
    {
        private readonly IList<Item> items = new List<Item>();
        private int index;

        public Item[] Items
        {
            get { return this.items.ToArray(); }
        }

        public Item Previous
        {
            get { return (this.index - 1 < this.items.Count && this.index - 1 >= 0) ? this.items[this.index - 1] : null; }
        }

        public Item Current
        {
            get { return (this.index < this.items.Count) ? this.items[this.index] : null; }
        }

        public Item Next
        {
            get { return (this.index + 1 < this.items.Count) ? this.items[this.index + 1] : null; }
        }

        public void Clear()
        {
            this.Reset();
            this.items.Clear();
        }

        public void Reset()
        {
            this.index = 0;
        }

        public void Enqueue(Item item)
        {
            this.items.Add(item);
        }

        public void Insert(Item item)
        {
            this.items.Insert(0, item);
        }

        public void InsertAfterCurrent(Item item)
        {
            var position = this.items.Any() ? this.index : 0;
            this.items.Insert(position, item);
        }

        public void MoveNext()
        {
            if (this.index == this.items.Count - 1) return;
            this.index++;
        }

        public void MovePrevious()
        {
            if (this.index == 0) return;
            this.index--;
        }
    }
}