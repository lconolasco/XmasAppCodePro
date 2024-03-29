﻿using Camera.MAUI.ZXingHelper;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using XmasAppCodePro.Models;

namespace XmasAppCodePro.ViewModels
{
    public partial class AggiornaProdottoPageViewModel : ObservableObject
    {
        //Servizio di connessione per il consumo della REST API
        readonly HttpClient client;

        //Configurazione JSON per la serializzazione
        readonly JsonSerializerOptions _serializerOptions;

        //Definizione dell'URL base
        readonly string baseUrl = "https://lconolasco.somee.com/api";

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
        public int _quantita;

        [ObservableProperty]
        public int _categoriaId;

        [ObservableProperty]
        public string _message;

        [ObservableProperty]
        public Prodotto _prodotto;

        [ObservableProperty]
        public ObservableCollection<Prodotto> _catalogoProdotti;

        public AggiornaProdottoPageViewModel()
        {
            //L'instanza di HttpClient
            client = new HttpClient();

            //L'instanza della colezione di oggetti
            CatalogoProdotti = new ObservableCollection<Prodotto>();

            //Settaggio della serializzazione di JSON
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        //** Consumo vero e proprio della R£ST API **//

        //Ricupero il prodotto tramite barcode

        public ICommand RicuperaProdotto => new Command(async () => await RicuperaProdottoAsync());

        private async Task RicuperaProdottoAsync()
        {
            Message=string.Empty;
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

        public ICommand AggiornaProdotto => new Command(async () => await AggiornaProdottoAsync());

        private async Task AggiornaProdottoAsync()
        {
            var url = $"{baseUrl}/Prodotto/{Prodotto.Barcode}";
            try
            {
                if(Barcode is not null && Nome is not null && Descrizione is not null && Prezzo >=0 && PesoLordo >= 0 && Quantita >=0 && CategoriaId >=0)
                {
                    Prodotto prodotto = new()
                    {
                        Barcode = Barcode,
                        Nome = Nome,
                        Descrizione = Descrizione,
                        ImageUrl = ImageUrl,
                        Prezzo = Prezzo,
                        PesoLordo = PesoLordo,
                        Quantita = Quantita,
                        CategoriaId = CategoriaId
                    };

                    string prodottoJson = JsonSerializer.Serialize(prodotto, _serializerOptions);
                    StringContent content = new(prodottoJson, Encoding.UTF8, "application/json");
                    var response = await client.PatchAsync(url, content);

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
                        await Shell.Current.GoToAsync("..");
                    }
                    else
                        await Shell.Current.DisplayAlert("Error", response.ToString(), "Ok");

                }
                return;
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Avviso", e.Message, "Ok");
                Message = $"Conessione falita col DataBase. Controllare le conessione e riprova.";
                Prodotto vuoto = new()
                {
                    Nome = Message,
                    Descrizione = e.Message
                };
                CatalogoProdotti.Add(vuoto);
            }
        }
    }
}
