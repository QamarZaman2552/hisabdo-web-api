namespace HisabDo.Application.DTOs;

public class UploadSettings
{
    public long MaxSizeBytes { get; set; } = 10 * 1024 * 1024;
    public string[] AllowedExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".gif", ".pdf"];
    public string Directory { get; set; } = "wwwroot/uploads";
}
