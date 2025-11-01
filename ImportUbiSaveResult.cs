using System.Text.Json.Nodes;

#nullable enable
namespace TCULauncher;

public class ImportUbiSaveResult
{
  public readonly JsonObject? importedSave;
  public readonly string? newSavegameName;
  public readonly bool failedSave;
  public readonly bool failedStats;
  public readonly bool failedScores;
  public readonly bool failedFog;
  public readonly bool failedAchievements;

  public ImportUbiSaveResult(
    JsonObject? importedSave,
    string? newSavegameName,
    bool failedSave,
    bool failedStats,
    bool failedScores,
    bool failedFog,
    bool failedAchievements)
  {
    this.importedSave = importedSave;
    this.newSavegameName = newSavegameName;
    this.failedSave = failedSave;
    this.failedStats = failedStats;
    this.failedScores = failedScores;
    this.failedFog = failedFog;
    this.failedAchievements = failedAchievements;
  }
}
