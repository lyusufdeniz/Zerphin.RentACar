using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Invoices.GetInvoiceById;

public class GetInvoiceByIdHandler : IRequestHandler<GetInvoiceByIdCommand, ServiceResult<GetInvoiceByIdResponse>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IMapper _mapper;

    public GetInvoiceByIdHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<GetInvoiceByIdResponse>> Handle(GetInvoiceByIdCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.Id);
        
        if (invoice == null)
        {
            return ServiceResult<GetInvoiceByIdResponse>.Fail($"Invoice with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        var response = _mapper.Map<GetInvoiceByIdResponse>(invoice);
        return ServiceResult<GetInvoiceByIdResponse>.Success(response);
    }
}

