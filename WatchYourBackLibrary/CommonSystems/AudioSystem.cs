using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace WatchYourBackLibrary
{
    public class AudioSystem : ESystem
    {
        private SongCollection songList;

        public AudioSystem()
            : base(false, true, 11)
        {
            components += (int)Masks.Audio;
            songList = new SongCollection();
            MediaPlayer.IsRepeating = true;
        }

        public override void update(TimeSpan gameTime)
        {
            foreach(Entity e in activeEntities)
            {
                if (e.hasComponent(Masks.SoundEffect))
                {
                    SoundEffectComponent audio = (SoundEffectComponent)e.Components[Masks.SoundEffect];

                    if (audio.Played == false)
                        audio.Sound.Play();
                    else
                        if (audio.Sound.State == SoundState.Stopped)
                            manager.removeEntity(e);
                }
                else if(e.hasComponent(Masks.Song))
                {
                    SongComponent audio = (SongComponent)e.Components[Masks.Song];
                    if (!songList.Contains(audio.Song))
                    {
                        songList.Add(audio.Song);
                        if (MediaPlayer.State == MediaState.Stopped)
                            MediaPlayer.Play(songList);
                    }
                    manager.removeEntity(e);
                }
            }
            
        }
    }
}
