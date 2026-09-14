using ProJob.Builder;

namespace ProJob.Themes;

public interface IGenerationTemplate
{
    void Apply(IDungeonBuilder builder);
}
