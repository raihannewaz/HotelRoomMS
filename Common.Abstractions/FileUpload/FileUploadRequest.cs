namespace Common.Abstractions.FileUpload;

public record FileUploadRequest
{
    public string Name { get; set; } = default!;
    public string Extension { get; set; } = default!;
    public string Data { get; set; } = default!;
    public bool IsBase { get; set; }
    public long Id { get; set; }
}
