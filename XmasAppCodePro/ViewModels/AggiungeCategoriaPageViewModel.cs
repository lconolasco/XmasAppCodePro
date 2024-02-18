using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using XmasAppCodePro.Models;

namespace XmasAppCodePro.ViewModels
{
    public partial class AggiungeCategoriaPageViewModel : ObservableObject
    {
        //Servizio di connessione per il consumo della REST API
        readonly HttpClient client;

        //Configurazione JSON
        readonly JsonSerializerOptions _serializerOptions;

        //Definiçao da URL base
        readonly string baseUrl = "https://lconolasco.somee.com/api";

        [ObservableProperty]
        public int _id;

        [ObservableProperty]
        public string _nome;

        [ObservableProperty]
        public int _CategoriaId;


        [ObservableProperty]
        public ObservableCollection<Categoria> _listaCategorie;

        [ObservableProperty]
        public string _message;

        public AggiungeCategoriaPageViewModel()
        {
            //L'instanza di HttpClient
            client = new HttpClient();

            //Instancia da Coleçao de objetos
            ListaCategorie = new ObservableCollection<Categoria>();

            //Configuraçao da serializaçao do JSON
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            _ = RicuperaCategorieAsync();
        }

        //retorna a coleçao de categorias
        [RelayCommand]
        private async Task RicuperaCategorieAsync()
        {
            ListaCategorie.Clear();
            try
            {
                var url = $"{baseUrl}/categoria";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    using var responseStream = await response.Content.ReadAsStreamAsync();
                    var data = await JsonSerializer.DeserializeAsync<ObservableCollection<Categoria>>(responseStream, _serializerOptions);
                    ListaCategorie = data;
                }
                else
                {
                    Message = $"Tentativo di conessione al database falita.";

                }
                return;
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Errore", e.Message, "Ok");
            }
        }

        [RelayCommand]
        private async Task AggingeCategoriaAsync()
        {
            Message = string.Empty;
            try
            {
                var url = $"{baseUrl}/categoria";
                if(Nome is not null)
                {
                    var categoria = new Categoria
                    {
                        Nome = this.Nome
                    };

                    string categoriaJson = JsonSerializer.Serialize<Categoria>(categoria, _serializerOptions);
                    StringContent content = new(categoriaJson, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Nome = String.Empty;
                        await RicuperaCategorieAsync();
                        return;
                    }
                     else
                    {
                        Message = response.ToString();
                        await Shell.Current.DisplayAlert("Error", Message, "Ok");
                    }
                }
                else
                {
                    Message = $"Tutti Campi sono obbligatori";
                    await Shell.Current.DisplayAlert("Error", Message, "Ok");
                }
                return;
            }
            catch (Exception e)
            {
                Message = e.Message;
                await Shell.Current.DisplayAlert("Errore", Message, "Ok");
            }
            return;
        } 
    }
}
