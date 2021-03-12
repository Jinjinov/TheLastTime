using Blazorise;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheLastTime.Shared.Data
{
    public class ThemeOptions
    {
        readonly Dictionary<string, string> ButtonSizeClassDict = new Dictionary<string, string>()
        {
            { "small", "btn-sm" },
            { "medium", "" },
            { "large", "btn-lg" }
        };

        public string ButtonSizeClass => ButtonSizeClassDict[DataService.Settings.Size];

        readonly Dictionary<string, Size> SizeDict = new Dictionary<string, Size>()
        {
            { "small", Size.Small },
            { "medium", Size.None },
            { "large", Size.Large }
        };

        public Size Size => SizeDict[DataService.Settings.Size];

        public readonly SortedList<string, string> BootswatchThemeDict = new SortedList<string, string>()
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

        public string BootswatchTheme
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

        public readonly string[] ElementSizes = new string[] { "small", "medium", "large" };

        public string ElementSize
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

        readonly DataService DataService;

        public ThemeOptions(DataService dataService)
        {
            DataService = dataService;
        }

        public async Task PreviousSize()
        {
            int idx = Array.IndexOf(ElementSizes, DataService.Settings.Size);

            if (1 <= idx && idx < ElementSizes.Length)
            {
                DataService.Settings.Size = ElementSizes[idx - 1];
                await DataService.SaveSettings();
            }
        }

        public async Task NextSize()
        {
            int idx = Array.IndexOf(ElementSizes, DataService.Settings.Size);

            if (0 <= idx && idx < ElementSizes.Length - 1)
            {
                DataService.Settings.Size = ElementSizes[idx + 1];
                await DataService.SaveSettings();
            }
        }

        public async Task PreviousTheme()
        {
            int idx = BootswatchThemeDict.IndexOfKey(DataService.Settings.Theme);

            if (idx == 0)
            {
                DataService.Settings.Theme = "default";
                await DataService.SaveSettings();
            }
            else if (1 <= idx && idx < BootswatchThemeDict.Count)
            {
                DataService.Settings.Theme = BootswatchThemeDict.Keys[idx - 1];
                await DataService.SaveSettings();
            }
        }

        public async Task NextTheme()
        {
            if (DataService.Settings.Theme == "default")
            {
                DataService.Settings.Theme = BootswatchThemeDict.Keys[0];
                await DataService.SaveSettings();
            }
            else
            {
                int idx = BootswatchThemeDict.IndexOfKey(DataService.Settings.Theme);

                if (0 <= idx && idx < BootswatchThemeDict.Count - 1)
                {
                    DataService.Settings.Theme = BootswatchThemeDict.Keys[idx + 1];
                    await DataService.SaveSettings();
                }
            }
        }
    }
}
