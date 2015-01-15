/*******************************************************************************************************************************
 * AK.RockCon.Player
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

using System.ComponentModel.Composition;
using WMPLib;

#endregion

namespace AK.RockCon
{
    /// <summary>
    /// Provides access to the music player.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IPlayer
    {
        string Path { get; set; }
        PlayerState State { get; }

        void Play();
        void Pause();
        void Stop();
        void IncreaseVolume();
        void DecreaseVolume();
    }

    /// <summary>
    /// Provides access to the music player.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (IPlayer)), PartCreationPolicy(CreationPolicy.Shared)]
    public class Player : IPlayer
    {
        private readonly WindowsMediaPlayer player = new WindowsMediaPlayer();

        public string Path
        {
            get { return this.player.URL; }
            set
            {
                this.player.URL = value;
                this.player.controls.stop();
            }
        }

        public PlayerState State { get; private set; }

        public void Play()
        {
            this.player.controls.play();
            this.State = PlayerState.Playing;
        }

        public void Pause()
        {
            this.player.controls.pause();
            this.State = PlayerState.Paused;
        }

        public void Stop()
        {
            this.player.controls.stop();
            this.State = PlayerState.Stopped;
        }

        public void IncreaseVolume()
        {
            if (this.player.settings.volume < 100)
                this.player.settings.volume++;
        }

        public void DecreaseVolume()
        {
            if (this.player.settings.volume > 0)
                this.player.settings.volume--;
        }
    }
}