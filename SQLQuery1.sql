drop proc [DBO].[usp_Get_Employees]

CREATE PROC [DBO].[usp_Get_Employees]
AS
BEGIN
	SELECT Id, FIrstName, LastName, DateOfBirth, Email,Salary FROM DBO.Employees WITH (NOLOCK)
END

---------------------------------------------
INSERT INTO [dbo].[Employees] ([FirstName], [LastName], [DateOfBirth], [Email], [Salary])
VALUES ('John', 'Doe', '1985-06-15', 'john.doe@example.com', 75000.00);

INSERT INTO [dbo].[Employees] ([FirstName], [LastName], [DateOfBirth], [Email], [Salary])
VALUES ('Jane', 'Smith', '1990-11-22', 'jane.smith@example.com', 68000.00);

INSERT INTO [dbo].[Employees] ([FirstName], [LastName], [DateOfBirth], [Email], [Salary])
VALUES ('Emily', 'Johnson', '1988-03-30', 'emily.johnson@example.com', 72000.00);

INSERT INTO [dbo].[Employees] ([FirstName], [LastName], [DateOfBirth], [Email], [Salary])
VALUES ('Michael', 'Brown', '1975-09-09', 'michael.brown@example.com', 80000.00);

INSERT INTO [dbo].[Employees] ([FirstName], [LastName], [DateOfBirth], [Email], [Salary])
VALUES ('Linda', 'Davis', '1992-12-05', 'linda.davis@example.com', 69000.00);



------------------------------------------


--GetById
--drop PROC [DBO].[usp_Get_Employees]
CREATE PROC [DBO].[usp_Get_EmployeesByID]
@Id INT
AS
BEGIN
	SELECT Id, FirstName, LastName, DateOfBirth, Email,Salary FROM DBO.Employees WITH (NOLOCK)
	WHERE Id = @Id;
END

--------------------------------------------------------------------------
Create Procedure [dbo].[usp_Insert_Employee]
@FirstName VARCHAR(50),
@LastName VARCHAR(50),
@DateOfBirth DATE,
@Email VARCHAR(50),
@Salary FLOAT

AS
BEGIN
    INSERT INTO DBO.Employees(FirstName,LastName,DateOfBirth,Email,Salary)
    VALUES
    (
    @FirstName,
    @LastName,
    @DateOfBirth,
    @Email,
    @Salary
    )
END

