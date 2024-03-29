﻿using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel;
using TheLastTime.Shared.Data;

namespace TheLastTime.Shared.Components
{
    public sealed partial class Header : IDisposable
    {
        [Inject]
        DataService DataService { get; set; } = null!;

        protected override void OnInitialized()
        {
            DataService.PropertyChanged += PropertyChanged;
        }

        void PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            StateHasChanged();
        }

        public void Dispose()
        {
            DataService.PropertyChanged -= PropertyChanged;
        }
    }
}
