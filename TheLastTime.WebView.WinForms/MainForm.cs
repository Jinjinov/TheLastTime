// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using TheLastTime.Shared;

namespace TheLastTime.WinForms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBlazorWebView();

            serviceCollection.AddServices();

            InitializeComponent();

            blazorWebView.HostPage = @"wwwroot\index.html";
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            blazorWebView.Services = serviceProvider;
            blazorWebView.RootComponents.Add<Main>("#app");

            //Task.Run(serviceProvider.UseServices);
        }
    }
}
