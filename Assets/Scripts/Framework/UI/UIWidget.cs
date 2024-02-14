using System.Threading.Tasks;

namespace Framework.UI
{
    public class UIWidget : UIView
    {
        public sealed override Task StartShow(bool isPlayAnimation = true)
        {
            return base.StartShow(isPlayAnimation);
        }
        
        public sealed override Task StartHide(bool isPlayAnimation = true)
        {
            return base.StartHide(isPlayAnimation);
        }
    }
}