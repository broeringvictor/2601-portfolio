using Microsoft.AspNetCore.Components;

namespace WebApp.Components.Shared;

public partial class ProfileHero : ComponentBase
{
    private const string S = "font-medium text-slate-900 dark:text-slate-200";

    private static readonly string Bio =
        $"Desenvolvedor Backend e pós-graduando em Engenharia de Software pela USP, " +
        $"especializado na automação de processos fiscais complexos e engenharia de dados. " +
        $"Integro experiência em Direito Tributário com competências técnicas em " +
        $"<strong class=\"{S}\">C#</strong> (<strong class=\"{S}\">.NET</strong>) e " +
        $"<strong class=\"{S}\">Python</strong> para orquestrar pipelines de dados heterogêneos " +
        $"(<strong class=\"{S}\">SQL</strong>, <strong class=\"{S}\">NoSQL</strong> e " +
        $"<strong class=\"{S}\">Parquet</strong>) e desenvolver arquiteturas orientadas a eventos " +
        $"de alta performance e escalabilidade.";
}
