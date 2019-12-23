namespace Qowaiv.Financial.Domain
{
    public class Report
    {
        public Report(Year year, Month month)
        {
            Year = year;
            Month = month;
        }

        public Year Year { get; }
        public Month Month { get; }
    }
}
