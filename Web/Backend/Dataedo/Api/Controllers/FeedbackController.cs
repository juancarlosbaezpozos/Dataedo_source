using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.CommunityEditManage;
using Dataedo.Api.Attributes.CommunityView;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Feedback;
using Dataedo.Repository.Services.Features.Feedback.DTO;
using Dataedo.Repository.Services.Features.Feedback.Enums;
using Dataedo.Repository.Services.Features.Feedback.FeedbackComments.DTO;
using Dataedo.Repository.Services.Features.Feedback.FeedbackComments.Enums;
using Dataedo.Repository.Services.Features.Feedback.Interfaces;
using Dataedo.Repository.Services.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FeedbackController : ControllerBase
{
	private readonly IFeedbackService service;

	private readonly IFeedbackWriteService writeService;

	public FeedbackController(IRepositoryAccessManager repositoryAccessManager)
	{
		service = repositoryAccessManager.GetRepository().FeedbackService;
		writeService = repositoryAccessManager.GetRepository().FeedbackWriteService;
	}

	[ProducesResponseType(typeof(FeedbackCommentsDTO), 200)]
	[HttpGet("{id}/comments")]
	public virtual async Task<IActionResult> GetCommentsById([Range(0, int.MaxValue)] int id, [FromQuery] OffsetLimitFilterParameters<FeedbackCommentsEnum.FeedbackCommentFilters> parameters = null)
	{
		return Ok(await service.GetComments(id, parameters));
	}

	/// <summary>
	/// Gets the basic information about feedback.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <returns>The basic information about object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(FeedbackCollection), 200)]
	[HttpGet("all")]
	[ServiceFilter(typeof(RepositoryCommunityViewPermissionAttribute))]
	[ProducesResponseType(typeof(FeedbackDTO), 200)]
	public virtual async Task<IActionResult> Get([FromQuery] FeedbackOrderByColumnEnum orderBy, [FromQuery] CollectionFeedbackParameters<FeedbackFiltersEnum.FeedbackFilters> parameters = null)
	{
		return Ok(await service.GetFeedbacks(orderBy, parameters));
	}

	[HttpPost("rating-summary")]
	public virtual async Task<IActionResult> GetRatingSummary([FromForm][Required] FeedbackLinkedObjectEnum.FeedbackLinkedObjectClassEnum objectClass, [FromForm][Required] int objectId)
	{
		return Ok(await service.GetRatingSummary(objectClass, objectId));
	}

	[ProducesResponseType(typeof(FeedbackDTO), 200)]
	[ServiceFilter(typeof(CommunityAddWithoutLinkedObjectAttribute))]
	[HttpPost("create")]
	public virtual async Task<IActionResult> Post([FromBody] FeedbackCreateDTO feedbackDTO)
	{
		return Ok(await writeService.CreateFeedback(feedbackDTO));
	}

	[ProducesResponseType(typeof(FeedbackDTO), 200)]
	[ServiceFilter(typeof(CommunityAddWithLinkedObjectAttribute))]
	[HttpPost("create/linked-object")]
	public virtual async Task<IActionResult> PostWithLink([FromBody] FeedbackWithLinkedObjectCreateDTO feedbackDTO)
	{
		return Ok(await writeService.CreateFeedbackWithLink(feedbackDTO));
	}

	[ProducesResponseType(typeof(FeedbackDTO), 200)]
	[ServiceFilter(typeof(CommunityEditPermissionAttribute))]
	[HttpPost("update/comment")]
	public virtual async Task<IActionResult> UpdateComment([FromForm] int feedbackId, [FromForm] string comment, [FromForm] byte? rating, [FromForm] bool? resolved, [FromForm] FeedbackTypeEnum.FeedbackType type)
	{
		FeedbackUpdateDTO feedbackDTO = new FeedbackUpdateDTO();
		feedbackDTO.FeedbackID = feedbackId;
		feedbackDTO.Comment = comment ?? "";
		feedbackDTO.Rating = rating;
		feedbackDTO.Resolved = resolved;
		feedbackDTO.Type = type;
		return Ok(await writeService.UpdateFeedbackComment(feedbackDTO));
	}

	[ProducesResponseType(typeof(FeedbackDTO), 200)]
	[ServiceFilter(typeof(CommunityManagePermissionAttribute))]
	[HttpPost("update/status")]
	public virtual async Task<IActionResult> UpdateStatus([FromForm] int feedbackId, [FromForm] string comment, [FromForm] byte? rating, [FromForm] bool? resolved, [FromForm] FeedbackTypeEnum.FeedbackType type)
	{
		FeedbackUpdateDTO feedbackDTO = new FeedbackUpdateDTO();
		feedbackDTO.FeedbackID = feedbackId;
		feedbackDTO.Comment = comment;
		feedbackDTO.Rating = rating;
		feedbackDTO.Resolved = resolved;
		feedbackDTO.Type = type;
		return Ok(await writeService.UpdateFeedbackResolved(feedbackDTO));
	}

	[ServiceFilter(typeof(CommunityManagePermissionAttribute))]
	[HttpPost("{feedbackId}/delete")]
	public virtual async Task<IActionResult> Delete(int feedbackId)
	{
		await writeService.Delete(feedbackId);
		return Ok();
	}

	[ProducesResponseType(typeof(FeedbackDTO), 200)]
	[ServiceFilter(typeof(CommunityManagePermissionAttribute))]
	[HttpPost("convert")]
	public virtual async Task<IActionResult> ConvertType([FromForm] int feedbackId, [FromForm] FeedbackTypeEnum.FeedbackType type)
	{
		FeedbackConvertDTO feedbackDTO = new FeedbackConvertDTO();
		feedbackDTO.FeedbackId = feedbackId;
		feedbackDTO.Type = type;
		return Ok(await writeService.ConvertFeedback(feedbackDTO));
	}

	[ProducesResponseType(typeof(FeedbackDTO), 200)]
	[HttpPost("update/rating")]
	public virtual async Task<IActionResult> UpdateRating([FromForm] int feedbackId, [FromForm] byte? rating)
	{
		FeedbackUpdateDTO feedbackDTO = new FeedbackUpdateDTO();
		feedbackDTO.FeedbackID = feedbackId;
		feedbackDTO.Rating = rating;
		return Ok(await writeService.UpdateRating(feedbackDTO));
	}

	[ProducesResponseType(typeof(FeedbackDTO), 200)]
	[HttpGet("column-ratings")]
	public virtual async Task<IActionResult> GetColumnRatingsByTableId([FromQuery] int tableId)
	{
		return Ok(await service.GetColumnsRatings(tableId));
	}

	[ProducesResponseType(typeof(FeedbackDTO), 200)]
	[HttpGet("trigger-ratings")]
	public virtual async Task<IActionResult> GetTriggerRatingsByTableId([FromQuery] int tableId)
	{
		return Ok(await service.GetTriggerRatings(tableId));
	}
}
