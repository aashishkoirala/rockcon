/*******************************************************************************************************************************
 * AK.RockCon.Commands.ControlCommand
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
    /// Special type of command for control-type functions.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class ControlCommand : CommandBase
    {
        public ControlCommand(ControlCommandType type)
        {
            this.Type = type;
            this.NeedsViewRefresh =
                !(type == ControlCommandType.IncreaseVolume || type == ControlCommandType.DecreaseVolume);
        }

        public ControlCommandType Type { get; private set; }

        public bool NeedsViewRefresh { get; private set; }

        public override void Execute(IContainer container)
        {
            var player = container.Player;
            var playList = container.PlayList;
            Item current;

            switch (this.Type)
            {
                case ControlCommandType.TogglePlayPause:
                    if (player.State == PlayerState.Playing) player.Pause();
                    else player.Play();
                    return;

                case ControlCommandType.Stop:
                    player.Stop();
                    return;

                case ControlCommandType.IncreaseVolume:
                    player.IncreaseVolume();
                    return;

                case ControlCommandType.DecreaseVolume:
                    player.DecreaseVolume();
                    return;

                case ControlCommandType.SkipToPrevious:
                    playList.MovePrevious();
                    if ((current = playList.Current) != null)
                    {
                        player.Path = current.FilePath;
                        player.Play();
                    }
                    return;

                case ControlCommandType.SkipToNext:
                    playList.MoveNext();
                    if ((current = playList.Current) != null)
                    {
                        player.Path = current.FilePath;
                        player.Play();
                    }
                    return;

                case ControlCommandType.Quit:
                    container.View.Close();
                    return;
            }
        }
    }
}