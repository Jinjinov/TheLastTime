using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TheLastTime.Shared.Data;
using TheLastTime.Shared.Models;
using YamlDotNet.Serialization;

namespace TheLastTime.Shared.Components
{
    public sealed partial class OptionsComponent : IDisposable
    {
        [Inject]
        JsInterop JsInterop { get; set; } = null!;

        [Inject]
        DataService DataService { get; set; } = null!;

        [Inject]
        State State { get; set; } = null!;

        [Inject]
        ThemeOptions Theme { get; set; } = null!;

        string TextClass = string.Empty; // "text-light"

        bool exportAllData = true;

        protected override void OnInitialized()
        {
            DataService.PropertyChanged += PropertyChanged;
            State.PropertyChanged += PropertyChanged;
        }

        void PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            StateHasChanged();
        }

        public void Dispose()
        {
            DataService.PropertyChanged -= PropertyChanged;
            State.PropertyChanged -= PropertyChanged;
        }

        async Task ImportFile(InputFileChangeEventArgs e)
        {
            Stream stream = e.File.OpenReadStream();

            using StreamReader streamReader = new StreamReader(stream);

            string text = await streamReader.ReadToEndAsync();

            List<Category>? categoryList = null;

            if (e.File.Name.EndsWith(".json"))
            {
                categoryList = JsonSerializer.Deserialize<List<Category>>(text, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true });
            }

            if (e.File.Name.EndsWith(".yaml"))
            {
                Deserializer deserializer = new Deserializer();
                Dictionary<string, List<string>> dict = deserializer.Deserialize<Dictionary<string, List<string>>>(text);

                categoryList = new List<Category>();

                long maxId = DataService.CategoryList.Max(category => category.Id);

                foreach (var pair in dict)
                {
                    Category category = new Category { Id = ++maxId, Description = pair.Key };

                    foreach (var item in pair.Value)
                    {
                        Habit habit = new Habit { CategoryId = category.Id, Description = item };

                        category.HabitList.Add(habit);
                    }

                    categoryList.Add(category);
                }
            }

            if (categoryList != null)
            {
                await DataService.AddData(categoryList);
            }
        }

        async Task ExportFile()
        {
            if (exportAllData)
            {
                string jsonString = JsonSerializer.Serialize(DataService.CategoryList, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true });

                await JsInterop.SaveAsUTF8("TheLastTime.json", jsonString);
            }
            else
            {
                ISerializer serializer = new SerializerBuilder().Build();

                var dict = DataService.CategoryList.ToDictionary(category => category.Description, category => category.HabitList.Select(habit => habit.Description));

                string yamlString = serializer.Serialize(dict);

                await JsInterop.SaveAsUTF8("TheLastTime.yaml", yamlString);
            }
        }

        //async Task SaveAsUTF8(string filename, string content)
        //{
        //    byte[] data = Encoding.UTF8.GetBytes(content);
        //    await jsRuntime.InvokeAsync<object>("saveAsFile", filename, Convert.ToBase64String(data));
        //}
    }
}
