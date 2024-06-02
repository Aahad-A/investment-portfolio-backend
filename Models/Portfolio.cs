using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_back_end_Aahad_A;
public class Portfolio
{
    public int PortfolioId { get; set; }
    public string Name { get; set; }
    public List<Investment> Investments { get; set; }
    
}

public class Investment
{
    public int InvestmentId { get; set; }
    public string Name { get; set; }
    public string Ticker { get; set; }
    public int Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public int PortfolioId { get; set; } // Foreign key to Portfolio

}