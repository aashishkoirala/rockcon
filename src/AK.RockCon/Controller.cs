/*******************************************************************************************************************************
 * AK.RockCon.Controller
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
using System.ComponentModel.Composition;
using System.Linq;

#endregion

namespace AK.RockCon
{
    /// <summary>
    /// Executes the main application flow.
    /// </summary>
    /// <remarks>
    /// See remarks on <see cref="IView">IView</see> for information on how the controller works with different types of
    /// views.
    /// </remarks>
    /// <author>Aashish Koirala</author>
    public interface IController
    {
        void Execute();
    }

    /// <summary>
    /// Executes the main application flow.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (IController)), PartCreationPolicy(CreationPolicy.Shared)]
    public class Controller : IController
    {
        private readonly IContainer container;
        private readonly ICommand startupCommand;

        [ImportingConstructor]
        public Controller([Import] IContainer container) : this(container, null)
        {
        }

        public Controller(IContainer container, ICommand startupCommand)
        {
            this.container = container;
            this.startupCommand = startupCommand ?? new MainMenuCommand(container);
        }

        public void Execute()
        {
            if (this.container.View.SupportsAwaitCommand) this.ExecuteAwait();
            else this.ExecuteNoAwait();
        }

        private void ExecuteAwait()
        {
            var quit = false;
            var command = this.startupCommand;

            this.ExecuteCommand(command);

            while (!quit)
            {
                command = this.container.View.AwaitCommand();
                if (command == null) continue;

                quit = command is ControlCommand && (command as ControlCommand).Type == ControlCommandType.Quit;

                this.ExecuteCommand(command);
            }
        }

        private void ExecuteNoAwait()
        {
            this.container.View.SelectCommandAction = this.ExecuteCommand;
            this.ExecuteCommand(this.startupCommand);
        }

        private void ExecuteCommand(ICommand command)
        {
            try
            {
                command.Execute(this.container);
            }
            catch (Exception ex)
            {
                this.container.View.SendMessage(ex.Message);
            }

            var controlCommand = command as ControlCommand;
            var refreshView = controlCommand == null || controlCommand.NeedsViewRefresh;

            var model = new Model
                {
                    PlayerState = this.container.Player.State,
                    CurrentItem = this.container.PlayList.Current,
                    NextItem = this.container.PlayList.Next,
                    PreviousItem = this.container.PlayList.Previous,
                    Commands = command.Commands.Any() ? command.Commands : null,
                    RefreshView = refreshView
                };

            this.container.View.Update(model);
        }
    }
}