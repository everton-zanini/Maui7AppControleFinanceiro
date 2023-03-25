using AppControleFinanceiro.Models;
using AppControleFinanceiro.Repositories;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;

namespace AppControleFinanceiro.Views;

public partial class TransactionList : ContentPage
{
    private readonly ITransactionRepository _repository;

    public TransactionList(ITransactionRepository repository)
    {
        InitializeComponent();
        _repository = repository;

        Reload();

        WeakReferenceMessenger.Default.Register<string>(this, (obj, msg) =>
        {
            Reload();
        });
    }

    private void Reload()
    {
        var items = _repository.GetAll();
        CollectionViewTransaction.ItemsSource = items;

        double income = items.Where(data => data.Type == TransactionType.Income).Sum(data => data.Value);
        double expense = items.Where(data=>data.Type == TransactionType.Expenses).Sum(data=>data.Value);
        double balance = income - expense;

        LabelIncome.Text = income.ToString("C");
        LabelExpense.Text = expense.ToString("C");
        LabelBalance.Text = balance.ToString("C");
    }

    private void OnButtonClicked_To_TransactionAdd(object sender, EventArgs args)
    {
        
        var addPage = Handler.MauiContext.Services.GetService<TransactionAdd>();
        Navigation.PushModalAsync(addPage);
    }

    private void TapGestureRecognizerTapped_To_TransactionEdit(object sender, TappedEventArgs e)
    {

        var grid = (Grid)sender;
        var gesture = (TapGestureRecognizer)grid.GestureRecognizers[0];
        Transaction transaction = (Transaction)gesture.CommandParameter;

        var editPage = Handler.MauiContext.Services.GetService<TransactionEdit>();
        editPage.SetTransactionToEdit(transaction);
        Navigation.PushModalAsync(editPage);
    }

    private async void TapGestureRecognizerTapped_To_Delete(object sender, TappedEventArgs e)
    {
        await AnimationBorder((Border)sender, true);
        bool result = await App.Current.MainPage.DisplayAlert("Excluir!", "Tem certeza que deseja excluir?", "Sim", "Não");

        if (result)
        {
            Transaction transaction = (Transaction)e.Parameter;
            _repository.Delete(transaction);

            Reload();
        }
        else
        {
            await AnimationBorder((Border)sender, false);
        }
    }

    private Color _borderOriginalBackgtoudColor;
    private String _labelOriginalText;

    private async Task AnimationBorder(Border border, bool isDeleteAnimation)
    {
        var label = (Label)border.Content;
        if(isDeleteAnimation)
        {
            _borderOriginalBackgtoudColor = border.BackgroundColor;
            _labelOriginalText= label.Text;
            await border.RotateYTo(90, 200);
            border.BackgroundColor = Colors.Red;
            label.TextColor = Colors.White;
            label.Text = "X";
            await border.RotateYTo(180, 200);
        }
        else
        {
            await border.RotateYTo(90,200);
            label.TextColor = Colors.Black;
            label.Text = _labelOriginalText;
            border.BackgroundColor = _borderOriginalBackgtoudColor;
            await border.RotateYTo(0, 200);
        }
    }
}