using Etra.StarterAssets.Items;
using System.Linq;
using System.Text.RegularExpressions;

namespace Etra.StarterAssets
{
    public class ItemScriptAndNameHolder 
    {
        public EtraFPSUsableItemBaseClass script;
        public string name;
        public string shortenedName;
        public bool state;

        public ItemScriptAndNameHolder(EtraFPSUsableItemBaseClass script)
        {
            this.script = script;
            state = false;
            name = script.GetType().Name;
            GenerateName();
        }

        public void GenerateName()
        {
            shortenedName = script.GetType().Name.Split('_').Last();
            shortenedName = Regex.Replace(shortenedName, "([a-z])([A-Z])", "$1 $2");
        }
    }
}
