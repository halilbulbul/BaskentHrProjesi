using Application.Features.UserOperationClaims.Constants;
using Application.Features.UserOperationClaims.Rules;
using Application.Services.Repositories;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Persistence.Paging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Application.Features.UserOperationClaims.Constants.UserOperationClaimsOperationClaims;

namespace Application.Features.UserOperationClaims.Commands.Update;

public class UpdateUserOperationClaimCommand : IRequest<UpdatedUserOperationClaimResponse>, ISecuredRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public int OperationClaimId { get; set; }

    public int[]? OperationClaimIds { get; set; }

    public string[] Roles => new[] { Admin, Write, UserOperationClaimsOperationClaims.Update };

    public class UpdateUserOperationClaimCommandHandler
        : IRequestHandler<UpdateUserOperationClaimCommand, UpdatedUserOperationClaimResponse>
    {
        private readonly IUserOperationClaimRepository _userOperationClaimRepository;
        private readonly UserOperationClaimBusinessRules _userOperationClaimBusinessRules;

        public UpdateUserOperationClaimCommandHandler(
            IUserOperationClaimRepository userOperationClaimRepository,
            UserOperationClaimBusinessRules userOperationClaimBusinessRules
        )
        {
            _userOperationClaimRepository = userOperationClaimRepository;
            _userOperationClaimBusinessRules = userOperationClaimBusinessRules;
        }

        public async Task<UpdatedUserOperationClaimResponse> Handle(
            UpdateUserOperationClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            int[] selectedIds =
                (request.OperationClaimIds != null && request.OperationClaimIds.Length > 0)
                    ? request.OperationClaimIds
                    : (request.OperationClaimId > 0 ? new[] { request.OperationClaimId } : Array.Empty<int>());

            selectedIds = selectedIds
                .Where(x => x > 0)
                .Distinct()
                .ToArray();

            IPaginate<UserOperationClaim> userClaims = await _userOperationClaimRepository.GetListAsync(
                predicate: uoc => uoc.UserId == request.UserId,
                index: 0,
                size: int.MaxValue,
                enableTracking: true,
                cancellationToken: cancellationToken
            );

            foreach (UserOperationClaim claim in userClaims.Items.ToList())
            {
                await _userOperationClaimRepository.DeleteAsync(
                    claim,
                    permanent: false,
                    cancellationToken: cancellationToken
                );
            }

            Guid firstInsertedId = Guid.Empty;
            int firstInsertedClaimId = 0;

            foreach (int claimId in selectedIds)
            {
                var entity = new UserOperationClaim
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    OperationClaimId = claimId
                };

                var inserted = await _userOperationClaimRepository.AddAsync(entity, cancellationToken);

                if (firstInsertedId == Guid.Empty)
                {
                    firstInsertedId = inserted.Id;
                    firstInsertedClaimId = inserted.OperationClaimId;
                }
            }

            return new UpdatedUserOperationClaimResponse
            {
                UserId = request.UserId,
                OperationClaimIds = selectedIds,

                Id = firstInsertedId,
                OperationClaimId = firstInsertedClaimId
            };
        }
    }
}
