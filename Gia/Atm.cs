using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gia
{
    public class Atm
    {
        private static JObject userData;
       
        readonly string filePath = "C:/me/projects/sweeft/Gia/Gia/Tdata.json";
        public Atm()
        {  
            try
            {
                 userData = JObject.Parse(File.ReadAllText(filePath));
            }
            catch(Exception e)
            {
               Console.WriteLine($"Error occured during fetching data\n{e.Message}");
            }
        }
        //program starting point
        public void RunAtm()
        {
            char[] options = { '1', '2', '3', '4','5', '6' };
            char option;
            ConsoleKeyInfo keyInfo =new();
            //saves users Prsonal ID before offering other functinality
            if (Login())
            {
                Console.WriteLine($"Hello {userData["firstName"].ToString()}");
            }
            else
            {
                return;
            }

            Console.WriteLine("## ATM ##");
            //offers user ATM funtionality
            do
            {
                Console.WriteLine("Choose coresponding number for desired functinality: ");
                Console.WriteLine("1 - view balance");
                Console.WriteLine("2 - Withdraw");
                Console.WriteLine("3 - Deposit");
                Console.WriteLine("4 - view transactions");
                Console.WriteLine("5 - change pin");
                Console.WriteLine("6 - stop the program");
                option = Console.ReadKey(true).KeyChar;
                Console.Write("\n");
                if (!options.Contains(option))
                {
                    Console.WriteLine("Invalid option try again");
                    continue;
                }
                if(option == '6')
                {
                    break;
                }
                RunOperation(option);                            
                Console.Write("\n");
            } while (true);
        }
        //excecutes specified operations
        public void RunOperation(char option)
        {
            switch (option)
            {
                case '1':
                    {
                        Console.WriteLine("View balance: ");
                        ViewBalance();
                        break;
                    }
                case '2':
                    {
                        Console.WriteLine("Withdraw money: ");
                        double money = EnterMoney();
                        WithdrawMoney(money);
                        break;
                    }
                case '3':
                    {
                        Console.WriteLine("Deposit money: ");
                        double money = EnterMoney();
                        DepositMoney(money);
                        break;
                    }
                case '4':
                    {
                        Console.WriteLine("View past 5 operaations: ");
                        ViewTransactions();
                        break;
                    }
                case '5':
                    {
                        Console.WriteLine("Change Pin: ");
                        ChangePin();
                        break;
                    }
            }
        }

        public bool Login()
        {
            string cardNumber;
            string cvc;
            string pin;
            string expiraitionDate;
            JObject card = (JObject)userData["cardDetails"];
            do
            {
                cardNumber = EnterCard();
                cvc = EnterCvc();
                expiraitionDate = EnterExpirationDate();
                if(cardNumber != card["cardNumber"].ToString() || cvc != card["CVC"].ToString() || expiraitionDate != card["expirationDate"].ToString())
                {
                    Console.WriteLine("Invalid card number or CVC try again: ");
                    continue;
                }
                pin=EnterPin();
                if(pin == userData["pinCode"].ToString())
                {
                    return true;
                }
                return false;
            } while (true);
        }
        //validates user's card Number
         private string EnterCard()
        {
            string Id;
            Regex IdRegex = new(@"^([0-9]{16,16}$)");

            do{
                Console.Write("Enter valid card number: ");
                Id=Console.ReadLine();
                Console.WriteLine(Id);
            }while(!IdRegex.IsMatch(Id));
            return Id;
        }
        private string EnterExpirationDate()
        {
            string Id;
            Regex IdRegex = new(@"^([0-9]{2,2})\/([0-9]{2,2})$");

            do
            {
                Console.Write("Enter valid card number use / to seperate month and year: ");
                Id = Console.ReadLine() ?? "";
            } while (!IdRegex.IsMatch(Id));
            return Id;
        }

        private string EnterCvc()
        {
            string Id;
            Regex IdRegex = new(@"^[0-9]{3,3}$");

            do
            {
                Console.Write("Enter valid CVC: ");
                Id = Console.ReadLine() ?? "";
            } while (!IdRegex.IsMatch(Id));
            return Id;
        }

        private string EnterPin()
        {
            string Id;
            Regex IdRegex = new(@"^[0-9]{4,4}$");

            do
            {
                Console.Write("Enter valid PIN: ");
                Id = Console.ReadLine() ?? "";
            } while (!IdRegex.IsMatch(Id));
            return Id;
        }


        //validates user input for money
        private double EnterMoney()
        {
            double value=-1;
            bool result=false;
            while (!result || value <=0)
            {
                Console.Write("Enter valid amount of money: ");
                result = double.TryParse(Console.ReadLine(), out value);
                Console.Write("\n");
            }
            return value;
        }
        //returns balance for specifc Personal ID
        private double ViewBalance()
         {
            JArray? transactions = (JArray)userData["transactionHistory"]!;
            if (transactions.Count == 0)
            {
                Console.WriteLine("balance: 0");
                return 0;
            }
            var balance = transactions[0]["amountGEL"];
            Console.WriteLine("Your balace is: "+balance);
            return double.Parse(balance.ToString());
         }

        //Withdraws specified amount of moeny from specified user
        private bool WithdrawMoney( double money)
        {
            double balance= ViewBalance();
            //stops if withdraw moeny is greater than balance
            if (money > balance)
            {
                Console.WriteLine("Withdraw opperation was canceled due to insufficient funds");
                return false;
            }
            balance -= money;
            JArray? transactions = (JArray)userData["transactionHistory"]!;
            JObject newTransaction = new JObject(
          new JProperty("transactionDate", DateTime.Now),
          new JProperty("transactionType", "Withdraw"),
          new JProperty("amountGEL", balance)
      );
            if (transactions.Count == 5)
            {
                transactions.Last.Remove();
            }
            transactions.Insert(0,newTransaction);
            //makes changes in the file
            
            if (Save())
            {
                Console.WriteLine($"Withdrawed: {money}$");
            }
            else
            {
                Console.WriteLine("Error occured ");
            }
            return true;
        }
        //depposits moeny in the users account
        private bool DepositMoney(double money)
        {
            double balance = ViewBalance();
            balance+= money;
            JArray? transactions = (JArray)userData["transactionHistory"]!;

            JObject newTransaction = new JObject(
             new JProperty("transactionDate", DateTime.Now),
             new JProperty("transactionType", "Deposit"),
             new JProperty("amountGEL", balance)
          );
            if (transactions.Count == 5)
            {
                transactions.Last.Remove();
            }
            transactions.Insert(0,newTransaction);
            if (Save())
            {
              Console.WriteLine($"Deposited: {money}$");
              Console.WriteLine($" balance: {balance}");
                return true;
            }
            else
            {
                Console.WriteLine("Error occured ");
                return false;
            }
        }
        private void ViewTransactions()
        {
            JArray? transactions = (JArray)userData["transactionHistory"]!;
            Console.WriteLine($"Transactions({transactions.Count}): ");
            foreach(var item in transactions)
            {
                Console.WriteLine($"{item["transactionDate"]}");
                Console.WriteLine($"{item["transactionType"]}");
                Console.WriteLine($"{item["amountGEL"]}");
                Console.WriteLine($"---------------------------------------------------\n");
            }
        }

        private void ChangePin()
        {
            string pin;
            while (true)
            {
                Console.WriteLine("Enter current pin: ");
                pin = EnterPin();
                if (pin == userData["pinCode"].ToString())
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Wrong input try again: \n");
                }
            }
            Console.WriteLine("choose a new pin");
            string newPin = EnterPin();
            userData["pinCode"] = newPin;
            Save();
            Console.WriteLine("Pin was updated");
        }
      
        //functinality for saving changes in the file
        private bool Save()
        {
            try
            {
                File.WriteAllText(filePath, userData.ToString());
                return true;
            }catch(Exception e)
            {
                Console.WriteLine("Error ocuured during saving data ...\n Eoor= "+e.Message);
                return false;
            }
        }

    }
}
