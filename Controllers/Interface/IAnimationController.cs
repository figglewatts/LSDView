using libLSD.Formats;
using LSDView.Models;

namespace LSDView.Controllers.Interface
{
    public interface IAnimationController
    {
        TOD Animation { get; }
        MOMDocument Focus { get; }
        bool Active { get; }
        int CurrentFrame { get; }

        void SetFocus(MOMDocument mom, int animationIdx = 0);
        void Update(double dt);
    }
}
