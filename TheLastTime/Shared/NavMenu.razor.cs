using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TheLastTime.Data;
using YamlDotNet.Serialization;

namespace TheLastTime.Shared
{
    public partial class NavMenu
    {
        protected readonly SortedList<string, string> bootswatchThemeDict = new SortedList<string, string>()
        {
            /*   L */ { "cerulean", "sha384-3fdgwJw17Bi87e1QQ4fsLn4rUFqWw//KU0g8TvV6quvahISRewev6/EocKNuJmEw" },
            /*  M  */ { "cosmo", "sha384-5QFXyVb+lrCzdN228VS3HmzpiE7ZVwLQtkt+0d9W43LQMzz4HBnnqvVxKg6O+04d" }, /* square corners */
            /*  M  */ { "cyborg", "sha384-nEnU7Ae+3lD52AK+RGNzgieBWMnEfgTbRHIwEvp1XXPdqdO6uLTd/NwXbzboqjc2" },
            /*  M  */ { "darkly", "sha384-nNK9n28pDUDDgIiIqZ/MiyO3F4/9vsMtReZK39klb/MtkZI3/LtjSjlmyVPS3KdN" },
            /*  M  */ { "flatly", "sha384-qF/QmIAj5ZaYFAeQcrQ6bfVMAh4zZlrGwTPY7T/M+iTTLJqJBJjwwnsE5Y0mV7QK" },
            /*  M  */ { "journal", "sha384-QDSPDoVOoSWz2ypaRUidLmLYl4RyoBWI44iA5agn6jHegBxZkNqgm2eHb6yZ5bYs" },
            /* S   */ { "litera", "sha384-enpDwFISL6M3ZGZ50Tjo8m65q06uLVnyvkFO3rsoW0UC15ATBFz3QEhr3hmxpYsn" },
            /* S   */ { "lumen", "sha384-GzaBcW6yPIfhF+6VpKMjxbTx6tvR/yRd/yJub90CqoIn2Tz4rRXlSpTFYMKHCifX" },
            /*   L */ { "lux", "sha384-9+PGKSqjRdkeAU7Eu4nkJU8RFaH8ace8HGXnkiKMP9I9Te0GJ4/km3L1Z8tXigpG" }, /* square corners */
            /*  M  */ /* { "materia", "sha384-B4morbeopVCSpzeC1c4nyV0d0cqvlSAfyXVfrPJa25im5p+yEN/YmhlgQP/OyMZD" }, /* broken select option */
            /*   L */ { "minty", "sha384-H4X+4tKc7b8s4GoMrylmy2ssQYpDHoqzPa9aKXbDwPoPUA3Ra8PA5dGzijN+ePnH" },
            /*  M  */ { "pulse", "sha384-L7+YG8QLqGvxQGffJ6utDKFwmGwtLcCjtwvonVZR/Ba2VzhpMwBz51GaXnUsuYbj" }, /* square corners */
            /*  M  */ { "sandstone", "sha384-zEpdAL7W11eTKeoBJK1g79kgl9qjP7g84KfK3AZsuonx38n8ad+f5ZgXtoSDxPOh" },
            /* S   */ { "simplex", "sha384-FYrl2Nk72fpV6+l3Bymt1zZhnQFK75ipDqPXK0sOR0f/zeOSZ45/tKlsKucQyjSp" },
            /* S   */ { "sketchy", "sha384-RxqHG2ilm4r6aFRpGmBbGTjsqwfqHOKy1ArsMhHusnRO47jcGqpIQqlQK/kmGy9R" },
            /*  M  */ { "slate", "sha384-8iuq0iaMHpnH2vSyvZMSIqQuUnQA7QM+f6srIdlgBrTSEyd//AWNMyEaSF2yPzNQ" },
            /*  M  */ /* { "solar", "sha384-NCwXci5f5ZqlDw+m7FwZSAwboa0svoPPylIW3Nf+GBDsyVum+yArYnaFLE9UDzLd" }, /* transparent card background */
            /*   L */ { "spacelab", "sha384-F1AY0h4TrtJ8OCUQYOzhcFzUTxSOxuaaJ4BeagvyQL8N9mE4hrXjdDsNx249NpEc" },
            /*  M  */ { "superhero", "sha384-HnTY+mLT0stQlOwD3wcAzSVAZbrBp141qwfR4WfTqVQKSgmcgzk+oP0ieIyrxiFO" }, /* square corners */
            /*   L */ { "united", "sha384-JW3PJkbqVWtBhuV/gsuyVVt3m/ecRJjwXC3gCXlTzZZV+zIEEl6AnryAriT7GWYm" },
            /*  M  */ { "yeti", "sha384-mLBxp+1RMvmQmXOjBzRjqqr0dP9VHU2tb3FK6VB0fJN/AOu7/y+CAeYeWJZ4b3ii" }, /* square corners */
        };

