using Honeylab.Sounds.Data;
using Honeylab.Utils.Prefs;


namespace Honeylab.Sounds
{
    public class SoundService : SoundServiceBase
    {
        public SoundService(SoundsData data, SoundServiceParams p, IPrefsService prefsService) :
            base(data, p, prefsService) { }
    }
}
