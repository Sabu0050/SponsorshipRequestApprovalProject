namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;

public record RequestAttachmentDto(
    Guid Id,
    string FileName,
    string ContentType,
    long FileSizeBytes,
    string UploadedByName,
    DateTimeOffset UploadedAt);
