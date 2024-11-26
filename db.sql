CREATE TABLE students
(
	studentId INT IDENTITY PRIMARY KEY,
	studentName NVARCHAR(40) NOT NULL,
	dateOfBirth DATE NOT NULL,
	insideDhaka BIT,
	picture NVARCHAR(50) NOT NULL
)
GO
CREATE TABLE courses
(
	courseId INT IDENTITY PRIMARY KEY,
	courseName NVARCHAR(50) NOT NULL,
	startDate DATE NOT NULL,
	endDate DATE NOT NULL,
	courseFee MONEY NOT NULL,
	studentId INT NOT NULL REFERENCES students (studentId)
)