using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.NetworkInformation;
using System.IO;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;

namespace Licencse;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        LoadInfor();
    }

    private string? dayUsed;
    private string? appID;
    private string? macAddress;
    private string pathLicense = "license.lic";
    // Lấy tất cả MAC (đã format) của các adapter
    public static List<string> GetAllMacAddresses(bool onlyUp = true)
    {
        List<string> result = new List<string>();

        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces(); // [page:1]
        for (int i = 0; i < nics.Length; i++)
        {
            NetworkInterface nic = nics[i];

            if (nic.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                continue;

            if (onlyUp && nic.OperationalStatus != OperationalStatus.Up)
                continue;

            PhysicalAddress pa = nic.GetPhysicalAddress(); // [page:0]
            string mac = FormatMac(pa);

            if (!string.IsNullOrEmpty(mac) && !result.Contains(mac))
                result.Add(mac);
        }

        return result;
    }
    private static string FormatMac(PhysicalAddress pa)
    {
        if (pa == null) return null;

        byte[] bytes = pa.GetAddressBytes();
        if (bytes == null || bytes.Length == 0) return null;

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            if (i > 0) sb.Append("-");
            sb.Append(bytes[i].ToString("X2"));
        }
        return sb.ToString();
    }
    private void LoadInfor()
    {
        List<string> macAddress = GetAllMacAddresses();
        cbx_macaddress.ItemsSource = macAddress;
        cbx_day.ItemsSource = new List<string>() { "1","7","30","365"};
    }
        
    private void btn_genlicense_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            appID = tb_appid.Text;
            if (dayUsed == string.Empty) return;
            if (appID == string.Empty) return;
            if (macAddress == string.Empty) return;

            string dayUsedEncode = EncryptData.Encode(dayUsed);
            string dayUsedDecode = EncryptData.Decode(dayUsedEncode);
            bool comparisonDay = string.Equals(dayUsed, dayUsedDecode);

            string macAddressEncode = EncryptData.Encode(macAddress);
            string macAddressDecode = EncryptData.Decode(macAddressEncode);
            bool ComparisonMac = string.Equals(macAddress, macAddressDecode);

            string appIdEncode = EncryptData.Encode(appID);
            string appIdDecode = EncryptData.Decode(appIdEncode);
            bool ComparisonAppId = string.Equals(appID, appIdDecode);

            /*
                             [enableFeature1 , enableFeature2, enableFeature3 ,enableFeature4 ]
                               */
            string enableFeature1Encode = EncryptData.Encode("InspectHighSpeed:1000");
            string enableFeature1Decode = EncryptData.Decode(enableFeature1Encode);

            string[] licenses =
                {
                "*************************** ABC Company ***************************",
                "License Copy@ Right by ABC Company 2026",
                macAddressEncode,
                appIdEncode,
                dayUsedEncode,
                enableFeature1Encode,
                "*******************************************************************"
                };

            File.WriteAllLines(pathLicense, licenses);
            MessageBox.Show("Gen License Successfully ...");

        }
        catch (Exception ex)
        {
            string error = ex.ToString();
        }
        finally
        {
            string temp = "";
        }
    }

    private void cbx_day_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ComboBox cbx = (ComboBox)(sender);
        if (cbx!=null)
        {
            dayUsed = cbx.SelectedItem.ToString();
        }
    }

    private void cbx_macaddress_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ComboBox cbx = (ComboBox)(sender);
        if (cbx != null)
        {
            macAddress = cbx.SelectedValue.ToString();
        }
    }

    private void btn_loadlicense_Click(object sender, RoutedEventArgs e)
    {
        bool validLicese = ValidLicense();
        if (!validLicese)
        {
            MessageBox.Show("Application Shutdown Immeditately= ...");
            Application.Current.Shutdown(1);
        }
        else
            MessageBox.Show("License Valid\nApplication is Lauching");

        string a = "10";
    }
    private bool ValidLicense()
    {
        if (!File.Exists(pathLicense))
        {
            MessageBox.Show("License file not found");
            return false;   
        }

        string[] licInfor = File.ReadAllLines(pathLicense);
        if (licInfor.Length != 7) {
            MessageBox.Show("License Content Invalid!");
            return false;
        }

        string macAddressEncode = licInfor[2];
        string macAddressDecode = EncryptData.Decode(macAddressEncode);
 
        if (!GetAllMacAddresses().Contains(macAddressDecode))
        {
            MessageBox.Show("License MacAddress Invalid");
            return false;
        }

        string appIdEncode = licInfor[3];
        string appIdDecode = EncryptData.Decode(appIdEncode);
        string processName = Process.GetCurrentProcess().ProcessName;
        if(!string.Equals(appIdDecode, processName))
        {
            MessageBox.Show("Process Name Invalid");
            return false;
        }

        /* Load từ registry lên */
        string activeSrt = "28-01-2026";
        DateTime outActivate;

        bool ok = DateTime.TryParseExact(
            activeSrt,
            "dd-MM-yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out outActivate);
        if (!ok)
        {
            MessageBox.Show("Activate Invalid");
            return false;
        }

        string dayUsedEncode = licInfor[4];
        string dayUsedDecode = EncryptData.Decode(dayUsedEncode);

        DateTime currentDay = DateTime.Now;
        int dayleftUsed = (int)(currentDay - outActivate).TotalDays;
        if(dayleftUsed > int.Parse(dayUsedDecode))
        {
            MessageBox.Show("license Expired !!!");
            return false;
        }

        return true;
    }
}