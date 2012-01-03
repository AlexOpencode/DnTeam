using System.ComponentModel;

namespace DnTeam
{
    public class LocalizedDisplayAttribute : DisplayNameAttribute
    {
        public LocalizedDisplayAttribute(string displayNameKey) : base(displayNameKey) {}
        
        public override string DisplayName
        {
            get { return Resources.Labels.ResourceManager.GetString(base.DisplayName); }
        }
    }
}