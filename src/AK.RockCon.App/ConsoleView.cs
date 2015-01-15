/*******************************************************************************************************************************
 * AK.RockCon.App.ConsoleView
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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

#endregion

namespace AK.RockCon.App
{
    /// <summary>
    /// Implementation of IView that uses the system console for UI.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (IView)), PartCreationPolicy(CreationPolicy.Shared)]
    public class ConsoleView : ViewBase
    {
        #region Fields

        private static readonly ConsoleKey[] keys = EnumerateAllValidConsoleKeys().ToArray();

        private static readonly IDictionary<ConsoleKey, ControlCommandType> keyControlCommandMap = new Dictionary
            <ConsoleKey, ControlCommandType>
            {
                {ConsoleKey.Escape, ControlCommandType.Quit},
                {ConsoleKey.Spacebar, ControlCommandType.TogglePlayPause},
                {ConsoleKey.F10, ControlCommandType.Stop},
                {ConsoleKey.UpArrow, ControlCommandType.IncreaseVolume},
                {ConsoleKey.DownArrow, ControlCommandType.DecreaseVolume},
                {ConsoleKey.LeftArrow, ControlCommandType.SkipToPrevious},
                {ConsoleKey.RightArrow, ControlCommandType.SkipToNext}
            };

        private readonly IDictionary<ConsoleKey, Action> keyViewActionMap;
        private readonly int menuCount;
        private readonly Stack<MenuState> menuStateHistoryStack = new Stack<MenuState>();
        private readonly Queue<string> messageQueue = new Queue<string>();

        private MenuState currentMenuState;
        private Model model;

        #endregion

        #region Constructor

        [ImportingConstructor]
        public ConsoleView([Import] IConfiguration configuration)
        {
            this.menuCount = configuration.MenuCount;
            if (this.menuCount > keys.Length) this.menuCount = keys.Length;

            this.keyViewActionMap = new Dictionary<ConsoleKey, Action>
                {
                    {ConsoleKey.Backspace, this.GoBack},
                    {ConsoleKey.PageDown, this.PageDown},
                    {ConsoleKey.PageUp, this.PageUp}
                };
        }

        #endregion

        #region IView/ViewBase

        public override bool SupportsAwaitCommand
        {
            get { return true; }
        }

        public override void Update(Model modelToUpdateWith)
        {
            this.model = modelToUpdateWith;

            if (this.model.Commands != null)
            {
                if (this.currentMenuState != null) this.menuStateHistoryStack.Push(this.currentMenuState);
                this.currentMenuState = new MenuState {Commands = this.model.Commands.ToArray()};
                this.UpdateMenuItemsFromCommands();
            }

            if (!this.model.RefreshView) return;

            this.WriteModelState();
        }

        public override ICommand AwaitCommand()
        {
            MenuItem menuItem;
            var keyInfo = Console.ReadKey(true);

            var command = this.HandleControlCommand(keyInfo.Key);
            if (command != null) return command;

            if (!this.currentMenuState.MenuItems.TryGetValue(keyInfo.Key, out menuItem)) return null;

            WriteLine(ConsoleColor.Gray, string.Empty);
            return menuItem.Command;
        }

        public override void Close()
        {
            Console.Clear();
        }

        protected override void SendMessageInternal(string message)
        {
            WriteLine(ConsoleColor.Gray, message);
            this.messageQueue.Enqueue(message);
        }

        #endregion

        #region Menu Construction

        private void UpdateMenuItemsFromCommands()
        {
            this.currentMenuState.MenuItems = this
                .currentMenuState.Commands
                .Skip(this.currentMenuState.StartIndex)
                .Take(this.menuCount)
                .Select(CreateMenuItemForCommand)
                .ToDictionary(x => x.Key);
        }

        private static MenuItem CreateMenuItemForCommand(ICommand command, int index)
        {
            var controlCommand = command as ControlCommand;
            if (controlCommand != null && controlCommand.Type == ControlCommandType.VisualDivider)
            {
                return new MenuItem
                    {
                        IsVisualDivider = true, 
                        
                        // Need something unique that will never be pressed.
                        //
                        Key = (ConsoleKey) (1000*index)
                    };
            }

            return new MenuItem
                {
                    Key = keys[index],
                    Command = command,
                    KeyName = GetDisplayNameForConsoleKey(keys[index])
                };
        }

        private static IEnumerable<ConsoleKey> EnumerateAllValidConsoleKeys()
        {
            var keyNumber = (int) ConsoleKey.D1;
            for (; keyNumber <= (int) ConsoleKey.D9; keyNumber++)
                yield return (ConsoleKey) keyNumber;
            yield return ConsoleKey.D0;

            keyNumber = 'A';
            for (; keyNumber <= (int) 'Z'; keyNumber++)
                yield return (ConsoleKey) keyNumber;
        }

        private static string GetDisplayNameForConsoleKey(ConsoleKey key)
        {
            return key >= ConsoleKey.D0 && key <= ConsoleKey.D9
                       ? ((int) key - (int) ConsoleKey.D0).ToString()
                       : key.ToString();
        }

        #endregion

        #region Control Commands

        private ICommand HandleControlCommand(ConsoleKey key)
        {
            Action action;
            if (this.keyViewActionMap.TryGetValue(key, out action))
            {
                action();
                return null;
            }

            ControlCommandType type;
            return keyControlCommandMap.TryGetValue(key, out type) ? new ControlCommand(type) : null;
        }

        private void GoBack()
        {
            if (!this.menuStateHistoryStack.Any()) return;

            var previousState = this.menuStateHistoryStack.Pop();
            this.currentMenuState = previousState;
            this.WriteModelState();
        }

        private void PageDown()
        {
            if (this.currentMenuState.StartIndex + this.menuCount >= this.currentMenuState.Commands.Length)
                return;

            this.currentMenuState.StartIndex += this.menuCount;
            this.UpdateMenuItemsFromCommands();
            this.WriteModelState();
        }

        private void PageUp()
        {
            if (this.currentMenuState.StartIndex == 0) return;

            this.currentMenuState.StartIndex -= this.menuCount;
            this.UpdateMenuItemsFromCommands();
            this.WriteModelState();
        }

        #endregion

        #region Writers

        private void WriteModelState()
        {
            var currentSong =
                this.model.CurrentItem == null
                    ? "Nothing is playing"
                    : string.Format("{0} - {1}", this.model.CurrentItem.Artist, this.model.CurrentItem.Title);

            var nextSong =
                this.model.NextItem == null
                    ? "Nothing is queued"
                    : string.Format("{0} - {1}", this.model.NextItem.Artist, this.model.NextItem.Title);

            Console.Clear();
            Write(ConsoleColor.Cyan, "ROCKCON ");
            WriteLine(ConsoleColor.Gray, "by Aashish Koirala");
            WriteDividerLine();

            WriteLine(ConsoleColor.Green, "*** {0} ***{1}",
                      this.model.PlayerState.ToString().ToUpper(), Environment.NewLine);
            Write(ConsoleColor.Gray, "Now playing: ");
            WriteLine(ConsoleColor.Yellow, currentSong);

            Write(ConsoleColor.Gray, "Up next: ");
            WriteLine(ConsoleColor.DarkYellow, nextSong);
            WriteDividerLine();

            var queueHasMessages = false;
            while (this.messageQueue.Any())
            {
                queueHasMessages = true;
                var message = this.messageQueue.Dequeue();
                WriteLine(ConsoleColor.Gray, message);
            }

            if (queueHasMessages) WriteDividerLine();

            foreach (var menuItem in this.currentMenuState.MenuItems.Values)
            {
                if (menuItem.IsVisualDivider) WriteDividerLine();
                else
                {
                    Write(ConsoleColor.White, menuItem.KeyName);
                    WriteLine(ConsoleColor.Gray, " - {0}", menuItem.Command.Name);
                }
            }

            WriteDividerLine();

            WriteKeyLegend("ESC", "Quit");
            if (this.menuStateHistoryStack.Any()) WriteKeyLegend("Backspace", "Go Back");

            if (this.model.PlayerState == PlayerState.Playing)
            {
                WriteKeyLegend("Spacebar", "Pause");
                WriteKeyLegend("F10", "Stop");
                WriteKeyLegend("Up", "Turn Up Volume");
                WriteKeyLegend("Down", "Turn Down Volume");
                WriteKeyLegend("Left", "Play Previous");
                WriteKeyLegend("Right", "Play Next");
            }

            if (this.model.PlayerState == PlayerState.Paused || this.model.PlayerState == PlayerState.Stopped)
                WriteKeyLegend("Spacebar", "Play");

            if (this.currentMenuState.Commands.Length > this.menuCount &&
                (this.currentMenuState.StartIndex + this.menuCount < this.currentMenuState.Commands.Length))
            {
                WriteKeyLegend("PgDn", "Next Page");
            }

            if (this.currentMenuState.Commands.Length > this.menuCount &&
                (this.currentMenuState.StartIndex > 0))
            {
                WriteKeyLegend("PgUp", "Prev Page");
            }
        }

        private static void WriteDividerLine()
        {
            WriteLine(ConsoleColor.Gray, new string('─', 70));
        }

        private static void WriteKeyLegend(string key, string action)
        {
            Write(ConsoleColor.DarkYellow, "[");
            Write(ConsoleColor.Yellow, key);
            Write(ConsoleColor.DarkYellow, " - {0}] ", action);
        }

        private static void Write(ConsoleColor color, string message, params object[] args)
        {
            var currentColor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = color;
                Console.Write(message, args);
            }
            finally
            {
                Console.ForegroundColor = currentColor;
            }
        }

        private static void WriteLine(ConsoleColor color, string message, params object[] args)
        {
            var currentColor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message, args);
            }
            finally
            {
                Console.ForegroundColor = currentColor;
            }
        }

        #endregion
    }
}