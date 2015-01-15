/*******************************************************************************************************************************
 * AK.RockCon.Model
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

using AK.RockCon.Commands;
using System.Collections.Generic;

#endregion

namespace AK.RockCon
{
    /// <summary>
    /// Represents the current state of the application as needed by a view.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class Model
    {
        public PlayerState PlayerState { get; set; }
        public Item PreviousItem { get; set; }
        public Item CurrentItem { get; set; }
        public Item NextItem { get; set; }
        public IEnumerable<ICommand> Commands { get; set; }
        public bool RefreshView { get; set; }
    }
}