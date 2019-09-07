namespace Seega.Scripts.Core
{
    public interface ISelectable
    {
        void OnSelect();
        void OnDeselect();
        bool IsSelected();
    }
}