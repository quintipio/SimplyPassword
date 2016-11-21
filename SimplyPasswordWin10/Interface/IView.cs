using SimplyPasswordWin10.Abstract;

namespace SimplyPasswordWin10.Interface
{
    public interface IView<T> where T:AbstractViewModel
    {
        T ViewModel { get; set; }
    }
}
