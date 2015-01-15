/*******************************************************************************************************************************
 * AK.RockCon.Container
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

using System.ComponentModel.Composition;

namespace AK.RockCon
{
    /// <summary>
    /// Provides access to application level services.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IContainer
    {
        IItemRepository ItemRepository { get; }
        IScanner Scanner { get; }
        IStripper Stripper { get; }
        ISynchronizer Synchronizer { get; }
        IPlayer Player { get; }
        IPlayList PlayList { get; }
        IConfiguration Configuration { get; }
        IView View { get; }
    }

    /// <summary>
    /// Provides access to application level services.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (IContainer)), PartCreationPolicy(CreationPolicy.Shared)]
    public class Container : IContainer
    {
        [ImportingConstructor]
        public Container(
            [Import] IItemRepository itemRepository,
            [Import] IScanner scanner,
            [Import] IStripper stripper,
            [Import] ISynchronizer synchronizer,
            [Import] IPlayer player,
            [Import] IPlayList playList,
            [Import] IConfiguration configuration,
            [Import] IView view)
        {
            this.ItemRepository = itemRepository;
            this.Scanner = scanner;
            this.Stripper = stripper;
            this.Synchronizer = synchronizer;
            this.Player = player;
            this.PlayList = playList;
            this.Configuration = configuration;
            this.View = view;
        }

        public IItemRepository ItemRepository { get; private set; }
        public IScanner Scanner { get; private set; }
        public IStripper Stripper { get; private set; }
        public ISynchronizer Synchronizer { get; private set; }
        public IPlayer Player { get; private set; }
        public IPlayList PlayList { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public IView View { get; private set; }
    }
}