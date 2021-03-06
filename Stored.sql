﻿/*
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

	SELECT * FROM tam WHERE STT between @begin and @end
END
GO
EXEC kh_NotOrderInMonthYear 7, 1996, 1, 5
GO
/* đề 5
-- 30/6/2020
*/
-- 5a. Sản phẩm không có đơn hàng trong ngày
CREATE PROC sp_ProductsNotOrder (@date datetime, @page int, @size int)
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
exec sp_ProductsNotOrder '2020/07/01', 1, 5
GO
-- 5b. Sản phẩm không còn tồn kho
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
-- 5c. Tìm kiếm Order theo CompanyName, EmployeeName
CREATE PROC hd_SearchOrder(@keyword nvarchar(50), @page int, @size int)
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
EXEC hd_SearchOrder 'Speedy Express', 1, 5
GO

/* đề 4
-- 23/6/2020
*/
-- 4a. Thêm nhà cung cấp
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
EXEC ncc_AddSupplier 'Open University', 'Ms. Yen', 'Madam', '92560 SA Cali', '', 'LA', '700000', 'USA', '(84)912-834-740', null, 'https://www.facebook.com/trhgyen'
GO
-- 4b. Sửa nhà cung cấp
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
EXEC ncc_UpdateSupplier 31, 'Open University', 'Ms. Yen', 'Madam', '92560 SA Cali', '', 'LA', '700000', 'USA', '(84)912-834-740', null, 'https://www.facebook.com/trhgyen'
GO
-- 4c. Tìm nhà cung cấp theo CompanyName, Country
CREATE PROC ncc_SearchSupplier( @page int, @size int, @keyword nvarchar(50))
AS
BEGIN
	declare @begin int, @end int
	set @end = @page * @size
	set @begin = @end - @size + 1;

	WITH tam AS(
			SELECT ROW_NUMBER() OVER(ORDER BY SupplierID) AS STT, * 
			FROM Suppliers
			WHERE CompanyName like'%' + @keyword + '%'
				OR Country like'%' + @keyword + '%' )
	SELECT * FROM tam
	WHERE STT between @begin and @end
END
GO
EXEC ncc_SearchSupplier 1 ,5, 'Tokyo'
GO
/* đề 3
-- 16/6/2020
*/
-- 3a. Danh sach don hang nhan vien trong khoang thoi gian ( co phan trang)
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
-- 3b. Danh sach mat hang ban chay nhat trong khoang thoi gian
CREATE PROC dh_DSSPBanChay(@page int, @size int, @month int, @year int, @isQuantity int)
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
EXEC dh_DSSPBanChay 1, 5, 7, 1996, 1
GO
-- 3c. Doanh thu theo quoc gia
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

/* Đề 2
-- 9/6/2020
*/
--- 2a. Lấy danh sách đơn hàng theo thời gian nhập vô có phân trang
CREATE PROC dh_DonHangTheoThoiGian ( @dateFrom datetime, @dateTo datetime, @page int, @size int)
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
exec dh_DonHangTheoThoiGian '1996-07-05', '1996-07-09', 1, 5
go
--- 2b. Chi tiết đơn hàng
CREATE PROC dh_ChiTietDonHang( @OrderID int)
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
exec dh_ChiTietDonHang 10248
go

/* Đề 1
- 2/6/2020
*/
--- 1a. Lấy danh sách hóa đơn theo ngày
alter PROC nv_DoanhThuNVTheoNgay( @Date datetime )
AS
BEGIN
	SELECT e.EmployeeID, e.LastName, e.FirstName, 
			sum(od.UnitPrice * od.Quantity * (1-od.Discount)) as DoanhThu 
	FROM Employees e inner join Orders o on e.EmployeeID = o.EmployeeID
					inner join [Order Details] od on o.OrderID = od.OrderID
	WHERE o.OrderDate = @Date
	GROUP BY e.EmployeeID, e.LastName, e.FirstName
END
GO
exec nv_DoanhThuNVTheoNgay '1996-07-05'
go
--- 1b. Lấy danh sách hóa đơn trong khoảng thời gian truyền vô
CREATE PROC nv_DoanhThuNVTheoThoiGian
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
exec nv_DoanhThuNVTheoThoiGian '1997-07-06', '1997-07-09'
go