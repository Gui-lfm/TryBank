namespace Trybank.Lib;

public class TrybankLib
{
    public bool Logged;
    public int loggedUser;
    public int[,] Bank;
    public int registeredAccounts;
    private int maxAccounts = 50;

    public TrybankLib()
    {
        loggedUser = -99;
        registeredAccounts = 0;
        Logged = false;
        Bank = new int[maxAccounts, 4];
    }

    public void RegisterAccount(int number, int agency, int pass)
    {
        bool alreadyExists = VerifyAccount(Bank, number, agency);

        if (alreadyExists)
        {
            throw new ArgumentException("A conta já está sendo usada!");
        }
        //0 -> Número da conta
        //1 -> Agência
        //2 -> Senha
        //3 -> Saldo
        Bank[registeredAccounts, 0] = number;
        Bank[registeredAccounts, 1] = agency;
        Bank[registeredAccounts, 2] = pass;
        Bank[registeredAccounts, 3] = 0;

        registeredAccounts++;
    }

    public void Login(int number, int agency, int pass)
    {
        if (Logged)
        {
            throw new AccessViolationException("Usuário já está logado");
        }

        for (int i = 0; i < registeredAccounts; i++)
        {
            if (Bank[i, 0] == number && Bank[i, 1] == agency && Bank[i, 2] == pass)
            {
                Logged = true;
                loggedUser = i;
            }
            else if (Bank[i, 0] == number && Bank[i, 1] == agency && Bank[i, 2] != pass)
            {
                throw new ArgumentException("Senha incorreta");
            }
            else
            {
                throw new ArgumentException("Agência + Conta não encontrada");
            }
        }
    }

    public void Logout()
    {
        if (!Logged)
        {
            throw new AccessViolationException("Usuário não está logado");
        }
        else
        {
            Logged = false;
            loggedUser = -99;
        }
    }

    public int CheckBalance()
    {
        CheckIfLogged();

        return Bank[loggedUser, 3];
    }

    public void Deposit(int value)
    {
        CheckIfLogged();

        Bank[loggedUser, 3] += value;
    }

    public void Withdraw(int value)
    {
        CheckIfLogged();

        int userBalance = Bank[loggedUser, 3];

        if (userBalance - value < 0)
        {
            throw new InvalidOperationException("Saldo insuficiente");
        }
        else
        {
            Bank[loggedUser, 3] = userBalance - value;
        }
    }

    public void Transfer(int destinationNumber, int destinationAgency, int value)
    {
        CheckIfLogged();

        int destinationAccount = FindAccountPosition(Bank, destinationNumber, destinationAgency);

        if (destinationAccount == -99)
        {
            throw new ArgumentException("Agência + Conta não encontrada");
        }

        Withdraw(value);
        Bank[destinationAccount, 3] += value;

    }

    // funções utilitárias
    public bool VerifyAccount(int[,] db, int number, int agency)
    {
        if (registeredAccounts > 0)
        {
            for (int i = 0; i < registeredAccounts; i++)
            {
                if (db[i, 0] == number && db[i, 1] == agency)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void CheckIfLogged()
    {
        if (!Logged)
        {
            throw new AccessViolationException("Usuário não está logado");
        }
    }

    public int FindAccountPosition(int[,] db, int number, int agency)
    {
        for (int i = 0; i < registeredAccounts; i++)
        {
            if (db[i, 0] == number && db[i, 1] == agency)
            {
                return i;
            }
        }
        return -99;
    }
}
