namespace Crawler.Models
{
    public class Bank
    {
        public static string SCB {get; set;} = "https://www.saigontown.com/wp-content/uploads/2020/02/scb.jpg";
        public static string StandardCharted {get; set;} = "https://www.saigontown.com/wp-content/uploads/2020/02/scbank.jpg";
        public static string SHB {get; set;} = "https://www.saigontown.com/wp-content/uploads/2020/02/shb.jpg";
        public static string Techcombank {get; set;} = "https://www.saigontown.com/wp-content/uploads/2020/02/techcombank.jpg";
        public static string VietA {get; set;} = "https://www.saigontown.com/wp-content/uploads/2020/02/thebank_thebank_images918241_logo_1_01_1527523871min_1560303746.jpg";
        public static string TPBank {get; set;} = "https://www.saigontown.com/wp-content/uploads/2020/02/tpbank.jpg";
        public static string ACB {get; set;} = "https://www.saigontown.com/wp-content/uploads/2020/02/acb.png";
        public static string Agribank {get; set;} = "https://www.saigontown.com/wp-content/uploads/2020/02/agribank.jpg";
        public static string BIDV {get; set;} = "https://www.saigontown.com/wp-content/uploads/2020/02/bidv.jpg";
        public static string EXIMBANK {get; set;} = "https://www.saigontown.com/wp-content/uploads/2020/02/eximbank.png";
        public static string GPBank {get; set;} = "https://www.saigontown.com/wp-content/uploads/2020/02/gpbank.png";
        public static string HDBank {get; set;} = "https://www.saigontown.com/wp-content/uploads/2020/02/hdbank.jpg";
        public static string MaritimeBank {get; set;} = "https://www.saigontown.com/wp-content/uploads/2020/02/msb.jpg";
        public static string OCEAN {get; set;} = "https://www.saigontown.com/wp-content/uploads/2020/02/oceanbank-amp.jpg";
        public static string SACOMBank {get; set;} = "https://www.saigontown.com/wp-content/uploads/2020/02/sacombank.jpg";
        public static string SaiGonBank {get; set;} = "https://www.saigontown.com/wp-content/uploads/2020/02/saigonbank.png";
        public static string VPBank {get; set;} = "https://www.saigontown.com/wp-content/uploads/2020/02/vpbank.jpg";
        public static string Vietcombank {get; set;} = "https://www.saigontown.com/wp-content/uploads/2020/02/vietcombank.jpg";
        public static string Vietinbank {get; set;} = "https://www.saigontown.com/wp-content/uploads/2020/02/vietinbank.jpg";

        public static string GetLink(string title) {
            title = title.ToLowerInvariant();
            if (title.Contains("msb"))
            {
                return MaritimeBank;
            } else if (title.Contains("vpbank"))
            {
                return VPBank;
            } else if (title.Contains("vietcombank"))
            {
                return Vietcombank;
            } else if (title.Contains("vietinbank"))
            {
                return Vietinbank;
            } else if (title.Contains("acb"))
            {
                return ACB;
            } else if (title.Contains("tpbank"))
            {
                return TPBank;
            } else if (title.Contains("techcombank"))
            {
                return Techcombank;
            } else if (title.Contains("agribank"))
            {
                return Agribank;
            } else if (title.Contains("saigonbank"))
            {
                return SaiGonBank;
            } else if (title.Contains("sacombank"))
            {
                return SACOMBank;
            } else if (title.Contains("scb"))
            {
                return SCB;
            } else if (title.Contains("vietabank"))
            {
                return VietA;
            } else if (title.Contains("oceanbank"))
            {
                return OCEAN;
            } else if (title.Contains("eximbank"))
            {
                return EXIMBANK;
            } else if (title.Contains("gpbank"))
            {
                return GPBank;
            } else if (title.Contains("bidv"))
            {
                return BIDV;
            } else if (title.Contains("scb"))
            {
                return SCB;
            } else if (title.Contains("hdbank"))
            {
                return HDBank;
            } else {
                return "https://www.saigontown.com/wp-content/uploads/2020/02/bank.jpg";
            }
        }
    }
}