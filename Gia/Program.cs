using Gia;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


Atm atm = new();
atm.RunAtm();

//const string filePath= "C:/me/projects/sweeft/Gia/Gia/Tdata.json";
//ReadJsonFile(filePath);
//Console.ReadKey();
//static void ReadJsonFile(string filePath)
//{
//    JObject data = JObject.Parse(File.ReadAllText(filePath));
//    JObject card = (JObject)data["cardDetails"];
    
//    Console.WriteLine($"cardNumber: {card["cardNumber"].ToString()}\nCVC: {card["CVC"]}\n" +
//        $"expirationDate: {card["expirationDate"]} \npinCode: {data["pinCode"]} ");
//    JArray? transactions = (JArray)data["transactionHistory"]!;
    
//    Console.WriteLine(transactions?.Count);

//    if (transactions?.Count > 0)
//    {
//        transactions.Last?.Remove();
//    }

//    JObject newTransaction = new JObject(
//            new JProperty("transactionDate",DateTime.Now),
//            new JProperty("transactionType", "xe"),
//            new JProperty("amountGEL",100),
//            new JProperty("amountUS", 0),
//            new JProperty("amountEUR", 0)
//        );
//    transactions?.Add(newTransaction);

//   foreach(var item in transactions)
//    {
//        Console.WriteLine(item);
//    }
//    File.WriteAllText(filePath, data.ToString());
//}