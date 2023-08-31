using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using CarClient.Helpers;
using CarClient.Views;
using CarStore.Models;
using Newtonsoft.Json;
using System.Text;
using System.Net;

namespace CarClient.ViewModels
{
    class AppViewModel
    {
        RelayCommand? addCommand;
        RelayCommand? editCommand;
        RelayCommand? deleteCommand;

        //TODO Вынести apiBaseUrl в appsettings
        const string apiBaseUri = "https://localhost:44383/api/";
        public ObservableCollection<Car> Cars { get; set; }

        async public static Task<AppViewModel> BuildViewModelAsync()
        {
            var cars = await GetDataTask();
            var result = new ObservableCollection<Car>(cars);
            return new AppViewModel(result);
        }

        private AppViewModel(ObservableCollection<Car> cars)
        {
            this.Cars = cars;
        }

        //Найти оптимальный способ изменения Cars 
        async private void UpdateData()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new System.Uri(apiBaseUri);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue
                ("applocation/json"));
            var response = httpClient.GetAsync("Cars").Result;
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var cars = JsonConvert.DeserializeObject<List<Car>>(result);
                Cars.Clear();
                foreach (Car car in cars){
                    Cars.Add(car);
                }
            }
        }

        public static async Task<List<Car>> GetDataTask()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new System.Uri(apiBaseUri);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue
                ("applocation/json"));
            var response = httpClient.GetAsync("Cars").Result;
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var cars = JsonConvert.DeserializeObject<List<Car>>(result);
                return cars;
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }
            return null;
        }

        // команда добавления
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand((o) =>
                  {
                      CarWindow carWindow = new CarWindow(new Car());
                      if (carWindow.ShowDialog() == true)
                      {
                          Car car = carWindow.Car;

                          var httpClient = (HttpWebRequest)WebRequest.Create(new System.Uri(apiBaseUri + "Cars"));
                          httpClient.Accept = "application/json";
                          httpClient.ContentType = "application/json";
                          httpClient.Method = "POST";

                          var json = JsonConvert.SerializeObject(car);
                          var bytes = Encoding.ASCII.GetBytes(json);
                          var stream = httpClient.GetRequestStream();
                          stream.Write(bytes, 0, bytes.Length);
                          stream.Close();

                          //TODO обработка ответов (выбрасывание исключений и отображение ошибки на экране)
                          httpClient.GetResponse();
                          UpdateData();
                      }
                  }));
            }
        }

        // команда редактирования
        public RelayCommand EditCommand
        {
            get
            {
                return editCommand ??
                  (editCommand = new RelayCommand((selectedItem) =>
                  {
                      // получаем выделенный объект
                      Car? car = selectedItem as Car;
                      if (car == null) return;

                      Car vm = new Car
                      {
                          Id = car.Id,
                          ModelName = car.ModelName,
                          Year = car.Year,
                          Price = car.Price
                      };
                      CarWindow carWindow = new CarWindow(vm);


                      if (carWindow.ShowDialog() == true)
                      {
                          car.Id = carWindow.Car.Id;
                          car.ModelName = carWindow.Car.ModelName;
                          car.Year = carWindow.Car.year;
                          car.Price = carWindow.Car.Price;

                          var httpClient = (HttpWebRequest)WebRequest.Create(new System.Uri(apiBaseUri + $"Cars/{car.Id}"));
                          httpClient.Accept = "application/json";
                          httpClient.ContentType = "application/json";
                          httpClient.Method = "PUT";

                          var json = JsonConvert.SerializeObject(car);
                          var bytes = Encoding.ASCII.GetBytes(json);
                          var stream = httpClient.GetRequestStream();
                          stream.Write(bytes, 0, bytes.Length);
                          stream.Close();

                          //TODO обработка ответов (выбрасывание исключений и отображение ошибки на экране)
                          httpClient.GetResponse();
                          UpdateData();
                      }
                  }));
            }
        }

        // команда удаления
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand((selectedItem) =>
                  {
                      // получаем выделенный объект
                      Car? car = selectedItem as Car;
                      if (car == null) return;

                      var httpClient = (HttpWebRequest)WebRequest.Create(new System.Uri(apiBaseUri + $"Cars/{car.Id}"));
                      httpClient.Method = "DELETE";

                      //TODO обработка ответов (выбрасывание исключений и отображение ошибки на экране)
                      httpClient.GetResponse();
                      UpdateData();
                  }));
            }
        }
    }
}
