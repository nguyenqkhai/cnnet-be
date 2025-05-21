using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseWeb.API.Migrations
{
    /// <inheritdoc />
    public partial class IncreasedPasswordLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {   
            // Kiểm tra xem bảng Contacts đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Contacts') 
                BEGIN
                    CREATE TABLE [Contacts] (
                        [id] bigint NOT NULL IDENTITY(1,1),
                        [name] nvarchar(100) NOT NULL,
                        [email] nvarchar(255) NOT NULL,
                        [phone] nvarchar(20) NULL,
                        [message] nvarchar(max) NOT NULL,
                        [createdAt] datetime NOT NULL DEFAULT (getdate()),
                        [updatedAt] datetime NULL,
                        [_destroy] bit NOT NULL,
                        CONSTRAINT [PK__Contacts__3213E83FFB27E44D] PRIMARY KEY ([id])
                    );
                END");
            

            // Kiểm tra xem bảng Courses đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Courses') 
                BEGIN
                    CREATE TABLE [Courses] (
                        [id] bigint NOT NULL IDENTITY(1,1),
                        [name] nvarchar(255) NOT NULL,
                        [description] nvarchar(max) NULL,
                        [thumbnail] nvarchar(255) NULL,
                        [instructor] nvarchar(100) NOT NULL,
                        [instructorRole] nvarchar(100) NULL,
                        [instructorDescription] nvarchar(max) NULL,
                        [duration] float NULL,
                        [price] int NOT NULL,
                        [discount] int NULL DEFAULT 0,
                        [students] int NOT NULL,
                        [courseModules] nvarchar(max) NULL,
                        [createdAt] datetime NOT NULL DEFAULT (getdate()),
                        [updatedAt] datetime NULL,
                        [_destroy] bit NOT NULL,
                        CONSTRAINT [PK__Courses__3213E83FA2BA6E4D] PRIMARY KEY ([id])
                    );
                END");
            

            // Kiểm tra xem bảng User đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'User') 
                BEGIN
                    CREATE TABLE [User] (
                        [id] bigint NOT NULL IDENTITY(1,1),
                        [email] nvarchar(255) NOT NULL,
                        [password] nvarchar(max) NOT NULL,
                        [username] nvarchar(255) NOT NULL,
                        [avatar] nvarchar(255) NULL,
                        [role] nvarchar(50) NOT NULL DEFAULT 'student',
                        [createdAt] datetime NOT NULL DEFAULT (getdate()),
                        [updatedAt] datetime NULL,
                        [_destroy] bit NOT NULL,
                        CONSTRAINT [PK__User__3213E83FB51CFA82] PRIMARY KEY ([id])
                    );
                END");
            

            // Kiểm tra xem bảng Vouchers đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Vouchers') 
                BEGIN
                    CREATE TABLE [Vouchers] (
                        [id] bigint NOT NULL IDENTITY(1,1),
                        [name] nvarchar(100) NOT NULL,
                        [code] nvarchar(50) NOT NULL,
                        [discount] int NOT NULL,
                        [courseIds] nvarchar(max) NULL,
                        [usageLimit] int NULL,
                        [usedCount] int NOT NULL,
                        [minOrderValue] int NULL,
                        [expiredAt] datetime NULL,
                        [createdAt] datetime NOT NULL DEFAULT (getdate()),
                        [updatedAt] datetime NULL,
                        [_destroy] bit NOT NULL,
                        CONSTRAINT [PK__Vouchers__3213E83F681B94E8] PRIMARY KEY ([id])
                    );
                END");
            

            // Kiểm tra xem bảng Lessons đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Lessons') 
                BEGIN
                    CREATE TABLE [Lessons] (
                        [id] bigint NOT NULL IDENTITY(1,1),
                        [name] nvarchar(255) NOT NULL,
                        [video_url] nvarchar(255) NULL,
                        [courseId] bigint NOT NULL,
                        [coursePart] nvarchar(100) NULL,
                        [createdAt] datetime NOT NULL DEFAULT (getdate()),
                        [updatedAt] datetime NULL,
                        [_destroy] bit NOT NULL,
                        CONSTRAINT [PK__Lessons__3213E83F3069372B] PRIMARY KEY ([id]),
                        CONSTRAINT [FK_Lessons_Courses] FOREIGN KEY ([courseId]) REFERENCES [Courses] ([id])
                    );
                END");
            

            // Kiểm tra xem bảng Modules đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Modules') 
                BEGIN
                    CREATE TABLE [Modules] (
                        [id] bigint NOT NULL IDENTITY(1,1),
                        [title] nvarchar(255) NOT NULL,
                        [description] nvarchar(max) NULL,
                        [duration] float NULL,
                        [lessons] nvarchar(max) NULL,
                        [courseId] bigint NOT NULL,
                        [createdAt] datetime NOT NULL DEFAULT (getdate()),
                        [updatedAt] datetime NULL,
                        [_destroy] bit NOT NULL,
                        CONSTRAINT [PK__Modules__3213E83F103A4830] PRIMARY KEY ([id]),
                        CONSTRAINT [FK_Modules_Courses] FOREIGN KEY ([courseId]) REFERENCES [Courses] ([id])
                    );
                END");
            

            // Kiểm tra xem bảng Blogs đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Blogs') 
                BEGIN
                    CREATE TABLE [Blogs] (
                        [id] bigint NOT NULL IDENTITY(1,1),
                        [title] nvarchar(255) NOT NULL,
                        [summary] nvarchar(500) NULL,
                        [content] nvarchar(max) NOT NULL,
                        [tags] nvarchar(max) NULL,
                        [coverImage] nvarchar(255) NULL,
                        [author] nvarchar(100) NOT NULL,
                        [authorId] bigint NOT NULL,
                        [createdAt] datetime NOT NULL DEFAULT (getdate()),
                        [updatedAt] datetime NULL,
                        [_destroy] bit NOT NULL,
                        CONSTRAINT [PK__Blogs__3213E83F47DB6F82] PRIMARY KEY ([id]),
                        CONSTRAINT [FK_Blogs_User] FOREIGN KEY ([authorId]) REFERENCES [User] ([id])
                    );
                END");
            

            // Kiểm tra xem bảng Carts đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Carts') 
                BEGIN
                    CREATE TABLE [Carts] (
                        [id] bigint NOT NULL IDENTITY(1,1),
                        [userId] bigint NOT NULL,
                        [courseId] bigint NOT NULL,
                        [courseName] nvarchar(255) NOT NULL,
                        [courseThumbnail] nvarchar(255) NULL,
                        [instructor] nvarchar(100) NOT NULL,
                        [duration] float NULL,
                        [totalPrice] int NOT NULL,
                        [totalLessons] int NOT NULL,
                        [totalReviews] int NOT NULL,
                        [rating] int NULL,
                        [createdAt] datetime NOT NULL DEFAULT (getdate()),
                        [updatedAt] datetime NULL,
                        [_destroy] bit NOT NULL,
                        CONSTRAINT [PK__Carts__3213E83F1A53FE3A] PRIMARY KEY ([id]),
                        CONSTRAINT [FK_Carts_Courses] FOREIGN KEY ([courseId]) REFERENCES [Courses] ([id]),
                        CONSTRAINT [FK_Carts_User] FOREIGN KEY ([userId]) REFERENCES [User] ([id])
                    );
                END");
            

            // Kiểm tra xem bảng Orders đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders') 
                BEGIN
                    CREATE TABLE [Orders] (
                        [id] bigint NOT NULL IDENTITY(1,1),
                        [userId] bigint NOT NULL,
                        [courseId] bigint NOT NULL,
                        [userEmail] nvarchar(255) NOT NULL,
                        [userName] nvarchar(100) NOT NULL,
                        [courseName] nvarchar(255) NOT NULL,
                        [courseThumbnail] nvarchar(255) NULL,
                        [instructor] nvarchar(100) NOT NULL,
                        [totalPrice] int NOT NULL,
                        [paymentMethod] nvarchar(50) NOT NULL,
                        [status] nvarchar(50) NOT NULL DEFAULT 'pending',
                        [createdAt] datetime NOT NULL DEFAULT (getdate()),
                        [updatedAt] datetime NULL,
                        [_destroy] bit NOT NULL,
                        CONSTRAINT [PK__Orders__3213E83FCB2047AA] PRIMARY KEY ([id]),
                        CONSTRAINT [FK_Orders_Courses] FOREIGN KEY ([courseId]) REFERENCES [Courses] ([id]),
                        CONSTRAINT [FK_Orders_User] FOREIGN KEY ([userId]) REFERENCES [User] ([id])
                    );
                END");
            

            // Kiểm tra xem bảng Progress đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Progress') 
                BEGIN
                    CREATE TABLE [Progress] (
                        [id] bigint NOT NULL IDENTITY(1,1),
                        [userId] bigint NOT NULL,
                        [courseId] bigint NOT NULL,
                        [completedLessons] nvarchar(max) NULL,
                        [totalLessons] int NOT NULL,
                        [percentComplete] int NOT NULL,
                        [lastAccessedAt] datetime NULL,
                        CONSTRAINT [PK__Progress__3213E83FC7CA75F9] PRIMARY KEY ([id]),
                        CONSTRAINT [FK_Progress_Course] FOREIGN KEY ([courseId]) REFERENCES [Courses] ([id]),
                        CONSTRAINT [FK_Progress_User] FOREIGN KEY ([userId]) REFERENCES [User] ([id])
                    );
                END");
            

            // Kiểm tra xem bảng Reviews đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Reviews') 
                BEGIN
                    CREATE TABLE [Reviews] (
                        [id] bigint NOT NULL IDENTITY(1,1),
                        [userId] bigint NOT NULL,
                        [userAvatar] nvarchar(255) NULL,
                        [userName] nvarchar(100) NOT NULL,
                        [courseId] bigint NOT NULL,
                        [content] nvarchar(max) NULL,
                        [rating] int NOT NULL,
                        [createdAt] datetime NOT NULL DEFAULT (getdate()),
                        [updatedAt] datetime NULL,
                        [_destroy] bit NOT NULL,
                        CONSTRAINT [PK__Reviews__3213E83F12AC6C80] PRIMARY KEY ([id]),
                        CONSTRAINT [FK_Reviews_Courses] FOREIGN KEY ([courseId]) REFERENCES [Courses] ([id]),
                        CONSTRAINT [FK_Reviews_User] FOREIGN KEY ([userId]) REFERENCES [User] ([id])
                    );
                END");
            

            // Kiểm tra xem index IX_Blogs_authorId đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Blogs_authorId' AND object_id = OBJECT_ID('Blogs'))
                BEGIN
                    CREATE INDEX [IX_Blogs_authorId] ON [Blogs] ([authorId]);
                END");
            

            // Kiểm tra xem index IX_Carts_courseId đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Carts_courseId' AND object_id = OBJECT_ID('Carts'))
                BEGIN
                    CREATE INDEX [IX_Carts_courseId] ON [Carts] ([courseId]);
                END");
            

            // Kiểm tra xem index IX_Carts_userId đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Carts_userId' AND object_id = OBJECT_ID('Carts'))
                BEGIN
                    CREATE INDEX [IX_Carts_userId] ON [Carts] ([userId]);
                END");
            

            // Kiểm tra xem index IX_Course_Name đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Course_Name' AND object_id = OBJECT_ID('Courses'))
                BEGIN
                    CREATE INDEX [IX_Course_Name] ON [Courses] ([name]);
                END");
            

            // Kiểm tra xem index IX_Lessons_Course đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Lessons_Course' AND object_id = OBJECT_ID('Lessons'))
                BEGIN
                    CREATE INDEX [IX_Lessons_Course] ON [Lessons] ([courseId]);
                END");
            

            // Kiểm tra xem index IX_Modules_Course đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Modules_Course' AND object_id = OBJECT_ID('Modules'))
                BEGIN
                    CREATE INDEX [IX_Modules_Course] ON [Modules] ([courseId]);
                END");
            

            // Kiểm tra xem index IX_Orders_courseId đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_courseId' AND object_id = OBJECT_ID('Orders'))
                BEGIN
                    CREATE INDEX [IX_Orders_courseId] ON [Orders] ([courseId]);
                END");
            

            // Kiểm tra xem index IX_Orders_UserCourse đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_UserCourse' AND object_id = OBJECT_ID('Orders'))
                BEGIN
                    CREATE INDEX [IX_Orders_UserCourse] ON [Orders] ([userId], [courseId]);
                END");
            

            // Kiểm tra xem index IX_Progress_courseId đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Progress_courseId' AND object_id = OBJECT_ID('Progress'))
                BEGIN
                    CREATE INDEX [IX_Progress_courseId] ON [Progress] ([courseId]);
                END");
            

            // Kiểm tra xem index IX_Progress_userId đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Progress_userId' AND object_id = OBJECT_ID('Progress'))
                BEGIN
                    CREATE INDEX [IX_Progress_userId] ON [Progress] ([userId]);
                END");
            

            // Kiểm tra xem index IX_Reviews_Course đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Reviews_Course' AND object_id = OBJECT_ID('Reviews'))
                BEGIN
                    CREATE INDEX [IX_Reviews_Course] ON [Reviews] ([courseId]);
                END");
            

            // Kiểm tra xem index IX_Reviews_userId đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Reviews_userId' AND object_id = OBJECT_ID('Reviews'))
                BEGIN
                    CREATE INDEX [IX_Reviews_userId] ON [Reviews] ([userId]);
                END");
            

            // Kiểm tra xem index IX_User_Email đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_User_Email' AND object_id = OBJECT_ID('User'))
                BEGIN
                    CREATE INDEX [IX_User_Email] ON [User] ([email]);
                END");
            

            // Kiểm tra xem index IX_User_Username đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_User_Username' AND object_id = OBJECT_ID('User'))
                BEGIN
                    CREATE INDEX [IX_User_Username] ON [User] ([username]);
                END");
            

            // Kiểm tra xem index UQ__Vouchers__357D4CF97C488B16 đã tồn tại chưa trước khi tạo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ__Vouchers__357D4CF97C488B16' AND object_id = OBJECT_ID('Vouchers'))
                BEGIN
                    CREATE UNIQUE INDEX [UQ__Vouchers__357D4CF97C488B16] ON [Vouchers] ([code]);
                END");
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blogs");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Progress");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Vouchers");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
