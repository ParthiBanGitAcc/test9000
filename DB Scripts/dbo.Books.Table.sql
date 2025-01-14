USE [BookRentalDB]
GO
/****** Object:  Table [dbo].[Books]    Script Date: 27-10-2024 12:25:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Books](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Author] [nvarchar](max) NOT NULL,
	[ISBN] [nvarchar](max) NOT NULL,
	[Genre] [nvarchar](max) NOT NULL,
	[IsAvailable] [bit] NOT NULL,
	[RentalCount] [int] NOT NULL,
 CONSTRAINT [PK_Books] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Books] ON 

INSERT [dbo].[Books] ([Id], [Title], [Author], [ISBN], [Genre], [IsAvailable], [RentalCount]) VALUES (1, N'The Great Gatsby', N'F. Scott Fitzgerald', N'9780743273565', N'Classics', 1, 2)
INSERT [dbo].[Books] ([Id], [Title], [Author], [ISBN], [Genre], [IsAvailable], [RentalCount]) VALUES (2, N'To Kill a Mockingbird', N'Harper Lee', N'9780060935467', N'Classics', 1, 0)
INSERT [dbo].[Books] ([Id], [Title], [Author], [ISBN], [Genre], [IsAvailable], [RentalCount]) VALUES (3, N'Pride and Prejudice', N'Jane Austen', N'9780141199078', N'Romance', 1, 1)
INSERT [dbo].[Books] ([Id], [Title], [Author], [ISBN], [Genre], [IsAvailable], [RentalCount]) VALUES (4, N'The Catcher in the Rye', N'J.D. Salinger', N'9780316769488', N'Classics', 1, 1)
INSERT [dbo].[Books] ([Id], [Title], [Author], [ISBN], [Genre], [IsAvailable], [RentalCount]) VALUES (5, N'The Hobbit', N'J.R.R. Tolkien', N'9780547928227', N'Fantasy', 1, 0)
SET IDENTITY_INSERT [dbo].[Books] OFF
GO
ALTER TABLE [dbo].[Books] ADD  DEFAULT ((0)) FOR [RentalCount]
GO
