using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelRoomMS.Application.Bookings.Features.GetBookingNumbers;

public record GetBookingNumber : IRequest<GetBookingNumberResponse>;

internal class GetBookingNumberHandler : IRequestHandler<GetBookingNumber, GetBookingNumberResponse>
{
    private readonly IDbConnectionCreator _connectionCreator;

    public GetBookingNumberHandler(IDbConnectionCreator connectionCreator)
    {
        _connectionCreator = connectionCreator;
    }

    public async Task<GetBookingNumberResponse> Handle(GetBookingNumber request, CancellationToken cancellationToken)
    {
		string fn = @"CREATE OR ALTER  FUNCTION [accounts].[fn_generate_voucher_no](@fy_id bigint)
RETURNS VARCHAR(100)
AS
BEGIN
    DECLARE @year_month VARCHAR(10) = FORMAT(GETDATE(), 'yyyy/MM');
    DECLARE @serial INT;
    DECLARE @padded_serial VARCHAR(10);
    DECLARE @result VARCHAR(100);

	select top 1 @serial = ISNULL(SUBSTRING(reference_no, 2, CHARINDEX('-', reference_no) - 2),0) 
	from [accounts].[journals_master]
	WHERE financial_year_id = @fy_id AND MONTH(created)=MONTH(GETDATE())
	order by created desc

	if(@serial = '' OR @serial is null)
		set @serial = 0

	if((@serial + 1) > 99999)
		SET @padded_serial = CAST(@serial + 1 AS VARCHAR(10))
    else
		SET @padded_serial = RIGHT('00000' + CAST(@serial + 1 AS VARCHAR(5)), 5);

    SET @result = '#' + @padded_serial + '-'  + @year_month;
	

	DECLARE @Counter INT 
	SET @Counter = 0

	WHILE (@Counter = 0)
	BEGIN
		if exists (select 1 from [accounts].[journals_master] where reference_no = @result)
		BEGIN
			SET @Counter  =  0
			set @serial =  @serial + 1
			if((@serial + 1) > 99999)
				SET @padded_serial = CAST(@serial + 1 AS VARCHAR(10))
			else
				SET @padded_serial = RIGHT('00000' + CAST(@serial + 1 AS VARCHAR(5)), 5);

			SET @result = '#' + @padded_serial + '-'  + @year_month;
		END
		ELSE
		BEGIN
			SET @Counter  =  1
		END
	END

    RETURN @result;
END;";
        return new GetBookingNumberResponse("");
    }
}