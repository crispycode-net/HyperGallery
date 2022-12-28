using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Center;
using Center.Shared;
using MudBlazor;
using MediaSchnaff.Shared.LocalData;

namespace Center.Pages
{
    public partial class Index
    {
        [Inject] IDirectories directories { get; set; }
        [Inject] IJSRuntime? js { get; set; }

        private List<int> years = new List<int>();
        private List<string> images = new List<string>();
        private IJSObjectReference jsFunctions;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender && js != null)
            {
                jsFunctions = await js.InvokeAsync<IJSObjectReference>(
                    "import", "./Pages/Index.razor.js");

                await jsFunctions.InvokeVoidAsync("focusElement", "yearrow");

                StateHasChanged();
            }
        }

        protected override Task OnInitializedAsync()
        {
            years = new List<int> { 2017, 2018, 2019, 2020, 2021, 2022 };

            var yearDir = Path.Combine(directories.ThumbnailDir, "2017");
            var yearFiles = Directory.GetFiles(@"C:\Users\aleba\source\repos\MediaSchnaff\Center\wwwroot\Thumbnails\2017");

            foreach (var file in yearFiles)
            {
                var relFile = Path.GetRelativePath(@"C:\Users\aleba\source\repos\MediaSchnaff\Center\wwwroot", file);
                images.Add(relFile);
            }

            return base.OnInitializedAsync();
        }

        private void YearRowKeyDown(KeyboardEventArgs e)
        {
            Console.WriteLine("Year: " + e.Key.ToString());
        }
        private void ImageRowKeyDown(KeyboardEventArgs e)
        {
            Console.WriteLine("Image: " + e.Key.ToString());
        }
    }
}