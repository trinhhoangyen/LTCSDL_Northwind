/*
-- 30/6/2020
*/
-- Tìm kiếm Order theo CompanyName, EmployeeName
CREATE PROC or_SearchOrder(@keyword nvarchar(50), @page int, @size int)
AS
BEGIN
	declare @begin int, @end int
	set @end = @page * @size
	set @begin = @end - @size + 1

	;with tam as(
		SELECT ROW_NUMBER() OVER(ORDER BY OrderID) AS STT, 
			o.*, e.FirstName as FirstNameEmplpyee, e.LastName as LastNameEmployee,  s.CompanyName
		FROM Orders o, Employees e, Shippers s
		WHERE o.EmployeeID = e.EmployeeID 
			and o.ShipVia = s.ShipperID
			and e.FirstName like '%' + @keyword + '%'
			or e.LastName like '%' + @keyword + '%'
			or s.CompanyName like '%' + @keyword + '%'
	)
	select * from tam where STT between @begin and @end
END
GO
EXEC or_SearchOrder 'Speedy Express', 1, 5
GO
-- Sản phẩm không còn tồn kho
CREATE PROC sp_SPKhongCoTonKho (@page int, @size int)
AS
BEGIN
	declare @begin int, @end int
	set @end = @page * @size
	set @begin = @end - @size + 1

	;with tam as(
		SELECT ROW_NUMBER() OVER(ORDER BY ProductID) AS STT, * FROM Products
		WHERE UnitsInStock = 0
	)
	select * from tam where STT between @begin and @end
END
GO
exec sp_SPKhongCoTonKho 1, 5
GO
-- Sản phẩm không có đơn hàng trong ngày
CREATE PROC pr_ProductsNotOrder (@date datetime, @page int, @size int)
AS
BEGIN
	declare @begin int, @end int
	set @end = @page * @size
	set @begin = @end - @size + 1

	;with tam as(
		SELECT ROW_NUMBER() OVER(ORDER BY ProductID) AS STT, * FROM Products
		WHERE ProductId NOT IN (
			SELECT ProductId FROM Orders
			WHERE OrderDate = @date
		))
	select * from tam where STT between @begin and @end
END
GO
exec pr_ProductsNotOrder '2020/07/01', 1, 5
GO
/*
-- 23/6/2020
*/
-- Thêm
CREATE PROC ncc_AddSupplier
(
	@CompanyName nvarchar(40), 
	@ContactName nvarchar(30), 
	@ContactTitle nvarchar(30), 
	@Address nvarchar(60),
    @City nvarchar(15),
    @Region nvarchar(15),
    @PostalCode nvarchar(10),
    @Country nvarchar(15),
    @Phone nvarchar(24),
    @Fax nvarchar(24),
    @HomePage ntext
)
AS
BEGIN
	INSERT INTO Suppliers 
	VALUES (@CompanyName, @ContactName, @ContactTitle, @Address, @City, @Region, @PostalCode, @Country, @Phone, @Fax, @HomePage)

	SELECT * 
	FROM Suppliers
	WHERE CompanyName = @CompanyName and ContactName = @ContactName and ContactTitle = @ContactTitle
END
GO

EXEC sp_AddSupplier 'Open University', 'Ms. Yen', 'Madam', '92560 SA Cali', '', 'LA', '700000', 'USA', '(84)912-834-740', null, 'https://www.facebook.com/trhgyen'
GO
-- Sửa
CREATE PROC ncc_UpdateSupplier
(
	@SupplierID int,
	@CompanyName nvarchar(40), 
	@ContactName nvarchar(30), 
	@ContactTitle nvarchar(30), 
	@Address nvarchar(60),
    @City nvarchar(15),
    @Region nvarchar(15),
    @PostalCode nvarchar(10),
    @Country nvarchar(15),
    @Phone nvarchar(24),
    @Fax nvarchar(24),
    @HomePage ntext
)
AS
BEGIN
	UPDATE Suppliers 
	SET CompanyName = @CompanyName, 
		ContactName = @ContactName, 
		ContactTitle = @ContactTitle, 
		Address = @Address, 
		City = @City, 
		Region = @Region, 
		PostalCode = @PostalCode, 
		Country = @Country,
		Phone = @Phone,
		Fax = @Fax, 
		HomePage = @HomePage
	WHERE SupplierID = @SupplierID

	SELECT * 
	FROM Suppliers
	WHERE SupplierID = @SupplierID
