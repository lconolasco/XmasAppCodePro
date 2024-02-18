using XmasAppCodePro.Views;

namespace XmasAppCodePro
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            RegisterForRoute<AggiungeProdottoPage>();
            RegisterForRoute<AggiornaProdottoPage>();
            RegisterForRoute<AggiungeCategoriaPage>();
            RegisterForRoute<AggiornaCategoriaPage>();
        }

        protected void RegisterForRoute<T>()
        {
            Routing.RegisterRoute(typeof(T).Name, typeof(T));
        }
    }
}
