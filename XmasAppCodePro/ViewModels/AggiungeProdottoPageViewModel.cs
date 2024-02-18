using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using XmasAppCodePro.Models;

namespace XmasAppCodePro.ViewModels
{
    [QueryProperty(nameof(Prodotto), "ProdottoObject")]


    public partial class AggiungeProdottoPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public Categoria _categoriaSelezionata;

        [ObservableProperty]
        private Prodotto _prodotto;

        //Servizio di connessione per il consumo della REST API
        readonly HttpClient client;

        //Configurazione JSON
        readonly JsonSerializerOptions _serializerOptions;

        //Definiçao da URL base
        readonly string baseUrl = "https://lconolasco.somee.com/api";


        [ObservableProperty]
        public int _id;

        [ObservableProperty]
        public string _barcode;

        [ObservableProperty]
        public string _nome;

        [ObservableProperty]
        public string _descrizione;

        [ObservableProperty]
        public string _imageUrl;

        [ObservableProperty]
        public decimal _prezzo;

        [ObservableProperty]
        public decimal _pesoLordo;

        [ObservableProperty]
        public int _CategoriaId;

        [ObservableProperty]
        public int _quantita;

        [ObservableProperty]
        public ObservableCollection<Prodotto> _catalogoProdotti;

        [ObservableProperty]
        public ObservableCollection<Categoria> _listaCategorie;

        [ObservableProperty]
        public string _message;

        public AggiungeProdottoPageViewModel()
        {
            //L'instanza di HttpClient
            client = new HttpClient();

            //Instancia da Coleçao de objetos
            CatalogoProdotti = new ObservableCollection<Prodotto>();

            ListaCategorie = new ObservableCollection<Categoria>();

            //Configuraçao da serializaçao do JSON
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            _=RicuperaCategorieAsync();
        }

        //** Consumo da REST API **//

        //Insere um novo produto na tabela do DB
        public ICommand AggiungeProdotto =>
            new Command(async () => await AggiungeProdottoAsync());

        private async Task AggiungeProdottoAsync()
        {
            var url = $"{baseUrl}/Prodotto";

            try
            {
                if (Barcode is not null && Nome is not null && Descrizione is not null && Prezzo >= 0 && PesoLordo >= 0 && Quantita >= 0)
                {
                    var prodotto = new Prodotto
                    {
                        Barcode = this.Barcode,
                        Nome = this.Nome,
                        Descrizione = this.Descrizione,
                        ImageUrl = this.ImageUrl,
                        Prezzo = this.Prezzo,
                        PesoLordo = this.PesoLordo,
                        Quantita = this.Quantita,
                        CategoriaId = this.CategoriaId

                    };
                    string prodottoJson = JsonSerializer.Serialize<Prodotto>(prodotto, _serializerOptions);
                    StringContent content = new(prodottoJson, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Barcode = string.Empty;
                        Nome = string.Empty;
                        Descrizione = string.Empty;
                        ImageUrl = string.Empty;
                        Prezzo = 0;
                        PesoLordo = 0;
                        Quantita = 0;
                        CategoriaId = 0;
                        _ = Shell.Current.GoToAsync("MainPage");
                    }
                    else
                    {
                        var Message = response.ToString();
                        await Shell.Current.DisplayAlert("Error", Message, "Ok");
                        Prodotto vuoto = new()
                        {
                            Nome = Message
                        };
                    }
                }

                else
                {
                    var Message = $"Tutti Campi sono obbligatori";
                    _ = Shell.Current.DisplayAlert("Error", Message, "Ok");
                    Prodotto vuoto = new()
                    {
                        Nome = Message
                    };
                }
                return;
            }
            catch (Exception ex)
            {
                var Message = ex.Message;
                _ = Shell.Current.DisplayAlert("Errore", Message, "Ok");
            }
            return;
        }

        //retorna a coleçao de categorias
        public ICommand RicuperaCategorie =>
            new Command(async () => await RicuperaCategorieAsync());

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
        public ICommand RicuperaProdotto => new Command(async () => await RicuperaProdottoAsync());

        private async Task RicuperaProdottoAsync()
        {
            Message = string.Empty;
            try
            {
                CatalogoProdotti.Clear();
                if (Barcode is not null)
                {
                    var url = $"{baseUrl}/Prodotto/{Barcode}";
                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        using var responseStream = await response.Content.ReadAsStreamAsync();
                        var data = await JsonSerializer.DeserializeAsync<Prodotto>(responseStream, _serializerOptions);
                        this.Prodotto = data;
                        Nome = data.Nome;
                        Descrizione = data.Descrizione;
                        ImageUrl = data.ImageUrl;
                        Prezzo = data.Prezzo;
                        PesoLordo = data.PesoLordo;
                        Quantita = data.Quantita;
                        CategoriaId = data.CategoriaId;

                        CatalogoProdotti.Add(data);

                    }
                    else
                    {
                        Message = $"Nessuno Prodotto trovato con questo codice.Certifica che questo prodotto sia inserito nel Database in precedenza.";
                    }
                }
            }
            catch (Exception e)
            {

                Message = "Connessione falita col Database. Ricontrolla le conessione e riprova.";
                Prodotto vuoto = new()
                {
                    Nome = Message,
                    Descrizione = e.Message
                };
                CatalogoProdotti.Add(vuoto);
                await Shell.Current.DisplayAlert("Avviso", e.Message, "Ok");
            }
            return;
        }
    }
}
