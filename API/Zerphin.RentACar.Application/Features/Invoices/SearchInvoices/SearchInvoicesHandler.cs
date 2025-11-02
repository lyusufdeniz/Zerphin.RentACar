using AutoMapper;
using MediatR;
using System.Linq.Expressions;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Invoices.SearchInvoices;

public class SearchInvoicesHandler : IRequestHandler<SearchInvoicesCommand, ServiceResult<SearchInvoicesResponse>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IMapper _mapper;

    public SearchInvoicesHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<SearchInvoicesResponse>> Handle(SearchInvoicesCommand request, CancellationToken cancellationToken)
    {
        Expression<Func<Invoice, bool>>? predicate = null;

        if (request.RentalId.HasValue)
        {
            var rentalId = request.RentalId.Value;
            Expression<Func<Invoice, bool>> rentalFilter = i => i.RentalId == rentalId;
            predicate = predicate == null ? rentalFilter : CombineExpressions(predicate, rentalFilter);
        }

        if (request.IsPaid.HasValue)
        {
            var isPaid = request.IsPaid.Value;
            Expression<Func<Invoice, bool>> paidFilter = i => i.IsPaid == isPaid;
            predicate = predicate == null ? paidFilter : CombineExpressions(predicate, paidFilter);
        }

        if (request.InvoiceDateFrom.HasValue)
        {
            var dateFrom = request.InvoiceDateFrom.Value;
            Expression<Func<Invoice, bool>> dateFromFilter = i => i.InvoiceDate >= dateFrom;
            predicate = predicate == null ? dateFromFilter : CombineExpressions(predicate, dateFromFilter);
        }

        if (request.InvoiceDateTo.HasValue)
        {
            var dateTo = request.InvoiceDateTo.Value;
            Expression<Func<Invoice, bool>> dateToFilter = i => i.InvoiceDate <= dateTo;
            predicate = predicate == null ? dateToFilter : CombineExpressions(predicate, dateToFilter);
        }

        if (!string.IsNullOrWhiteSpace(request.InvoiceNumber))
        {
            var invoiceNumber = request.InvoiceNumber;
            Expression<Func<Invoice, bool>> numberFilter = i => i.InvoiceNumber.Contains(invoiceNumber);
            predicate = predicate == null ? numberFilter : CombineExpressions(predicate, numberFilter);
        }

        Expression<Func<Invoice, object>>? orderByExpression = GetOrderByExpression(request.OrderBy);

        PagedResult<Invoice> pagedResult;
        if (predicate != null && orderByExpression != null)
        {
            pagedResult = await _invoiceRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate,
                orderByExpression,
                request.IsDescending
            );
        }
        else if (predicate != null)
        {
            pagedResult = await _invoiceRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate
            );
        }
        else if (orderByExpression != null)
        {
            pagedResult = await _invoiceRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                i => true,
                orderByExpression,
                request.IsDescending
            );
        }
        else
        {
            pagedResult = await _invoiceRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize
            );
        }

        var response = new SearchInvoicesResponse
        {
            Invoices = _mapper.Map<List<SearchInvoicesResponse.InvoiceDto>>(pagedResult.Data),
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalPages = pagedResult.TotalPages
        };

        return ServiceResult<SearchInvoicesResponse>.Success(response);
    }

    private static Expression<Func<Invoice, bool>> CombineExpressions(
        Expression<Func<Invoice, bool>> first,
        Expression<Func<Invoice, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(Invoice), "i");
        var body = Expression.AndAlso(
            Expression.Invoke(first, parameter),
            Expression.Invoke(second, parameter)
        );
        return Expression.Lambda<Func<Invoice, bool>>(body, parameter);
    }

    private static Expression<Func<Invoice, object>>? GetOrderByExpression(string? orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return i => i.Id;

        return orderBy.ToLower() switch
        {
            "id" => i => (object)i.Id,
            "invoicedate" => i => (object)i.InvoiceDate,
            "totalamount" => i => (object)i.TotalAmount,
            "invoicenumber" => i => (object)i.InvoiceNumber,
            _ => i => (object)i.Id
        };
    }
}