        protected string BootswatchTheme
        {
            get => DataService.Settings.Theme;
            set
            {
                if (DataService.Settings.Theme != value)
                {
                    DataService.Settings.Theme = value;
                    DataService.SaveSettings().Wait();
                }
            }
        }

        protected readonly string[] elementSizes = new string[] { "small", "medium", "large" };

        protected string ElementSize
        {
            get => DataService.Settings.Size;
            set
            {
                if (DataService.Settings.Size != value)
                {
                    DataService.Settings.Size = value;
                    DataService.SaveSettings().Wait();
                }
            }
        }

        protected Sort Sort
        {
            get => DataService.Settings.Sort;
            set
            {
                if (DataService.Settings.Sort != value)
                {
                    DataService.Settings.Sort = value;
                    DataService.SaveSettings().Wait();
                }
            }
        }

        [Inject]
        NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        DataService DataService { get; set; } = null!;

        [Inject]
        State State { get; set; } = null!;

        private bool collapseNavMenu = true;

        protected string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        protected void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        protected async Task PreviousSort()
        {
            if (DataService.Settings.Sort > Sort.Index)
            {
                DataService.Settings.Sort -= 1;
                await DataService.SaveSettings();
            }
        }

        protected async Task NextSort()
        {
            if (DataService.Settings.Sort < Sort.ElapsedPercent)
            {
                DataService.Settings.Sort += 1;
                await DataService.SaveSettings();
            }
        }

        protected async Task PreviousSize()
        {
            int idx = Array.IndexOf(elementSizes, DataService.Settings.Size);

            if (1 <= idx && idx < elementSizes.Length)
            {
                DataService.Settings.Size = elementSizes[idx - 1];
                await DataService.SaveSettings();
            }
        }

        protected async Task NextSize()
        {
            int idx = Array.IndexOf(elementSizes, DataService.Settings.Size);

            if (0 <= idx && idx < elementSizes.Length - 1)
            {
                DataService.Settings.Size = elementSizes[idx + 1];
                await DataService.SaveSettings();
            }
        }

        protected async Task PreviousTheme()
        {
            int idx = bootswatchThemeDict.IndexOfKey(DataService.Settings.Theme);

            if (1 <= idx && idx < bootswatchThemeDict.Count)
            {
                DataService.Settings.Theme = bootswatchThemeDict.Keys[idx - 1];
                await DataService.SaveSettings();
            }
        }

        protected async Task NextTheme()
        {
            int idx = bootswatchThemeDict.IndexOfKey(DataService.Settings.Theme);

            if (0 <= idx && idx < bootswatchThemeDict.Count - 1)
            {
                DataService.Settings.Theme = bootswatchThemeDict.Keys[idx + 1];
                await DataService.SaveSettings();
            }
        }

        protected async Task ImportFile(InputFileChangeEventArgs e)
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

                NavigationManager.NavigateTo("/", true);
            }
        }

        protected bool allData = true;

        protected async Task ExportFile()
        {
            if (allData)
            {
                string jsonString = JsonSerializer.Serialize(DataService.CategoryList, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true });

                await SaveAsUTF8("TheLastTime.json", jsonString);
            }
            else
            {
                ISerializer serializer = new SerializerBuilder().Build();

                var dict = DataService.CategoryList.ToDictionary(category => category.Description, category => category.HabitList.Select(habit => habit.Description));

                string yamlString = serializer.Serialize(dict);

                await SaveAsUTF8("TheLastTime.yaml", yamlString);
            }
        }

        [Inject]
        IJSRuntime JSRuntime { get; set; } = null!;

        async Task SaveAsUTF8(string filename, string content)
        {
            byte[] data = Encoding.UTF8.GetBytes(content);

            await JSRuntime.InvokeAsync<object>("saveAsFile", filename, Convert.ToBase64String(data));
        }
    }
}
