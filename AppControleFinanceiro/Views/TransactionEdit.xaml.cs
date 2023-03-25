using AppControleFinanceiro.Models;
using AppControleFinanceiro.Repositories;
using CommunityToolkit.Mvvm.Messaging;
using System.Text;

namespace AppControleFinanceiro.Views;

public partial class TransactionEdit : ContentPage
{
    private readonly ITransactionRepository _repository;
    private Transaction _transaction;
    public TransactionEdit(ITransactionRepository repository)
    {
        InitializeComponent();
        _repository = repository;
    }

    public void SetTransactionToEdit(Transaction transaction)
    {
        _transaction = transaction;

        if(_transaction.Type == TransactionType.Income)
        {
            RadioIncome.IsChecked= true;
        }
        else
        {
            RadioExpense.IsChecked= true;
        }

        EntryName.Text = _transaction.Name;
        EntryValue.Text = _transaction.Value.ToString();
        DatePickerDate.Date = _transaction.Date.Date; 
    }

    private void TapGestureRecognizerTapped_To_Close(object sender, TappedEventArgs e)
    {
        Navigation.PopModalAsync();
    }

    private void OnButtonClicked_Edit(object sender, EventArgs e)
    {
        // 1 - Validar dados digitados
        if (!IsValidData())
        {
            return;
        }

        // 2 - Salvar no banco
        SaveTransactionInDataBase();

        // 3 - Fechar a tela
        Navigation.PopModalAsync();
        WeakReferenceMessenger.Default.Send<string>(string.Empty);
    }

    private void SaveTransactionInDataBase()
    {
        Transaction transaction = new Transaction()
        {
            Id = _transaction.Id,
            Type = RadioIncome.IsChecked ? TransactionType.Income : TransactionType.Expenses,
            Name = EntryName.Text,
            Date = DatePickerDate.Date,
            Value = double.Parse(EntryValue.Text)

        };

        _repository.Update(transaction);
    }

    private bool IsValidData()
    {
        bool valid = true;
        double result;
        StringBuilder sb = new StringBuilder();

        if (string.IsNullOrWhiteSpace(EntryName.Text))
        {
            sb.AppendLine("O campo 'Nome' deve ser preenchido.");
            valid = false;
        }

        if (string.IsNullOrWhiteSpace(EntryValue.Text))
        {
            sb.AppendLine("O campo 'Valor' deve ser preenchido.");
            valid = false;
        }

        if (!string.IsNullOrEmpty(EntryName.Text) && !double.TryParse(EntryValue.Text, out result))
        {
            sb.AppendLine("O campo 'Valor' é inválido.");
            valid = false;
        }

        if (!valid)
        {
            LabelError.IsVisible = true;
            LabelError.Text = sb.ToString();
        }

        return valid;
    }
}