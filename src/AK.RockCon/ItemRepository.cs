/*******************************************************************************************************************************
 * AK.RockCon.ItemRepository
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

using Newtonsoft.Json;
using System.ComponentModel.Composition;
using System.IO;

#endregion

namespace AK.RockCon
{
    /// <summary>
    /// Maintains a persisted list of music items.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IItemRepository
    {
        bool Exists { get; }
        Item[] Items { get; set; }
    }

    /// <summary>
    /// Maintains a persisted list of music items.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (IItemRepository)), PartCreationPolicy(CreationPolicy.Shared)]
    public class ItemRepository : IItemRepository
    {
        private readonly string path;
        private Item[] items;

        [ImportingConstructor]
        public ItemRepository([Import] IConfiguration configuration)
        {
            this.path = configuration.RepositoryFilePath;
        }

        public bool Exists
        {
            get { return File.Exists(this.path); }
        }

        public Item[] Items
        {
            get { return this.items ?? (this.items = this.Read()); }
            set { this.Write(value); }
        }

        private Item[] Read()
        {
            var jsonSerializer = new JsonSerializer();

            using (var reader = new StreamReader(this.path))
            using (var jtr = new JsonTextReader(reader))
            {
                this.items = jsonSerializer.Deserialize<Item[]>(jtr);
            }
            return this.items;
        }

        private void Write(Item[] itemsToWrite)
        {
            var jsonSerializer = new JsonSerializer {Formatting = Formatting.Indented};

            using (var writer = new StreamWriter(this.path, false))
            using (var jtr = new JsonTextWriter(writer))
            {
                jsonSerializer.Serialize(jtr, itemsToWrite);
            }
            this.items = itemsToWrite;
        }
    }
}