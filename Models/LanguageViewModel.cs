namespace SomaShare.Models
{
    public static class LanguageHelper
    {
        public static string Welcome(string culture)
        {
            return culture switch
            {
                "af" => "Welkom by SomaShare",
                "zu" => "Siyakwamukela ku SomaShare",
                "xh" => "Wamkelekile kwi SomaShare",
                "st" => "Rea u amohela ho SomaShare",
                _ => "Welcome to SomaShare"
            };
        }
    }
}