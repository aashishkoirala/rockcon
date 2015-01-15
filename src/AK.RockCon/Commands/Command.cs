/*******************************************************************************************************************************
 * AK.RockCon.Commands.Command
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
    /// Represents any command that can get executed and also spawn its list of sub-commands.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface ICommand
    {
        ICommand[] Commands { get; }
        string Name { get; }

        void Execute(IContainer container);
    }

    /// <summary>
    /// Common functionality for all commands.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public abstract class CommandBase : ICommand
    {
        protected CommandBase()
        {
            this.Commands = this.Commands ?? new ICommand[0];
        }

        public ICommand[] Commands { get; protected set; }

        public string Name { get; protected set; }

        public virtual void Execute(IContainer container)
        {
        }
    }
}