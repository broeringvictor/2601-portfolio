using Microsoft.AspNetCore.Components;

namespace WebApp.Components.Shared;

public partial class ExperienceTimeline : ComponentBase
{

    private static readonly List<ExperienceTimeline.ExperienceItem> Experiences =
    [
        new()
        {
            Period = "Set/2025 – Presente",
            Role = "Desenvolvedor Backend",
            Company = "Tributo Devido",
            Url = "https://tributodevido.com.br/",
            Descriptions =
            [
                "<strong class=\"font-medium text-slate-900 dark:text-slate-200\">Engenharia de software e otimização de cloud:</strong> Projeto arquitetura orientada a eventos com RabbitMQ para processamento paralelo de janelas fiscais de 60 meses. Implemento alocação dinâmica de infraestrutura, utilizando pods ajustáveis à volumetria do arquivo (SPED) em substituição a instâncias estáticas (64GB), mantendo custos de AWS e latência reduzidos.",
                "<strong class=\"font-medium text-slate-900 dark:text-slate-200\">Manutenção de sistemas e engenharia de dados:</strong> Sustento sistema legado de processamento híbrido, utilizando Pandas para cálculos em arquivos de texto (TXT) e AWS Athena para transformações em formato colunar (Parquet) sobre estruturas NoSQL. Aplico conhecimento em Direito Tributário na tradução de regras fiscais para código, contribuindo para a recuperação de R$ 2 bilhões em créditos (2025).",
                "<strong class=\"font-medium text-slate-900 dark:text-slate-200\">Interface técnico-jurídica:</strong> Integro a experiência em Direito Tributário ao desenvolvimento de software, atuando na tradução de regras de negócio para a equipe técnica e alinhando os requisitos fiscais à engenharia de dados e automação."
            ],
            Technologies = ["Python", "Pandas", "AWS", "RabbitMQ", "Microservices", "Event-Driven", "Data Engineering", "Parquet", "Athena"]
        },
        new()
        {
            Period = "Nov/2020 – Jul/2025",
            Role = "Desenvolvedor Full Stack e Advogado",
            Company = "Victor Broering Advocacia",
            Url = "https://www.victorbroering.adv.br",
            Descriptions =
            [
                "Liderei a iniciativa de desenvolvimento interno, criando um microssistema de web scraping que gerou uma economia de R$11.000 anuais em licenças de software jurídico.",
                "Desenvolvi e implantei uma solução full stack em .NET que automatizou 100% do processo de geração de contratos, eliminando tarefas manuais e permitindo a delegação segura da precificação.",
                "Integrei sistemas legados que não aceitavam API utilizando Selenium e SQL Server para garantir a consistência e a centralização dos dados da operação."
            ],
            Technologies = [".NET", "C#", "ASP.NET", "Blazor", "Selenium", "SQL Server", "Python"]
        },
        new()
        {
            Period = "01/2020 – 11/2020",
            Role = "Estagiário de Direito",
            Company = "Henrique Franceschetto Advocacia",
            Url = "https://www.henriquef.com.br",
            Descriptions =
            [
                "Otimizei processos internos da equipe ao assumir responsabilidades-chave em um ambiente de metodologia ágil.",
                "Aumentei a velocidade e precisão na identificação de créditos tributários ao automatizar a análise de bi-tributação com Power BI."
            ],
            Technologies = ["Power BI", "Metodologia Ágil"]
        }
    ];

}