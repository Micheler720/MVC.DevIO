using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DevIO.App.Extensions
{
    public class EmailTagHelper : TagHelper
    {
        public string EmailDomain { get; set; } = "teste.com";

        public async override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";
            var content = await output.GetChildContentAsync();
            var target  = $"{content.GetContent()}@{EmailDomain}";
            output.Attributes.SetAttribute("href", $"malito:{target}");
            output.Content.SetContent(target);
        }
    }
}
