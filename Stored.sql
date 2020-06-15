/*
-- 9/6/2020
*/
--- Lấy danh sách đơn hàng theo thời gian nhập vô có phân trang
CREATE PROC dh_DonHangTheoNgay ( @dateFrom datetime, @dateTo datetime, @page int, @size int)
AS 
BEGIN
	declare @begin int, @end int
	set @end = @page * @size
	set @begin = @end - @size + 1
	;with tam as(
		select ROW_NUMBER() over(order by OrderID) AS STT, * 
		from Orders 
		where OrderDate between @dateFrom and @dateTo)

		select * from tam where STT between @begin and @end
END
GO
exec dh_DonHangTheoNgay '6/7/1996', '9/7/1996', 1, 5
go

--- 
CREATE PROC dH_ChiTietDonHang( @OrderID int)
AS
BEGIN
	SELECT o.OrderID, p.ProductName, o.CustomerID, o.EmployeeID, o.OrderDate ,
	Total=ROUND(Convert(money, od.Quantity * (1 - od.Discount) * od.UnitPrice), 2)
	FROM Orders o, [Order Details] od, Products p
	WHERE o.OrderID = od.OrderID 
		AND od.ProductID = p.ProductID
		AND o.OrderID = @OrderID
END
GO
exec dH_ChiTietDonHang 10248
go

/*
- 26/5/2020
*/
--- Lấy ds KH không phát sinh đơn hàng trong tháng năm truyền vào có phân trang
ALTER PROC kh_NotOrderInMonthYear
(
	@month int, @year int, @page int, @size int
)
AS
BEGIN
	declare @begin int, @end int
	set @end = @page * @size
	set @begin = @end - @size + 1

	;with tam as(
	select ROW_NUMBER() over(order by CustomerID) AS STT, * from Customers c
	where CustomerID not in(
		select CustomerID from Orders o
		where MONTH(OrderDate) = @month and YEAR(OrderDate) = @year
	))

	select * from tam where STT between @begin and @end
END
GO
exec kh_NotOrderInMonthYear 7, 1996, 1, 5

/*
- 2/6/2020
*/
--- Lấy danh sách hóa đơn theo ngày
set dateformat dmy
go
ALTER PROC DoanhThuTheoNgay
(
	@Date datetime
)
AS
BEGIN
	SELECT e.EmployeeID, e.LastName, e.FirstName, 
			sum(od.UnitPrice * od.Quantity * (1-od.Discount)) as DoanhThu 
	FROM Employees e inner join Orders o on e.EmployeeID = o.EmployeeID
					inner join [Order Details] od on o.OrderID = od.OrderID
	WHERE MONTH(@Date) = MONTH(o.OrderDate) and
			DAY(@Date) = DAY(o.OrderDate) and
			YEAR(@Date) = YEAR(o.OrderDate)
	GROUP BY e.EmployeeID, e.LastName, e.FirstName
END
GO
exec DoanhThuTheoNgay '5-7-1996'
go

--- Lấy danh sách hóa đơn trong khoảng thời gian truyền vô
ALTER PROC DoanhThuTheoThoiGian
(
	@begintime datetime, @endTime datetime
)
AS
BEGIN
	SELECT e.EmployeeID, e.LastName, e.FirstName, sum(od.UnitPrice * od.Quantity * (1-od.Discount)) as DoanhThu 
	FROM Employees e inner join Orders o on e.EmployeeID = o.EmployeeID
					inner join [Order Details] od on o.OrderID = od.OrderID
	WHERE o.OrderDate between @begintime and @endTime
	group BY e.EmployeeID, e.LastName, e.FirstName
END
GO
exec DoanhThuTheoThoiGian '7/6/1997', '7/9/1997'
go