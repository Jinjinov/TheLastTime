using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
                // TODO: Deserialize<Category>

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
                await DataService.AddCategories(categoryList);
            }
        }

        async Task ExportFile()
        {
            if (exportAllData)
            {
                // TODO: serialize DataService.RootCategory

                string jsonString = JsonSerializer.Serialize(DataService.CategoryList, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true });

                await JsInterop.SaveAsUTF8("ididit.json", jsonString);
            }
            else
            {
                ISerializer serializer = new SerializerBuilder().Build();

                var dict = DataService.CategoryList.ToDictionary(category => category.Description, category => category.HabitList.Select(habit => habit.Description));

                string yamlString = serializer.Serialize(dict);

                await JsInterop.SaveAsUTF8("ididit.yaml", yamlString);
            }
        }

        //async Task SaveAsUTF8(string filename, string content)
        //{
        //    byte[] data = Encoding.UTF8.GetBytes(content);
        //    await jsRuntime.InvokeAsync<object>("saveAsFile", filename, Convert.ToBase64String(data));
        //}

        async Task ImportNotes(InputFileChangeEventArgs e)
        {
            int count = e.FileCount;

            long maxId = DataService.GoalList.Any() ? DataService.GoalList.Max(g => g.Id) : 0;

            List<Goal> goalList = new List<Goal>();

            foreach (IBrowserFile browserFile in e.GetMultipleFiles(count))
            {
                string name = browserFile.Name;

                if (!name.EndsWith(".md"))
                    continue;

                Stream stream = browserFile.OpenReadStream();

                using StreamReader streamReader = new StreamReader(stream);

                string text = await streamReader.ReadToEndAsync();

                Goal goal = new Goal
                {
                    Id = ++maxId,
                    CategoryId = DataService.RootCategory.Id,
                    Description = name,
                    Notes = text
                };

                goalList.Add(goal);
            }

            await DataService.AddGoals(goalList);
        }

        async Task ReadDirectoryFiles()
        {
            JsonElement json = await JsInterop.ReadDirectoryFiles();

            ParseJson(json);
        }

        private void ParseJson(JsonElement json)
        {
            JsonElement jsonName = json.GetProperty("name");
            JsonElement jsonNodes = json.GetProperty("nodes");

            string name = jsonName.GetString() ?? string.Empty;

            Category? root = DataService.CategoryList.FirstOrDefault(c => c.Description == name);

            if (root == null)
            {
                long maxId = DataService.CategoryList.Max(category => category.Id);

                // TODO: add to DB

                root = new Category
                {
                    Id = ++maxId,
                    CategoryId = DataService.RootCategory.Id,
                    Description = name
                };
            }

            Traverse(jsonNodes, root);
        }

        private static void Traverse(JsonElement json, Category parent)
        {
            foreach (JsonElement jsonElement in json.EnumerateArray())
            {
                JsonElement jsonName = jsonElement.GetProperty("name");
                string name = jsonName.GetString() ?? string.Empty;

                if (jsonElement.TryGetProperty("text", out JsonElement jsonText))
                {
                    if (!name.EndsWith(".md"))
                        continue;

                    // TODO: add to DB

                    // TODO: set Id

                    Goal goal = new Goal
                    {
                        //Id = ,
                        CategoryId = parent.Id,
                        Description = name,
                        Notes = jsonText.GetString() ?? string.Empty
                    };

                    parent.GoalList.Add(goal);
                }
                else if (jsonElement.TryGetProperty("nodes", out JsonElement jsonNodes))
                {
                    if (name.StartsWith('.'))
                        continue;

                    // TODO: get existing category form DB

                    // TODO: add to DB

                    // TODO: set Id

                    Category category = new Category
                    {
                        //Id = ,
                        CategoryId = parent.Id,
                        Description = name
                    };

                    if (parent.CategoryList == null)
                    {
                        parent.CategoryList = new List<Category>();
                    }

                    parent.CategoryList.Add(category);

                    Traverse(jsonNodes, category);
                }
            }
        }
    }
}
