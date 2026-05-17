namespace AssistenciaTecnica.ViewModels
{
    public class RelatorioViewModel
    {
        public int TotalClientes { get; set; }
        public int TotalServicos { get; set; }
        public int TotalOrdens { get; set; }

        public int OrdensAbertas { get; set; }
        public int OrdensFinalizadas { get; set; }
        public int OrdensCanceladas { get; set; }

        public decimal FaturamentoFinalizado { get; set; }
        public decimal FaturamentoEmAberto { get; set; }
        public decimal TicketMedio { get; set; }

        public List<string> StatusLabels { get; set; } = new();
        public List<int> StatusValores { get; set; } = new();

        public List<string> ServicosLabels { get; set; } = new();
        public List<int> ServicosValores { get; set; } = new();

        public List<string> MesesLabels { get; set; } = new();
        public List<decimal> FaturamentoMesValores { get; set; } = new();
    }
}