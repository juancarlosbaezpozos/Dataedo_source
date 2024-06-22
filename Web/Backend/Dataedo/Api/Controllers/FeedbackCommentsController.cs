using System.Threading.Tasks;
using Dataedo.Api.Attributes.CommunityEditManage;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Api.Services;
using Dataedo.Repository.Services.Features.Feedback.DTO;
using Dataedo.Repository.Services.Features.Feedback.Enums;
using Dataedo.Repository.Services.Features.Feedback.FeedbackComments.DTO;
using Dataedo.Repository.Services.Features.Feedback.FeedbackComments.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FeedbackCommentsController : ControllerBase
{
	private readonly IFeedbackCommentsWriteService writeService;

	public FeedbackCommentsController(IRepositoryAccessManager repositoryAccessManager)
	{
		writeService = repositoryAccessManager.GetRepository().FeedbackCommentWriteService;
	}

	[ProducesResponseType(typeof(FeedbackDTO), 200)]
	[ServiceFilter(typeof(CommunityAddCommentPermissionAttribute))]
	[HttpPost("create")]
	public virtual async Task<IActionResult> Post([FromBody] FeedbackCommentCreateDTO input)
	{
		return Ok(await writeService.CreateComment(input, JWTService.GetLogin(base.Request)));
	}

	[ProducesResponseType(typeof(FeedbackDTO), 200)]
	[HttpPost("create-information-comment")]
	public virtual async Task<IActionResult> PostChange([FromBody] FeedbackInformationCommentDTO input)
	{
		return Ok(await writeService.CreateInformationComment(input, JWTService.GetLogin(base.Request)));
	}

	[ProducesResponseType(typeof(FeedbackDTO), 200)]
	[ServiceFilter(typeof(CommunityEditCommentPermissionAttribute))]
	[HttpPost("update")]
	public virtual async Task<IActionResult> Put([FromForm] int feedbackId, [FromForm] string comment, [FromForm] ActionTypeEnum.ActionType action, [FromForm] int commentId)
	{
		FeedbackCommentUpdateDTO input = new FeedbackCommentUpdateDTO();
		input.FeedbackID = feedbackId;
		input.Comment = comment;
		input.Action = action;
		input.CommentId = commentId;
		return Ok(await writeService.UpdateFeedbackComment(input));
	}

	[ProducesResponseType(typeof(FeedbackDTO), 200)]
	[ServiceFilter(typeof(CommunityEditCommentPermissionAttribute))]
	[HttpPost("{commentId}/delete")]
	public virtual async Task<IActionResult> Delete(int commentId)
	{
		await writeService.Delete(commentId);
		return Ok();
	}
}
