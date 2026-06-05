namespace SomaShare.ViewModels
{
    public class TextbookFilterViewModel
    {
        public string? SearchTerm { get; set; }

        public string? Author { get; set; }

        public string? Condition { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string? SortOrder { get; set; }

        public int Page { get; set; } = 1;
    }
}