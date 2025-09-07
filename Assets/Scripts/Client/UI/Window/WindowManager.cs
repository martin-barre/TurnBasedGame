public class WindowManager : MonoSingleton<WindowManager>
{
    public T CreateWindow<T>(T windowUI) where T : WindowUI
    {
        return Instantiate(windowUI, gameObject.transform);
    }
}
