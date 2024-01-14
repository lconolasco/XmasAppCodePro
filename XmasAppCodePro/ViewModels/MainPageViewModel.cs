using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using XmasAppCodePro.Models;

namespace XmasAppCodePro.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        //Serviço de conexao para consumo REST API
        HttpClient client;

        //Configuraçao do JSON
        JsonSerializerOptions _serializerOptions;

        //Definiçao da URL base
        string baseUrl = "https://lconolasco.somee.com/api";

        [ObservableProperty]
        public int _id;

        [ObservableProperty]
        public string _barcode;

        [ObservableProperty]
        public string _nome;

        [ObservableProperty]
        public string _imageUrl;

        [ObservableProperty]
        public string _descrizione;

        [ObservableProperty]
        public decimal _prezzo;

        [ObservableProperty]
        public decimal _pesoLordo;

        [ObservableProperty]
        public int _CategoriaId;

        [ObservableProperty]
        public ObservableCollection<Prodotto> _catalogoProdotti;

        [ObservableProperty]
        public Prodotto _prodotto;

        [ObservableProperty]
        public string _message;

        public MainPageViewModel()
        {
            //Instancia de HttpClient
            client = new HttpClient();

            //Instancia da Coleçao de objetos
            CatalogoProdotti = new ObservableCollection<Prodotto>();

            //Configuraçao da serializaçao do JSON
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        //** Consumo da REST API **//

        //retorna a coleçao de produtos
        public ICommand RicuperaProdotti => new Command(async () => await RicuperaProdottiAsync());

        private async Task RicuperaProdottiAsync()
        {
            CatalogoProdotti.Clear();
            var url = $"{baseUrl}/prodotto";
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    var data = await JsonSerializer.DeserializeAsync<ObservableCollection<Prodotto>>(responseStream, _serializerOptions);
                    CatalogoProdotti = data;
                }
            }
            return;
        }

        //retorna um produto pelo barcode
        public ICommand RicuperaProdottoDalBarcode =>
            new Command(async () => await RicuperaProdottoDalBarcodeAsync());

        private async Task RicuperaProdottoDalBarcodeAsync()
        {

            try
            {
                CatalogoProdotti.Clear();
                if (Barcode is not null)
                {
                    var barcodeProdotto = Barcode;
                    if (barcodeProdotto is not null)
                    {
                        var url = $"{baseUrl}/prodotto/{Barcode}";
                        var response = await client.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            using (var responseStream = await response.Content.ReadAsStreamAsync())
                            {
                                var data = await JsonSerializer.DeserializeAsync<Prodotto>(responseStream, _serializerOptions);
                                Prodotto = data;
                                CatalogoProdotti.Add(data);

                            }
                        }
                        else
                        {
                            Prodotto vuoto = new Prodotto();

                            vuoto.Nome = "Nessuno Prodotto trovato con questo codice.";
                            vuoto.Descrizione = "Certifica che questo prodotto sia inserito nel Database in precedenza.";
                            CatalogoProdotti.Add(vuoto);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Message = $"Conessione falita col DataBase. Controllare le conessione e riprova.";
                Prodotto vuoto = new Prodotto();
                vuoto.Nome = Message;
                vuoto.Descrizione = e.Message;
                CatalogoProdotti.Add(vuoto);
                _ = Shell.Current.DisplayAlert("Avviso", e.Message, "Ok");
            }
            return;

        }

        [RelayCommand]
        private async Task AggiungeProdotto() => await Shell.Current.GoToAsync("AggiungeProdottoPage");

        [RelayCommand]
        private void Refresh()
        {
            CatalogoProdotti.Clear();
            Barcode = string.Empty;
        }
    }
}
