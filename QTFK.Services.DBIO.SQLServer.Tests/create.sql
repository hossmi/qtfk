use qtfk
go

CREATE TABLE dbo.persona(
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](256) NULL,
	[apellidos] [nvarchar](512) NULL,
	[fecha_nacimiento] [date] NULL,
	[hora_nacimiento] [time](7) NULL,
 CONSTRAINT [PK_persona] PRIMARY KEY CLUSTERED ([id])
) 

GO

CREATE TABLE dbo.[etiqueta](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](256) NULL,
 CONSTRAINT [PK_etiqueta] PRIMARY KEY CLUSTERED ([id] )
)

GO

CREATE TABLE dbo.[etiquetas_personas](
	[persona_id] [int] NOT NULL,
	[etiqueta_id] [int] NOT NULL,
 CONSTRAINT [PK_etiquetas_personas] PRIMARY KEY CLUSTERED ([persona_id] ASC,[etiqueta_id] ASC)
) 

GO

ALTER TABLE dbo.[etiquetas_personas]  WITH CHECK 
ADD  CONSTRAINT [FK_etiquetas_personas__etiqueta] 
FOREIGN KEY([etiqueta_id])
REFERENCES dbo.[etiqueta] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE dbo.[etiquetas_personas] CHECK CONSTRAINT [FK_etiquetas_personas__etiqueta]
GO

ALTER TABLE dbo.[etiquetas_personas]  WITH CHECK 
ADD  CONSTRAINT [FK_etiquetas_personas__persona] 
FOREIGN KEY([persona_id])
REFERENCES dbo.[persona] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE dbo.[etiquetas_personas] CHECK CONSTRAINT [FK_etiquetas_personas__persona]
GO

