// See https://aka.ms/new-console-template for more information
using JamaaTech.Smpp.Net.Client;
using JamaaTech.Smpp.Net.Lib;

//Start
SmppClient client = new();
SmppConfig config = GetSmppConfiguration();
bool getNewConfig = false;
do
{
    //Get new smpp configuration
    if (getNewConfig)
    {
        config = GetSmppConfiguration();
    }

    //Show smpp configuration chosen
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"\n\nYour Smpp Configuration:\n" +
        $"Ip: {config.Ip}\n" +
        $"Port: {config.Port}\n" +
        $"Username: {config.Username}\n" +
        $"SystemType: {config.SystemType}\n" +
        $"Password: {config.Password}\n" +
        $"DataCoding: {GetDataCoding(config.DataCoding)}\n" +
        $"NPI: {GetNPI(config.NPI)}\n" +
        $"TON: {GetTON(config.TON)}\n"
        );
    Console.ResetColor();

    Console.WriteLine("Connecting now ...");

    //Connect to the SMPP
    Connect2SMPP(config);
    bool exit = true;

    //Wait for the connection to establish
    await Task.Delay(3000);

    //Check if the smpp connection is established
    if (client.ConnectionState == SmppConnectionState.Connected)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Yaay!, SMPP Connected Successfully :D");
        Console.ResetColor();

        //Send Test SMS
        do
        {
            Console.Write("Send SMS Test To:");
            string? msisdn = Console.ReadLine();
            Send(msisdn);

            Console.Write("Done Sending SMS? Y/N:");
            string? input = Console.ReadLine();
            if (input!.ToLower() == "y" || input.ToLower() == "yes")
            {
                exit = false;
            }
        } while (exit);

        //Disconnect the smpp
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("\nPress Enter to Disconnect");
        Console.ResetColor();
        Console.ReadLine();
        DisconnectFromSMPP();

        //Option to keep the connection on or try another smpp
        Console.Write("want to reconnect to the same SMPP Y/N:");
        string? sameConnection = Console.ReadLine();
        if (sameConnection!.ToLower() == "y" || sameConnection.ToLower() == "yes")
        {
            getNewConfig = false;
        }
        else
        {
            getNewConfig = true;
        }
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Couldnt Connect, Try again!\n\n");
        Console.ResetColor();
        getNewConfig = true;
    }

} while (true);

//Get Smpp Config
SmppConfig GetSmppConfiguration()
{
    SmppConfig config = new();

    //Get IP
    Console.Write("Enter IP:");
    config.Ip = Console.ReadLine();

    //Get Port
    Console.Write("Enter Port:");
    string? port = Console.ReadLine();
    _ = int.TryParse(port, out int portNumber);
    config.Port = portNumber;

    //Get username
    Console.Write("Enter Username:");
    config.Username = Console.ReadLine();

    //Get system type
    Console.Write("Enter SystemType:");
    config.SystemType = Console.ReadLine();

    //Get password
    Console.Write("Enter Password:");
    config.Password = Console.ReadLine();

    //Get data encoding
    Console.WriteLine("Data Encoding:");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("0.SMSCDefault (default)\n1.ASCII\n2.Octet1\n3.Latin1\n4.Octet2\n5.JIS\n6.Cyrllic\n7.UCS2\n8.Pictogram\n9.MusicCodes\n10.ExtendedKanji\n11.KS_C_5601\n12.GSM_MWI_1\n13.GSM_MWI_2\n14.GMS_MessageClass");
    Console.ResetColor();
    Console.Write("Choose an option:");
    string? DataEncoding = Console.ReadLine();
    _ = int.TryParse(DataEncoding, out int DataEncodingOption);
    config.DataCoding = DataEncodingOption;

    //Get NPI
    Console.WriteLine("Numbering Plan Indicator (NPI):");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("0.Unknown (default)\n1.ISDN\n2.Data\n3.Telex\n4.LandMobile\n5.National\n6.Private\n7.ERMES\n8.Internet\n9.WapClient");
    Console.ResetColor();
    Console.Write("Choose an option:");
    string? NPI = Console.ReadLine();
    _ = int.TryParse(NPI, out int NPIOption);
    config.NPI = NPIOption;

    //Get TON
    Console.WriteLine("Type Of Number (TON):");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("0.Unknown (default)\n1.International\n2.National\n3.NetworkSpecific\n4.SubscriberNumber\n5.Alphanumeric\n6.Abbreviated");
    Console.ResetColor();
    Console.Write("Choose an option:");
    string? TON = Console.ReadLine();
    _ = int.TryParse(TON, out int TONOption);
    config.TON = TONOption;

    return config;
}
//Smpp connection method
void Connect2SMPP(SmppConfig config)
{

    if (client.ConnectionState != SmppConnectionState.Connected)
    {
        client.Properties.Host = config.Ip;
        client.Properties.Port = config.Port;
        client.Properties.SourceAddress = "SmppTester";
        client.Properties.SystemID = config.Username;
        client.Properties.Password = config.Password;
        client.Properties.SystemType = config.SystemType;
        client.Properties.DefaultEncoding = GetDataCoding(config.DataCoding);
        client.Properties.AddressNpi = GetNPI(config.NPI);
        client.Properties.AddressTon = GetTON(config.TON);
        client.Properties.UseSeparateConnections = false;
        client.AutoReconnectDelay = 3_000;
        client.KeepAliveInterval = 13_000;
        client.Start();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Smpp already connected");
        Console.ResetColor();
    }
}

