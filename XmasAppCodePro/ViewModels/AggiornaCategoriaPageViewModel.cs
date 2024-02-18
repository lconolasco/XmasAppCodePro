using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using XmasAppCodePro.Models;
using XmasAppCodePro.Views;

namespace XmasAppCodePro.ViewModels
{
    public partial class AggiornaCategoriaPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public int _id;

        [ObservableProperty]
        public string _nomeCategoria;

        [ObservableProperty]
        public string _message;

        [ObservableProperty]
        private Categoria _categoria;

        [ObservableProperty]
        public ObservableCollection<Categoria> _listaDiCategorie;

        HttpClient client;

        JsonSerializerOptions _serializerOptions;

        string baseUrl = "https://lconolasco.somee.com/api";

        public AggiornaCategoriaPageViewModel()
        {
            client = new HttpClient();

            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        [RelayCommand]
        private async Task OttieniCategorieAsync()
        {
            Message = string.Empty;
            
            var url = $"{baseUrl}/categoria/{Id}";
            var response = await client.GetAsync(url);

            try
            {
                if (Id > 0)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using var responseStream = await response.Content.ReadAsStreamAsync();
                        var data = await JsonSerializer.DeserializeAsync<Categoria>(responseStream, _serializerOptions);
                        this.Categoria = data;

                        Id = data.Id;
                        NomeCategoria = data.Nome;
                    }
                    else
                    {
                        Message = "Nessuna Categoria trovata con questo codice.";
                        await Shell.Current.DisplayAlert("AVVISO", $"{response.Content} Contralla il codice inserito e ritenta.", "Ok");
                    }
                }else
                { 
                    Message = "Ops!!! Inserisca il Codice della categoria da cercare.";
                }
            }
            catch (Exception e)
            {

                Message = "Ops!!! Qualcosa è andato storto. Tentativo di collegamento al database falito.";
                await Shell.Current.DisplayAlert("Errore", e.Message, "Ok");
            }

        }

        [RelayCommand]
        private async Task AggiornaCategoriaAsync()
        {
            Message = string.Empty;

            var url = $"{baseUrl}/categoria/{Categoria.Id}";
            try
            {
                if (Id > 0 && NomeCategoria is not null && Id==Categoria.Id)
                {
                    Categoria categoria = new()
                    {
                        Id = Id,
                        Nome = NomeCategoria
                    };

                    string categoriaJson = JsonSerializer.Serialize(categoria, _serializerOptions);
                    StringContent content = new(categoriaJson, Encoding.UTF8, "application/json");
                    var response = await client.PatchAsync(url, content);

                    if (response.IsSuccessStatusCode) 
                    {
                        Id = 0;
                        NomeCategoria = string.Empty;

                        await Shell.Current.GoToAsync("..");
                    }
                    else
                        await Shell.Current.DisplayAlert("Error", $"{response.Content}", "Ok");
                }
                else
                {
                    Message = "Impossibile aggiornare la categoria con il codice diverso da quello assegnato";
                }
            }
            catch (Exception e)
            {
                await Shell.Current.DisplayAlert("Errore", e.Message, "Ok");
                Message = e.Message;
            }
        }
    }
}
