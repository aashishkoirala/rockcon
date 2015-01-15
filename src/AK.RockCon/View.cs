/*******************************************************************************************************************************
 * AK.RockCon.View
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
using System;

#endregion

namespace AK.RockCon
{
    /// <summary>
    /// Represents a view that will handle UI for the application.
    /// </summary>
    /// <remarks>
    /// The idea here is to make the library unaware of the type of UI. An implementation of IView could be
    /// a console view, a WinForm view, a WPF view, whatever. A console view would be fundamentally different in that
    /// it would need to act synchronously and in a blocking way as opposed to GUI view that is event driven and uses
    /// the Windows message loop. Thus, a console view would have SupportsAwaitCommand set to True and would implement
    /// the AwaitCommand method, whereas others would have it False and call the SelectCommandAction when the user
    /// triggers a command.
    /// </remarks>
    /// <author>Aashish Koirala</author>
    public interface IView
    {
        void SendMessage(string messageFormat, params object[] args);
        void Update(Model model);
        void Close();
        bool SupportsAwaitCommand { get; }
        ICommand AwaitCommand();
        Action<ICommand> SelectCommandAction { get; set; }
    }

    /// <summary>
    /// Represents a view that will handle UI for the application.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public abstract class ViewBase : IView
    {
        public abstract bool SupportsAwaitCommand { get; }

        public Action<ICommand> SelectCommandAction { get; set; }

        public abstract void Update(Model model);

        public void SendMessage(string messageFormat, params object[] args)
        {
            this.SendMessageInternal(string.Format(messageFormat, args));
        }

        public virtual ICommand AwaitCommand()
        {
            if (!this.SupportsAwaitCommand)
                throw new InvalidOperationException("The view does not support waiting for commands.");

            return null;
        }

        public virtual void Close()
        {
        }

        protected abstract void SendMessageInternal(string message);

        protected void SelectCommand(ICommand command)
        {
            if (this.SupportsAwaitCommand)
            {
                throw new InvalidOperationException(
                    "The view supports waiting for commands, " +
                    "thus use the synchronous AwaitCommand method instead of this.");
            }

            if (this.SelectCommandAction == null) return;
            this.SelectCommandAction(command);
        }
    }
}