END
GO
EXEC sp_UpdateSupplier 31, 'Open University', 'Ms. Yen', 'Madam', '92560 SA Cali', '', 'LA', '700000', 'USA', '(84)912-834-740', null, 'https://www.facebook.com/trhgyen'
GO
-- Tìm kiếm
--CREATE PROC ncc_SearchSupplier( @page int, @size int, @kw nvarchar()
/*
-- 16/6/2020
*/
-- Doanh thu theo quoc gia
CREATE PROC dh_DoanhThuTheoQG( @month int, @year int)
AS 
BEGIN
	SELECT c.Country, SUM(od.UnitPrice * od.Quantity * (1-od.Discount)) as DoanhThu
	FROM Orders o, [Order Details] od, Customers c
	WHERE o.OrderID = od.OrderID and o.CustomerID = c.CustomerID
		and MONTH(o.OrderDate) = @month and YEAR(o.OrderDate) = @year
	GROUP BY c.Country
END
GO
EXEC dh_DoanhThuTheoQG 7, 1996
GO

-- Danh sach mat hang ban chay nhat trong khoang thoi gian
CREATE PROC dh_DSMatHangChayNhat(@page int, @size int, @month int, @year int, @isQuantity int)
AS 
BEGIN
	declare @begin int, @end int
	set @end = @page * @size
	set @begin = @end - @size + 1

	-- sap xep theo doanhthu
	if @isQuantity = 1
	Begin
		with tam as(
			SELECT ROW_NUMBER() OVER(ORDER BY p.ProductID) AS STT, 
				p.ProductID, p.ProductName, 
				SUM(od.UnitPrice * od.Quantity * (1-od.Discount)) AS DoanhThu
			FROM Orders o, [Order Details] od, Products p
			WHERE o.OrderID = od.OrderID and od.ProductID = p.ProductID
				and MONTH(o.OrderDate) = @month and YEAR(o.OrderDate) = @year
			GROUP BY p.ProductID, p.ProductName
		)
		SELECT * FROM tam WHERE STT between @begin and @end
		ORDER BY DoanhThu DESC
	End
	-- sap xep theo so luong
	else if @isQuantity = 0
		Begin
			with tam as(
			SELECT ROW_NUMBER() OVER(ORDER BY p.ProductID) AS STT, 
				p.ProductID, p.ProductName, 
				SUM(od.Quantity) AS SoLuong
			FROM Orders o, [Order Details] od, Products p
			WHERE o.OrderID = od.OrderID and od.ProductID = p.ProductID
				and MONTH(o.OrderDate) = @month and YEAR(o.OrderDate) = @year
			GROUP BY p.ProductID, p.ProductName
		)
		SELECT * FROM tam WHERE STT between @begin and @end
		ORDER BY SoLuong DESC
		End
END
GO
EXEC dh_DSMatHangChayNhat 1, 20, 7, 1996, 0
GO

-- Danh sach don hang nhan vien theo khoang thoi gian ( co phan trang)
CREATE PROC dh_DSDHNV (@page int, @size int, @keyword nvarchar(50), @dateFrom datetime, @dateTo datetime)
AS
BEGIN
	declare @begin int, @end int
	set @end = @page * @size
	set @begin = @end - @size + 1
	;with tam as(
		select ROW_NUMBER() over(order by OrderID) AS STT, * 
		from Orders 
		where OrderDate between @dateFrom and @dateTo
			and EmployeeID in (
				SELECT EmployeeID 
				FROM Employees 
				WHERE LastName = @keyword)
	)

	SELECT * FROM tam WHERE STT between @begin and @end
END
GO
exec dh_DSDHNV 1, 5, 'Davolio', '1996-07-06', '1996-09-09'
go

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

--- Chi tiết đơn hàng
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
CREATE PROC kh_NotOrderInMonthYear
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
CREATE PROC DoanhThuTheoNgay
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
CREATE PROC DoanhThuTheoThoiGian
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