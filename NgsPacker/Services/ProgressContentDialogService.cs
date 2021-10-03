using NgsPacker.Views;
using System.Threading.Tasks;

namespace NgsPacker.Services
{
    public class ProgressContentDialogService : IProgressContentDialogService
    {
        private ProgressContentDialog contentDialog;

        public ProgressContentDialogService()
        {
            contentDialog = new();
        }

        public async Task ShowAsync()
        {
            _ = await contentDialog.ShowAsync();
        }

        public void Hide()
        {
            contentDialog.Hide();
        }
    }
}
