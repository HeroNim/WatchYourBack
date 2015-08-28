using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace WatchYourBackLibrary
{
    public class AudioSystem : ESystem
    {
        private SongCollection songList;
        private List<SoundEffectInstance> sounds;
        private ContentManager content;

        public AudioSystem(ContentManager content)
            : base(false, true, 21)
        {
            this.content = content;
            components += (int)Masks.Audio;
            songList = new SongCollection();
            sounds = new List<SoundEffectInstance>();
            MediaPlayer.IsRepeating = true;           
        }

        public override void Initialize(IECSManager manager)
        {
            base.Initialize(manager);
            foreach (ESystem system in manager.Systems)
                system.inputFired += new EventHandler(EventListener);
        }

        public override void Update(TimeSpan gameTime)
        {  
            foreach (SoundEffectInstance audio in sounds)
                if (audio.State == SoundState.Stopped)
                    audio.Dispose();

            //if (songList.Count != 0 && MediaPlayer.State == MediaState.Stopped)
            //    MediaPlayer.Play(songList);
        }

        public SongCollection Songs
        {
            get { return songList; }
            set { songList = value; }
        }

        public override void EventListener(object sender, EventArgs e)
        {
            if (e is SoundArgs)
            {
                SoundArgs s = (SoundArgs)e;
                SoundEffectInstance sound = content.Load<SoundEffect>(s.FileName).CreateInstance();
                if (s.Loop == true)
                    sound.IsLooped = true;
                sounds.Add(sound);
                sound.Play();
            }
        }
    }
}
