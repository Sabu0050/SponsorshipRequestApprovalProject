namespace SponsorshipRequestApprovalProject.Domain.Entities;

public class RequestAttachment
{
    public Guid Id { get; set; }

    public Guid SponsorshipRequestId { get; set; }

    public SponsorshipRequest? SponsorshipRequest { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    public string StoragePath { get; set; } = string.Empty;

    public string UploadedById { get; set; } = string.Empty;

    public string UploadedByName { get; set; } = string.Empty;

    public DateTimeOffset UploadedAt { get; set; }
}
