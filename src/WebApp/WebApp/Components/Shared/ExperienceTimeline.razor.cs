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
            Technologies = ["Python", "C# .NET", "Pandas", "AWS (Athena, S3)", "RabbitMQ", "Docker", "Kubernetes", "Microservices", "Event-Driven Architecture", "SQL", "NoSQL", "Parquet", "ETL", "Data Engineering", "Performance Optimization"]
        },
        new()
        {
            Period = "Nov/2020 – Jul/2025",
            Role = "Desenvolvedor Full Stack e Advogado",
            Company = "Victor Broering Advocacia",
            Url = "https://www.victorbroering.adv.br",
            Descriptions =
            [
                "<strong class=\"font-medium text-slate-900 dark:text-slate-200\">Atuação contenciosa e gestão de litígios:</strong> Conduzi o ciclo de vida completo de mais de 100 demandas jurídicas, atuando diretamente na estruturação e resolução de processos de divórcio e ações de alimentos.",
                "<strong class=\"font-medium text-slate-900 dark:text-slate-200\">Desenvolvimento full stack e eficiência operacional:</strong> Projetei e implantei uma solução em .NET para a automação integral (100%) da esteira de geração de contratos. O sistema eliminou intervenções manuais repetitivas e viabilizou a delegação estruturada e segura das regras de precificação.",
                "<strong class=\"font-medium text-slate-900 dark:text-slate-200\">Integração de sistemas legados e consistência de dados:</strong> Implementei uma arquitetura de integração para sistemas sem suporte a APIs, utilizando automação de interface com Selenium e persistência relacional em SQL Server. A engenharia garantiu a centralização, interoperabilidade e consistência dos dados de toda a operação."
            ],
            Technologies = [".NET", "C#", "ASP.NET", "Blazor", "Selenium", "SQL Server", "Python", "D. das Famílias", "D. das Sucessões", "D. Civil"]
        },
        new()
        {
            Period = "01/2020 – 11/2020",
            Role = "Estagiário de Direito",
            Company = "Franceschetto Tributária",
            Url = "https://www.henriquef.com.br",
            Descriptions =
            [
                "<strong class=\"font-medium text-slate-900 dark:text-slate-200\">Atuação técnica em contencioso tributário:</strong> Participei ativamente da elaboração e execução de teses tributárias de alta complexidade, colaborando diretamente com liderança jurídica de referência estadual (SC) na condução de litígios estratégicos e estruturação de defesas fiscais.",
                "<strong class=\"font-medium text-slate-900 dark:text-slate-200\">Otimização de processos e gestão ágil:</strong> Atuei no aprimoramento de fluxos operacionais internos, assumindo a execução de responsabilidades críticas e garantindo a eficiência das rotinas da equipe em um ambiente orientado por metodologias ágeis.",
                "<strong class=\"font-medium text-slate-900 dark:text-slate-200\">Automação analítica e auditoria de créditos:</strong> Projetei e implementei rotinas de análise de dados utilizando Power BI para o mapeamento e cálculo de bitributação. A automação maximizou a precisão e reduziu o tempo de processamento na identificação e validação de créditos tributários."
            ],
            Technologies = ["Power BI", "Metodologia Ágil", "D. Tributário", "Teses Tributárias"]
        }
    ];

}