//Data coding options method
DataCoding GetDataCoding(int option)
{
    return option switch
    {
        0 => DataCoding.SMSCDefault,
        1 => DataCoding.ASCII,
        2 => DataCoding.Octet1,
        3 => DataCoding.Latin1,
        4 => DataCoding.Octet2,
        5 => DataCoding.JIS,
        6 => DataCoding.Cyrllic,
        7 => DataCoding.UCS2,
        8 => DataCoding.Pictogram,
        9 => DataCoding.MusicCodes,
        10 => DataCoding.ExtendedKanji,
        11 => DataCoding.KS_C_5601,
        12 => DataCoding.GSM_MWI_1,
        13 => DataCoding.GSM_MWI_2,
        14 => DataCoding.GMS_MessageClass,
        _ => DataCoding.SMSCDefault,
    };
}

//Numbering plan indicator options method
NumberingPlanIndicator GetNPI(int option)
{
    return option switch
    {
        0 => NumberingPlanIndicator.Unknown,
        1 => NumberingPlanIndicator.ISDN,
        2 => NumberingPlanIndicator.Data,
        3 => NumberingPlanIndicator.Telex,
        4 => NumberingPlanIndicator.LandMobile,
        5 => NumberingPlanIndicator.National,
        6 => NumberingPlanIndicator.Private,
        7 => NumberingPlanIndicator.ERMES,
        8 => NumberingPlanIndicator.Internet,
        9 => NumberingPlanIndicator.WapClient,
        _ => NumberingPlanIndicator.Unknown,
    };
}

//Type of number options method
TypeOfNumber GetTON(int option)
{
    return option switch
    {
        0 => TypeOfNumber.Unknown,
        1 => TypeOfNumber.International,
        2 => TypeOfNumber.National,
        3 => TypeOfNumber.NetworkSpecific,
        4 => TypeOfNumber.SubscriberNumber,
        5 => TypeOfNumber.Alphanumeric,
        6 => TypeOfNumber.Abbreviated,
        _ => TypeOfNumber.Unknown,
    };
}

//Disconnect smpp method
void DisconnectFromSMPP()
{
    if (client.ConnectionState == SmppConnectionState.Connected)
    {
        client.Shutdown();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("SMPP disconnected");
        Console.ResetColor();
    }
}

//Send sms method
void Send(string? msisdn)
{
    if (client.ConnectionState != SmppConnectionState.Connected)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("SMPP not connected");
        Console.ResetColor();
        return;
    }
    bool isNumber = long.TryParse(msisdn, out var number);
    if (!isNumber)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Number format or range is wrong");
        Console.ResetColor();
        return;
    }

    TextMessage msg = new()
    {
        DestinationAddress = $"{number}",
        Text = $"Test from {client.Properties.SystemID} SMPP"
    };
    try
    {
        client.SendMessage(msg);
    }
    catch(Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error!:\n{ex.Message}\nDouble check the smpp configuration.");
        Console.ResetColor();
    }
}

class SmppConfig
{
    public string? Ip { get; set; }
    public int Port { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? SystemType { get; set; }
    public int DataCoding { get; set; }
    public int NPI { get; set; }
    public int TON { get; set; }

}
